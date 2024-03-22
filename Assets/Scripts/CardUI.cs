using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    public static Action<BasicCard> OnOpenCard;
    private void Start()
    {
        OnOpenCard += ctx => openCard(ctx);
    }
    void openCard(BasicCard card)
    {

    }
}
