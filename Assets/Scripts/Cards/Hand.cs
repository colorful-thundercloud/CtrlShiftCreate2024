using System.Collections.Generic;
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
        Field.OnCast += ctx => updateHand();
        Invoke("DrawCards", 1);
    }
    void updateHand()
    {
        List<GameObject> t = new List<GameObject>();
        foreach( GameObject item in hand ) if(item.GetComponent<Card>().IsCasted) t.Add(item);
        foreach (GameObject item in t) hand.Remove(item);
        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 pos = HandPosition.position;
            pos.x = distance * i;
            hand[i].transform.position = pos;
        }
        if (hand.Count == 0) return;
        float center = hand[hand.Count - 1].transform.position.x / 2;
        foreach (GameObject item in hand)
        {
            item.transform.Translate(-center, 0, 0);
            item.GetComponent<Card>().SavePosition();
        }
        Debug.Log(hand.ToArray());
    }
    public void addCard(BasicCard card)
    {
        if (card != null)
        {
            GameObject go = Instantiate(cardPrefab, HandPosition.position, Quaternion.identity);
            go.GetComponent<Card>().SetCard(card);
            hand.Add(go);
            updateHand();
        }
    }
    public void DrawCards() 
    {
        for (int i = 3; i > hand.Count; i--)
            addCard(deckController.CardDraw());
    }
    public List<Card> GetCards()
    {
        List<Card> cards;
        cards = hand.ConvertAll(n => n.GetComponent<Card>());
        return cards;
    }
}
