using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Казнь")]
public class KillCard : OneShot
{
    [SerializeField] Kill kill;
    public override void initialize(CardController card)
    {
        base.initialize(card);
        action = kill;
    }

    public override bool cast(CardController card)
    {
        if (CardController.otherCard == null) return false;
        if (!action.CheckAlies(card, CardController.otherCard)) return false;
        action.Directed(card, CardController.otherCard.transform, CardController.otherCard.GetStats);
        return true;
    }
    public override List<Stat> GetBasicStats(CardController card)
    {
        List<Stat> stats = new();
        return stats;
    }
}
