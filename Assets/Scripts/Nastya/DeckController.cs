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

    void CreateDeck(List<CardSet.Card> cardSet)
    {
        foreach (CardSet.Card card in cardSet)
        {
            for (counter = 0; counter < card.amountInDeck; counter++)
            {
                currentDeck.Add(card.card);
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

    BasicCard CardDraw() {
        card = currentDeck.LastOrDefault<BasicCard>();
        currentDeck.RemoveAt(currentDeck.Count - 1);
        return card;
    }

    public void CardBeat(BasicCard card) {
        beaten.Add(card);
    }
}
