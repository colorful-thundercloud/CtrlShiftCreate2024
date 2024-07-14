using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hand : MonoBehaviour
{
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
        foreach (GameObject item in t) hand.Remove(item);
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
            go = Instantiate(cardPrefab, HandPosition.position - new Vector3(0, 0, 0), Quaternion.identity);

            if (enemy)
            {
                go.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                go.tag = "enemyCard";
            }
            else
            {
                go.transform.localScale = new Vector3(2f, 2f, 1);
                go.tag = "myCard";
            }
            go.GetComponent<CardController>().SetCard(card);
            hand.Add(go);

            if (enemy)
                foreach (Transform t in go.transform) t.gameObject.SetActive(false);

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
        hand.Remove(card.gameObject);
        deckController.BeatCard(card.GetBasicCard);
        Destroy(card.gameObject);
    }
}
