using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OneShot : BasicCard
{
    public override void initialize(CardController card)
    {
        isIngoringFieldCapacity = true;
        Field.OnCast.AddListener(ctx => { if (ctx == card) destroy(card); });
    }
    void destroy(CardController card) => Field.OnCardBeat?.Invoke(card);
}
