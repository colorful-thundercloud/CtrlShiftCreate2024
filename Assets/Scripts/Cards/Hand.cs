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
        Field.OnCast.AddListener(ctx => updateHand());
    }
    void updateHand()
    {
        List<GameObject> t = new List<GameObject>();
        foreach( GameObject item in hand )
        {
            if (item == null) return;
            if(item.GetComponent<CardController>().isCasted) t.Add(item);
        }
        foreach (GameObject item in t) RemoveCard(item);
        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 pos = HandPosition.position;
            pos.x = distance * i;
            pos.z = (hand[i].tag == "myCard")? 1f : 2f;
            hand[i].transform.position = pos;
        }
        if (hand.Count == 0) return;
        float center = hand[hand.Count - 1].transform.position.x / 2;
        foreach (GameObject item in hand)
        {
            item.transform.Translate(-center, 0, 0);
            item.GetComponent<CardController>().SavePosition();
        }
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
    public void DrawCards(bool enemy = false) 
    {
        if (deckController.currentDeck.Count == 0)
            deckController.MoveBeatenToDeck();

        for (int i = hand.Count; i < 3 ; i++)
            addCard(deckController.DrawCard(), enemy);
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
