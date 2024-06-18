using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Атакующие/Обычный")]
public class UnitCard : BasicCard
{
    [SerializeField] Attack attack;
    [SerializeField] Health hp;
    public override void initialize()
    {
        action = attack;
        action.steps = 0;
    }
    public override bool OnClick()
    {
        if (!Card.Selected.GetBasicCard.GetAction().CheckAlies(action.Card)) return false;
        Card.Selected.GetBasicCard.GetAction().Directed(action.Card);    
        return true;
    }
    public override bool cast() { return true; }
    public override IHaveStats TryGetAttack()
    {
        return action.TryGetStats();
    }
    public override Health TryGetHealth()
    {
        return hp;
    }
}
