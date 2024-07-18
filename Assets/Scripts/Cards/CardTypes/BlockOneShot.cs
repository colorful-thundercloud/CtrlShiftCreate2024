using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Блок/Одноразовый")]
public class BlockOneShot : OneShot
{
    [SerializeField] Block block;
    public override void initialize(CardController card)
    {
        base.initialize(card);
        action = block;
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
        stats.Add(block.GetStat(card));
        return stats;
    }
}
