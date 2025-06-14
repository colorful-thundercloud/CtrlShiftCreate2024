using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] bool online = true;
    [SerializeField] Animator GameOverWindow;
    [SerializeField] Button endMoveBtn;
    [SerializeField] EnemyController enemyController;
    [SerializeField] Timer timer;

    [SerializeField] DeckController myDeck, enemyDeck;
    [SerializeField] Hand myHand, enemyHand;
    [SerializeField] Transform myField, enemyField;

    [SerializeField] float distance;

    [SerializeField] Animator moneyAnim;
    [SerializeField] AudioClip moneySound;
    [SerializeField] TMP_Text turnText;
    List<TurnData> myTurns = new();

    public Vector3 CardSize = new Vector3(1.5f, 1.5f, 1);
    static List<GameObject> myCards, enemyCards;

    public static UnityEvent<CardController> OnCast = new();
    public static UnityEvent<bool> OnEndTurn = new();
    public static UnityEvent<CardController> OnCardBeat = new();
    public static UnityEvent<TurnData> UpdateTurns = new();
    public static UnityEvent<bool> OnGameOver = new();

    public static bool myTurn { get; private set; }
    void Awake()
    {
        myCards = new(); enemyCards = new();
        Time.timeScale = 1f;
        UpdateTurns.AddListener(updateTurns);
        OnCardBeat.AddListener(beatCard);
        OnCast.AddListener(ctx => addCard(ctx, ctx.CompareTag("enemyCard")));
        OnEndTurn.AddListener(setTurn);
        OnGameOver.AddListener(GameOver);
    }
    private void Start()
    {
        myDeck.SetSet(GetCardSet(IsServer ? 0 : 1));
        enemyDeck.SetSet(GetCardSet(IsServer ? 1 : 0));
        if (online) return;

        StartCoroutine(StartGame(myTurn));
    }
    public CardSet GetCardSet(int playerID)
    {
        List<CardSet> sets = Resources.LoadAll<CardSet>("Sets").ToList();
        sets.Sort((x, y) => x.Price.CompareTo(y.Price));
        int id = LobbyOrchestrator.PlayersInCurrentLobby[playerID].SetId;
        return sets[id];
    }
    public override void OnDestroy()
    {
        base.OnDestroy(); 
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }
    private void OnClientDisconnectCallback(ulong id)
    {
        OnGameOver.Invoke(true);
    }
    private void GameOver(bool isWin)
    {
        Debug.Log($"isWin = {isWin}");
        if (GameOverWindow.gameObject.activeSelf) return;
        GameOverWindow.gameObject.SetActive(true);
        GameOverWindow.SetBool("IsWin", isWin);
    }
    private void updateTurns(TurnData turnData) => myTurns.Add(turnData);
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        StartCoroutine(StartGame(myTurn));
    }
    public static void startGame(bool turn) => myTurn = turn;
    private IEnumerator StartGame(bool turn)
    {
        yield return StartCoroutine(lot(turn));
        setTurn(turn);
    }
    #region TurnBase

    public void Lot(bool myTurn) => StartCoroutine(lot(myTurn));
    public IEnumerator lot(bool turn)
    {
        turnText.text = MenuController.GetLocalizedString(turn ? "YourTurn" : "EnemyTurn");
        SoundPlayer.Play.Invoke(moneySound);
        moneyAnim.SetTrigger(turn ? "MyLot" : "EnemyLot");
        yield return new WaitForSeconds(3f);
    }

    [Rpc(SendTo.NotMe)]
    private void EndTurnRpc(TurnData[] turns)
    {
        //string[] names = turns.Select(turn => $"{turn.CardName}: {turn.Action} to target ID: {turn.TargetId}").ToArray();
        //string turnDebug = $"Turns receive \n Turn count = {turns.Count()}:\n{string.Join("\n", names)}";
        //Debug.Log(turnDebug);

        if (turns != null && turns.Count() != 0)
            enemyController.MakeTurn(turns);
        else OnEndTurn.Invoke(true);
    }
    public void NextTurn()
    {
        //string[] names = myTurns.Select(turn=> $"{turn.CardName}: {turn.Action} to target ID: {turn.TargetId}").ToArray();
        //string turnDebug = $"Turns send rival \n Turn count = {myTurns.Count}:\n{string.Join("\n", names)}";
        //Debug.Log(turnDebug);

        timer.Set(false);

        EndTurnRpc(myTurns.ToArray());

        OnEndTurn.Invoke(false);
        CardController.Selected = null;
    }
    private void setTurn(bool turn)
    {
        if (SceneManager.GetActiveScene().name == "Tutorial") return;
        myTurns.Clear();

        if (turn)
        {
            if (GameOverWindow.gameObject.activeSelf) return;
            List<BasicCard> cards = myHand.DrawCards();
            if (cards.Count != 0)
            {
                string cardIds = string.Join(";", cards.Select(card => card.Title).ToArray());
                myHand.AddCards(cards);
                DrawEnemyRpc(cardIds);
            }
            timer.Set(true);
        }
        else if(!online)
        {
            foreach(var card in enemyHand.DrawCards())
                enemyHand.AddCards(card);
            enemyController.SingleAI();
        }
        endMoveBtn.interactable = turn;
        myTurn = turn;
    }
    [Rpc(SendTo.NotMe)]
    private void DrawEnemyRpc(string cardIds)
    {
        enemyHand.AddCards(cardIds.Split(";"), true);
    }
    #endregion
    #region Field

    public bool CheckCount(bool isEnemy = false)
    {
        if (isEnemy) return enemyCards.Count < Constants.MaxCardsInField;
        else return myCards.Count < Constants.MaxCardsInField;
    }
    public Transform GetEnemyField { get { return enemyField; } }
    private void addCard(CardController card, bool isEnemy)
    {
        if (isEnemy) enemyCards.Add(card.gameObject);
        else myCards.Add(card.gameObject);
        updateField(isEnemy);
    }
    private void updateField(bool isEnemy)
    {
        List<GameObject> field = isEnemy ? enemyCards : myCards;

        StopAllCoroutines();
        if (field.Count == 0) return;
        float center = ((field.Count - 1) * distance) / 2f;
        for (int i = 0; i < field.Count; i++)
        {
            field[i].transform.SetParent(isEnemy ? enemyField : myField);
            Vector3 pos = field[i].transform.position;
            pos.y = isEnemy ? enemyField.position.y : myField.position.y;
            pos.x = (distance * i) - center;
            pos.z = pos.z == -20 ? pos.z : 4;

            StartCoroutine(Mover.MoveCard(field[i].transform, pos, 0.1f));
            field[i].transform.localScale = CardSize;
            var card = field[i].GetComponent<CardController>();
            card.SaveScale();
        }
    }
    private void beatCard(CardController card)
    {
        if (card.gameObject.CompareTag("myCard"))
        {
            myCards.Remove(card.gameObject);
            updateField(false);
            myDeck.BeatCard(card.GetBasicCard);
        }
        else
        {
            enemyCards.Remove(card.gameObject);
            updateField(true);
            enemyDeck.BeatCard(card.GetBasicCard);
        }
        Destroy(card.gameObject);
    }
    public CardController FindCard(int id)
    {
        CardController target = GetCards(true).Find(card => card.cardID == id);
        if (target == default) target = GetCards(false).Find(card => card.cardID == id);
        if (target == default) target = myHand.FindCard(id);
        if (target == default) target = enemyHand.FindCard(id);
        return target;
    }
    public static List<CardController> GetCards(bool isEnemy)
    {
        List<CardController> cards;
        if (isEnemy) cards = enemyCards.ConvertAll(n => n.GetComponent<CardController>());
        else cards = myCards.ConvertAll(n => n.GetComponent<CardController>());
        return cards;
    }
    #endregion
}
