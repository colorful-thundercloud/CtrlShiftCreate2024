using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Бафы/Юнит")]
public class EffectUnit : BasicCard, IHaveSteps
{
    [field: SerializeField] public Steps Steps { get; set; }
    [SerializeField] Effect buff;
    [SerializeField] bool directed = false;
    [SerializeField] Health hp;

    public override void initialize(CardController card)
    {
        action = buff;
    }
    public override bool OnClick(CardController card)
    {
        if (!CardController.Selected.GetBasicCard.GetAction()
            .CheckAlies(CardController.Selected, card)) return false;
        CardController.Selected.GetBasicCard.GetAction()
            .Directed(CardController.Selected, card.transform, card.GetStats);
        return true;
    }
    public override bool cast(CardController card) { return true; }

    public override List<Stat> GetBasicStats(CardController card)
    {
        List<Stat> stats = new();

        IHaveSteps stepsCard = this;
        stats.Add(stepsCard.Steps.GetStat(card));
        TurnBasedGameplay.OnEndTurn.AddListener(isEnemy => stepsCard.Steps.reloadSteps(card, isEnemy));

        stats.Add(hp.GetStat(card, card));
        stats.Add(action.GetStat(card));

        return stats;
    }
    public override void OnSelect(CardController card)
    {
        if (!directed) return;
        CardController.Selected = null;
        action.Undirected(card);
    }
}
