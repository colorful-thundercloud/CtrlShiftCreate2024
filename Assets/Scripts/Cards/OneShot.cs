using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OneShot : BasicCard
{
    public override void initialize(CardController card)
    {
        isIngoringFieldCapacity = true;
        GameManager.OnCast.AddListener(ctx => { if (ctx == card) destroy(card); });
    }
    void destroy(CardController card) => GameManager.OnCardBeat?.Invoke(card);
}
