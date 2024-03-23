using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class DeckController : MonoBehaviour
{
    public CardSet cardSet;
    public List<BasicCard> currentDeck;
    public List<BasicCard> beaten = new();
    int counter;
    BasicCard card;
    void Start()
    {
        foreach (BasicCard card in cardSet.cards)
            currentDeck.Add(card);
        ShuffleDeck();
    }

    void ShuffleDeck()
    {
        System.Random random = new();
        currentDeck = currentDeck.OrderBy(x => random.Next()).ToList<BasicCard>();
    }

    public BasicCard DrawCard() {
        card = currentDeck.LastOrDefault<BasicCard>();
        if (currentDeck.Count > 0) currentDeck.RemoveAt(currentDeck.Count - 1);
        else MoveBeatenToDeck();
        return card;
    }

    public void BeatCard(BasicCard card) {
        beaten.Add(card);
    }

    public void MoveBeatenToDeck()
    {
        currentDeck = beaten;
        beaten = new();
        ShuffleDeck();
    }
}
