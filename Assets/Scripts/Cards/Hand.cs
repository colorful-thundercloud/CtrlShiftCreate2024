using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Hand : MonoBehaviour
{
    [SerializeField] DeckController deckController;
    public List<GameObject> hand = new List<GameObject>();
    [SerializeField] Transform HandPosition;
    [SerializeField] float distance;
    [SerializeField] GameObject cardPrefab;
    private void Start()
    {
        Field.OnCast += ctx => updateHand();
        Invoke("InitiateHand", 1);
    }
    void updateHand()
    {
        foreach( GameObject item in hand ) if(item.GetComponent<Card>().IsCasted) hand.Remove(item);
        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 pos = HandPosition.position;
            pos.x = distance * i;
            hand[i].transform.position = pos;
        }
        float center = hand[hand.Count - 1].transform.position.x / 2;
        foreach (GameObject item in hand)
        {
            item.transform.Translate(-center, 0, 0);
            item.GetComponent<Card>().SavePosition();
        }
        Debug.Log(hand);
    }
    public void addCard(BasicCard card)
    {
        GameObject go = Instantiate(cardPrefab, HandPosition.position, Quaternion.identity);
        go.GetComponent<Card>().SetCard(card);
        hand.Add(go);
        updateHand();
    }

    void InitiateHand() {
        for (int i = 0; i < 3; i++)
        {
            addCard(deckController.CardDraw());
        }
    }
}
