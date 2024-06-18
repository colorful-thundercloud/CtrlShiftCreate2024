using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Бафы/Одноразовый")]
public class BuffOneshot : BasicCard
{
    [SerializeField] Effect buff;
    public override void initialize()
    {
        isIngoringFieldCapacity = true;
        action = buff;
        Field.OnCast.AddListener(ctx => { if (ctx == action.Card) destroy(); });
    }

    void destroy() => Field.OnCardBeat?.Invoke(action.Card);

    public override bool cast()
    {
        if (Card.otherCard == null) return false;
        action.Directed(Card.otherCard);
        return true;
    }
}
