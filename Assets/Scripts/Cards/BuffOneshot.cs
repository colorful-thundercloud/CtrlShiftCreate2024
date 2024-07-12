using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Бафы/Одноразовый")]
public class BuffOneshot : BasicCard
{
    [SerializeField] Effect buff;
    public override void initialize(CardController card)
    {
        isIngoringFieldCapacity = true;
        action = buff;
        Field.OnCast.AddListener(ctx => { if (ctx == card) destroy(card); });
    }

    void destroy(CardController card) => Field.OnCardBeat?.Invoke(card);

    public override bool cast(CardController card)
    {
        if (CardController.otherCard == null) return false;
        action.Directed(card, CardController.otherCard);
        return true;
    }
}
