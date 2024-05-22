using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Атакующие/Обычный")]
public class UnitCard : BasicCard
{
    [SerializeField] Attack attack;
    [SerializeField] Health hp;
    public override void initialize(Card card)
    {
        action = attack;
        action.Initialize(card);
        hp.Initialize(action.card, action.card.GetComponent<Animator>());

    }
    public override void OnSelect()
    {
        Card.Selected = action;
    }
    public override bool OnClick()
    {
        if (!action.CheckAlies(action.card)) return false;
        Card.Selected.Directed(action.card);    
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
