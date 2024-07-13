using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Атакующие/Обычный")]
public class UnitCard : BasicCard
{
    [SerializeField] Attack attack;
    [SerializeField] Health hp;
    public override void initialize(CardController card)
    {
        action = attack;
    }
    public override bool OnClick(CardController card)
    {
        if (!CardController.Selected.GetBasicCard.GetAction()
            .CheckAlies(CardController.Selected,card)) return false;
        CardController.Selected.GetBasicCard.GetAction()
            .Directed(CardController.Selected, card.transform, card.GetStats);    
        return true;
    }
    public override bool cast(CardController card) { return true; }
    
    public override List<Stat> GetBasicStats(CardController card)
    {
        List<Stat> stats = base.GetBasicStats(card);

        stats.Add(hp.GetStat(card, card));
        stats.Add(action.GetStat(card));

        return stats;
    }
}
