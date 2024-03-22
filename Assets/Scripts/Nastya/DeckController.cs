using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class DeckController : MonoBehaviour
{
    public CardSet cardSet;
    public List<BasicCard> currentDeck;
    public List<BasicCard> beaten;
    int counter;
    BasicCard card;
    void Start()
    {
        CreateDeck(cardSet.cards);
    }

    void CreateDeck(List<BasicCard> cardSet)
    {
        foreach (BasicCard card in cardSet)
        {
            for (counter = 0; counter < card.GetCardCount; counter++)
            {
                currentDeck.Add(card);
            }
        }
        beaten = new();
        ShuffleDeck();
    }

    void ShuffleDeck()
    {
        System.Random random = new();
        currentDeck = currentDeck.OrderBy(x => random.Next()).ToList<BasicCard>();
    }

    public BasicCard CardDraw() {
        card = currentDeck.LastOrDefault<BasicCard>();
        currentDeck.RemoveAt(currentDeck.Count - 1);
        return card;
    }

    public void CardBeat(BasicCard card) {
        beaten.Add(card);
    }
}
