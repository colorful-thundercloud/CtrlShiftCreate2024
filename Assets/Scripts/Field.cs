using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{

    List<BasicCard> myCards;
    List<BasicCard> enemyCards;
    public static Action<Card> OnCast;
    private void Start()
    {
        OnCast += ctx => addCard(ctx);
    }
    void addCard(Card card)
    {
        myCards.Add(card.card);
    }
}
