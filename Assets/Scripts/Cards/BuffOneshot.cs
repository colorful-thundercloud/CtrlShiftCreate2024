using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Бафы/Одноразовый")]
public class BuffOneshot : BasicCard
{
    [SerializeField] Effect buff;
    public override void initialize(Card card)
    {
        action = buff;
        action.Initialize(card);
        Field.OnCast.AddListener(ctx => { if (ctx == card) destroy(); });
    }

    void destroy() => Field.OnCardBeat?.Invoke(action.card);

    public override bool cast()
    {
        if (Card.otherCard == null) return false;
        action.Directed(Card.otherCard);
        return true;
    }
}
