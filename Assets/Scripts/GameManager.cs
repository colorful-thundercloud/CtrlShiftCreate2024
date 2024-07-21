using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button endMoveBtn;
    [SerializeField] DeckController myDeck, enemyDeck;
    [SerializeField] Hand myHand, enemyHand;
    [SerializeField] Transform myField, enemyField;
    [SerializeField] float distance;
    [SerializeField] int maxCardCount;
    public Vector3 CardSize = new Vector3(1.5f, 1.5f, 1);
    public bool CheckCount(bool isEnemy = false)
    {
        if (isEnemy) return enemyCards.Count < maxCardCount;
        else return myCards.Count < maxCardCount;
    }
    static List<GameObject> myCards, enemyCards;
    public static UnityEvent<CardController> OnCast = new();
    public static UnityEvent<CardController> OnCardBeat = new();
    public static UnityEvent<bool> OnEndTurn = new();
    public static bool myTurn = true;
    [HideInInspector]
    public static bool endMoveBtnIsOff = false;
    void Awake()
    {
        //Invoke("DrawCards", 0.5f);
        OnEndTurn.AddListener(onNextTurn);
    }
    private void Start()
    {
        Time.timeScale = 1f;
        OnCast.AddListener(ctx => addCard(ctx, ctx.CompareTag("enemyCard")));
        OnCardBeat.AddListener(BeatCard);
        myCards = new();
        enemyCards = new();
    }
    private void Update()
    {
        if (endMoveBtnIsOff || !myTurn) endMoveBtn.interactable = false;
        else endMoveBtn.interactable = true;
    }
    void DrawCards()
    {
        endMoveBtn.interactable = true;
    }

    public void NextTurn(bool isEnemyTurn) => OnEndTurn.Invoke(isEnemyTurn);
    void onNextTurn(bool isEnemyTurn)
    {
        endMoveBtn.interactable = !isEnemyTurn;
        myTurn = !isEnemyTurn;
        endMoveBtnIsOff = isEnemyTurn;
        CardController.Selected = null;
        if (isEnemyTurn) enemyHand.DrawCards(true);
        else myHand.DrawCards();
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
        if (enemyField == null) return;
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

            StartCoroutine(Mover.MoveCard(field[i].GetComponent<CardController>(), pos, 0.1f));
            field[i].transform.localScale = CardSize;
            field[i].GetComponent<CardController>().SaveScale();
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
        endMoveBtnIsOff = false;
    }
    public static List<CardController> GetCards(bool isEnemy)
    {
        List<CardController> cards;
        if (isEnemy) cards = enemyCards.ConvertAll(n => n.GetComponent<CardController>());
        else cards = myCards.ConvertAll(n => n.GetComponent<CardController>());
        return cards;
    }
    public void Clear()
    {
        while (myCards.Count > 0) BeatCard(myCards[0].GetComponent<CardController>());
        while (enemyCards.Count > 0) BeatCard(enemyCards[0].GetComponent<CardController>());
    }
}
