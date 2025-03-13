using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections;
using TMPro;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    [SerializeField] Button endMoveBtn;
    [SerializeField] EnemyController enemyController;

    [SerializeField] DeckController myDeck, enemyDeck;
    [SerializeField] Hand myHand, enemyHand;
    [SerializeField] Transform myField, enemyField;

    [SerializeField] float distance;
    [SerializeField] int maxCardCount;

    [SerializeField] Animator moneyAnim;
    [SerializeField] AudioClip moneySound;
    [SerializeField] TMP_Text turnText;

    List<TurnData> myTurns = new();

    public Vector3 CardSize = new Vector3(1.5f, 1.5f, 1);
    public bool CheckCount(bool isEnemy = false)
    {
        if (isEnemy) return enemyCards.Count < maxCardCount;
        else return myCards.Count < maxCardCount;
    }
    static List<GameObject> myCards, enemyCards;

    public static UnityEvent<CardController> OnCast = new();
    public static UnityEvent<bool> OnEndTurn = new();
    public static UnityEvent<CardController> OnCardBeat = new();
    public static UnityEvent<TurnData> UpdateTurns = new(); 

    public static bool myTurn { get; private set; }
    private Lobby currentLobby;
    void Awake()
    {
        UpdateTurns.AddListener(updateTurns);
        OnCardBeat.AddListener(BeatCard);
        OnCast.AddListener(ctx => addCard(ctx, ctx.CompareTag("enemyCard")));
        OnEndTurn.AddListener(setTurn);
    }
    private void updateTurns(TurnData turnData) => myTurns.Add(turnData);
    public override void OnNetworkSpawn()
    {
        Time.timeScale = 1f;
        myCards = new();
        enemyCards = new();
        if (!IsServer) ClientConnectedServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClientConnectedServerRpc() => firstTurnSet();
    private async void firstTurnSet()
    {
        if (IsServer)
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(LobbyOrchestrator.CurrentLobbyId);
            Constants.FirstTurn turnType = (Constants.FirstTurn)int.Parse(currentLobby.Data[Constants.FirstTurnKey].Value);
            bool turn = (turnType == Constants.FirstTurn.Random) ? Random.Range(0, 2) == 0 : turnType == Constants.FirstTurn.Host;
            StartCoroutine(lot(turn));
            turnClientRpc(!turn);
        }
    }
    [ClientRpc]
    private void turnClientRpc(bool turn)
    {
        if (IsServer) return;
        StartCoroutine(lot(turn));
    }
    private IEnumerator lot(bool turn)
    {
        turnText.text = turn ? "Твой ход" : "Ход противника";
        SoundPlayer.Play.Invoke(moneySound);
        moneyAnim.SetTrigger(turn ? "MyLot" : "EnemyLot");
        yield return new WaitForSeconds(3f);
        setTurn(turn);
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndTurnServerRpc(bool turn, TurnData[] turns)
    {
        OnEndTurn.Invoke(false);
        if (turns != null && turns.Count() != 0)
            enemyController.MakeTurn(turns);
        else OnEndTurn.Invoke(true);
    }
    [ClientRpc]
    private void EndTurnClientRpc(bool turn, TurnData[] turns)
    {
        if (IsServer) return;

        OnEndTurn.Invoke(false);
        if (turns != null && turns.Count() != 0)
            enemyController.MakeTurn(turns);
        else OnEndTurn.Invoke(true);
    }
    public void NextTurn()
    {
        string[] names = myTurns.Select(turn=>turn.CardName).ToArray();
        Debug.Log($"Turn count = {myTurns.Count}: {string.Join("; ", names)}");
        if(IsServer) EndTurnClientRpc(true, myTurns.ToArray());
        else EndTurnServerRpc(true, myTurns.ToArray());
        CardController.Selected = null;
        setTurn(false);
    }
    private void setTurn(bool turn)
    {
        myTurns.Clear();

        if (turn)
        {
            List<BasicCard> cards = myHand.DrawCards();
            if (cards.Count != 0)
            {
                string cardIds = string.Join(";", cards.Select(card => card.Title).ToArray());
                myHand.AddCards(cards);
                if (IsServer) DrawEnemyClientRpc(cardIds);
                else DrawEnemyServerRpc(cardIds);
            }
        }
        endMoveBtn.interactable = turn;
        myTurn = turn;
    }
    [ServerRpc(RequireOwnership = false)]
    private void DrawEnemyServerRpc(string cardIds)
    {
        enemyHand.AddCards(cardIds.Split(";"), true);
    }
    [ClientRpc]
    private void DrawEnemyClientRpc(string cardIds)
    {
        if (IsServer) return;
        enemyHand.AddCards(cardIds.Split(";"), true);
    }
    public Transform GetEnemyField { get { return enemyField; } }
    public void addCard(CardController card, bool isEnemy)
    {
        if (isEnemy) enemyCards.Add(card.gameObject);
        else myCards.Add(card.gameObject);
        updateField(isEnemy);
    }
    public void updateField(bool isEnemy)
    {
        //if (enemyField == null) return;
        List<GameObject> field = isEnemy ? enemyCards : myCards;

        StopAllCoroutines();
        if (field.Count == 0) return;
        float center = ((field.Count - 1) * distance) / 2f;
        for (int i = 0; i < field.Count; i++)
        {
            Vector3 pos = field[i].transform.position;
            pos.y = isEnemy ? enemyField.position.y : myField.position.y;
            pos.x = (distance * i) - center;
            pos.z = 4;

            StartCoroutine(Mover.MoveCard(field[i].transform, pos, 0.1f));
            field[i].transform.localScale = CardSize;
            var card = field[i].GetComponent<CardController>();
            card.SaveScale();
            card.cardID = i;
        }
    }
    public void BeatCard(CardController card)
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
    public static List<CardController> GetCards(bool isEnemy)
    {
        List<CardController> cards;
        if (isEnemy) cards = enemyCards.ConvertAll(n => n.GetComponent<CardController>());
        else cards = myCards.ConvertAll(n => n.GetComponent<CardController>());
        return cards;
    }
    /*public void Clear()
    {
        while (myCards.Count > 0) BeatCard(myCards[0].GetComponent<CardController>());
        while (enemyCards.Count > 0) BeatCard(enemyCards[0].GetComponent<CardController>());
    }*/
}
