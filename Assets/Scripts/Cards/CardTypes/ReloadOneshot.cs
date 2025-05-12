using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Бафы/Перезарядка")]
public class ReloadOneshot : OneShot
{
    [SerializeField] Reload reload;
    public override void initialize(CardController card)
    {
        base.initialize(card);
        action = reload;
    }

    public override bool cast(CardController card)
    {
        if(!action.GetAllTargets(card).Any(target=> target.GetBasicCard.GetType().GetInterfaces().Contains(typeof(IHaveSteps)))) 
            return false;
        action.Undirected(card);
        return true;
    }
    public override List<Stat> GetBasicStats(CardController card)
    {
        List<Stat> stats = new();
        return stats;
    }
}
