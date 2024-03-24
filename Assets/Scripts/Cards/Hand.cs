using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] DeckController deckController;
    List<GameObject> hand = new List<GameObject>();
    [SerializeField] Transform HandPosition;
    [SerializeField] float distance;
    [SerializeField] GameObject cardUnitPrefab, cardBuffPrefab;
    private void Start()
    {
        Field.OnCast += ctx => updateHand();
    }
    void updateHand()
    {
        List<GameObject> t = new List<GameObject>();
        foreach( GameObject item in hand ) if(item.GetComponent<Card>().isCasted) t.Add(item);
        foreach (GameObject item in t) hand.Remove(item);
        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 pos = HandPosition.position;
            pos.x = distance * i;
            pos.z = hand[i].transform.position.z;
            hand[i].transform.position = pos;
        }
        if (hand.Count == 0) return;
        float center = hand[hand.Count - 1].transform.position.x / 2;
        foreach (GameObject item in hand)
        {
            item.transform.Translate(-center, 0, 0);
            item.GetComponent<Card>().SavePosition();
        }
    }
    void addCard(BasicCard card, bool enemy = false)
    {
        GameObject go;
        if (card != null)
        {
            if (card.Type == BasicCard.cardType.Unit)
                go = Instantiate(cardUnitPrefab, HandPosition.position - new Vector3(0, 0, 4), Quaternion.identity);
            else
                go = Instantiate(cardBuffPrefab, HandPosition.position - new Vector3(0, 0, 5), Quaternion.identity);

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
            go.GetComponent<Card>().SetCard(card);
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
    }
    public List<Card> GetCards()
    {
        List<Card> cards;
        cards = hand.ConvertAll(n => n.GetComponent<Card>());
        return cards;
    }
}
