using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Бафы/Одноразовый")]
public class BuffOneshot : OneShot
{
    [SerializeField] Effect buff;
    public override void initialize(CardController card)
    {
        base.initialize(card);
        action = buff;
    }

    public override bool cast(CardController card)
    {
        if (CardController.otherCard == null) return false;
        if (!action.CheckAlies(card, CardController.otherCard)) return false;
        action.Directed(card, CardController.otherCard.transform, CardController.otherCard.GetStats);
        return true;
    }
}
