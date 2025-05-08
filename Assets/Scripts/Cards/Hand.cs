using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Hand : MonoBehaviour
{
    [SerializeField] Vector3 CardSize = new Vector3(1, 1, 1);
    [SerializeField] DeckController deckController;
    List<GameObject> hand = new List<GameObject>();
    [SerializeField] Transform HandPosition;
    [SerializeField] float distance;
    [SerializeField] GameObject cardPrefab;
    private void Start()
    {
        GameManager.OnCast.AddListener(ctx => updateHand());
    }
    void updateHand()
    {
        StopAllCoroutines();
        List<GameObject> t = new List<GameObject>();
        foreach( GameObject item in hand )
        {
            if (item == null) return;
            if(item.GetComponent<CardController>().isCasted) t.Add(item);
        }
        foreach (GameObject item in t) RemoveCard(item);

        if (hand.Count == 0) return;
        float center = ((hand.Count - 1)* distance)/2f;
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.SetParent(transform);
            Vector3 pos = HandPosition.position;
            pos.x = (distance * i) - center;
            pos.z = (hand[i].tag == "myCard")? 1f : 2f;
            StartCoroutine(kostyl(hand[i].GetComponent<CardController>(), pos));
        }

        for (int i = 0; i < hand.Count; i++)
            hand[i].GetComponent<CardController>().cardID = i;
    }
    IEnumerator kostyl(CardController card, Vector3 pos)
    {
        yield return StartCoroutine(Mover.MoveCard(card.transform, pos, 0.1f));
        card.SavePosition();
    }
    void addCard(BasicCard card, bool enemy = false)
    {
        GameObject go;
        if (card != null)
        {
            go = Instantiate(cardPrefab, HandPosition.position, Quaternion.identity);

            go.transform.localScale = CardSize;
            go.tag = (enemy) ? "enemyCard" : "myCard";

            go.GetComponent<CardController>().SetCard(card);
            hand.Add(go);

            if (enemy) go.GetComponent<CardController>().Show(false);

            updateHand();
        }
    }
    public List<BasicCard> DrawCards(bool enemy = false) 
    {
        if (deckController.cardCount == 0)
            deckController.MoveBeatenToDeck();
        List<BasicCard> cards = new();
        for (int i = hand.Count; i < Constants.MaxCardsInHand; i++)
            cards.Add(deckController.DrawCard());
        return cards;
    }
    public void AddCards(string[] cards, bool enemy = false)
    {
        foreach(var card in cards)
            addCard(deckController.DrawCard(card), enemy);
    }
    public void AddCards(List<BasicCard> cards, bool enemy = false)
    {
        foreach (var card in cards)
            addCard(card, enemy);
    }
    public void RemoveCard(GameObject card)
    {
        hand.Remove(card);
        updateHand();
    }
    public List<CardController> GetCards()
    {
        List<CardController> cards;
        cards = hand.ConvertAll(n => n.GetComponent<CardController>());
        return cards;
    }
    public void BeatCard(CardController card)
    {
        RemoveCard(card.gameObject);
        deckController.BeatCard(card.GetBasicCard);
        Destroy(card.gameObject);
    }
    public void clear()
    {
        while (hand.Count > 0) BeatCard(hand[0].GetComponent<CardController>());
    }
}
