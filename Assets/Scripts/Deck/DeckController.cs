using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class DeckController : MonoBehaviour
{
    [SerializeField] CardSet cardSet;
    List<BasicCard> currentDeck = new(), beaten = new();
    public int cardCount { get { return currentDeck.Count; } }
    BasicCard card;
    private void Start()
    {
        if(cardSet!=default) SetSet(cardSet);
    }
    public void SetSet(CardSet set)
    {
        currentDeck.Clear();
        cardSet = set; 
        foreach (BasicCard card in cardSet.Cards)
            currentDeck.Add(card);
        ShuffleDeck();
    }
    void ShuffleDeck()
    {
        System.Random random = new();
        currentDeck = currentDeck.OrderBy(x => random.Next()).ToList();
    }

    public BasicCard DrawCard(string title = "")
    {
        if (currentDeck.Count == 0) MoveBeatenToDeck();
        if(title != "")
            card = currentDeck.FirstOrDefault(card => card.Title == title);
        else card = currentDeck.LastOrDefault();
        currentDeck.Remove(card);
        return card;
    }

    public void BeatCard(BasicCard card)
    {
        beaten.Add(card);
    }

    public void MoveBeatenToDeck()
    {
        currentDeck.AddRange(beaten);
        beaten = new();
        ShuffleDeck();
    }
}
