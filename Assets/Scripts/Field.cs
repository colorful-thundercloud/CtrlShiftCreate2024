using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] DeckController myDeck, enemyDeck;
    [SerializeField] Hand myHand, enemyHand;
    [SerializeField] Transform myField, enemyField;
    [SerializeField] float distance;
    List<GameObject> myCards = new List<GameObject>();
    List<GameObject> enemyCards = new List<GameObject>();
    public static Action<Card> OnCast;
    public static Action<Card> OnEnemyCast;
    public static Action<Card> OnCardBeat;
    public static Card SelectedCard;
    private void Start()
    {
        Time.timeScale = 1f;
        OnCast += ctx => addCard(ctx, false);
        OnEnemyCast += ctx => addCard(ctx, true);
        OnCardBeat += ctx => BeatCard(ctx);
    }
    public Transform GetEnemyField()
    {
        return enemyField;
    }
    public void addCard(Card card, bool isEnemy)
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
            pos.z = field[i].transform.position.z;
            field[i].transform.position = pos;
        }
        if (field.Count == 0) return;
        float center = field[field.Count - 1].transform.position.x / 2;
        foreach (GameObject item in field) item.transform.Translate(-center, 0, 0);
    }
    public void BeatCard(Card card)
    {
        if (card.gameObject.CompareTag("myCard"))
        {
            myCards.Remove(card.gameObject);
            updateField(false);
            myDeck.BeatCard(card.GetBasicCard);
            myHand.RemoveCard(card.gameObject);
        }
        else
        {
            enemyCards.Remove(card.gameObject);
            updateField(true);
            enemyDeck.BeatCard(card.GetBasicCard);
            enemyHand.RemoveCard(card.gameObject);
        }
        Destroy(card.gameObject);
    }
    public List<Card> GetCards(bool isEnemy)
    {
        List<Card> cards;
        if (isEnemy) cards = enemyCards.ConvertAll(n => n.GetComponent<Card>());
        else cards = myCards.ConvertAll(n => n.GetComponent<Card>());
        return cards;
    }
}
