using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Field : MonoBehaviour
{
    [SerializeField] DeckController myDeck, enemyDeck;
    [SerializeField] Hand myHand, enemyHand;
    [SerializeField] Transform myField, enemyField;
    [SerializeField] float distance;
    [SerializeField] int maxCardCount;
    public Vector3 CardSize = new Vector3(1.5f, 1.5f, 1);
    public bool CheckCount(bool isEnemy = false)
    {
        if(isEnemy) return enemyCards.Count < maxCardCount;
        else return myCards.Count < maxCardCount;
    }
    static List<GameObject> myCards, enemyCards;
    public static UnityEvent<CardController> OnCast = new();
    public static UnityEvent<CardController> OnCardBeat = new();
    private void Start()
    {
        Time.timeScale = 1f;
        OnCast.AddListener(ctx => addCard(ctx, ctx.CompareTag("enemyCard")));
        OnCardBeat.AddListener(BeatCard);
        myCards = new();
        enemyCards = new();
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
        if(enemyField==null) return;
        List<GameObject> field = isEnemy ? enemyCards : myCards;
        for (int i = 0; i < field.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.y = isEnemy ? enemyField.position.y : myField.position.y;
            pos.x = distance * i;
            pos.z = 4;
            field[i].transform.position = pos;
            field[i].transform.localScale = CardSize;
        }
        if (field.Count == 0) return;
        float center = field[field.Count - 1].transform.position.x / 2;
        foreach (GameObject item in field) item.transform.Translate(-center, 0, 0);
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
    public void Clear()
    {
        while(myCards.Count > 0) BeatCard(myCards[0].GetComponent<CardController>());
        while(enemyCards.Count > 0) BeatCard(enemyCards[0].GetComponent<CardController>());
    }
}
