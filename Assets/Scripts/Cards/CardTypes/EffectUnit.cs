using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Карты/Бафы/Юнит")]
public class EffectUnit : BasicCard, IHaveSteps
{
    [field: SerializeField] public Steps Steps { get; set; }
    [SerializeField] Effect buff;
    [SerializeField] bool directed = true;
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
    public override bool cast(CardController card)
    {
        card.GetStat("steps").Value = 0;
        return true;
    }

    public override List<Stat> GetBasicStats(CardController card)
    {
        List<Stat> stats = new();

        IHaveSteps stepsCard = this;
        stats.Add(stepsCard.Steps.GetStat(card));
        GameManager.OnEndTurn.AddListener(myTurn => stepsCard.Steps.reloadSteps(card, myTurn));

        stats.Add(hp.GetStat(card, card));
        stats.Add(action.GetStat(card));

        return stats;
    }
    public override void OnSelect(CardController card)
    {
        if (directed) return;
        if (!card.GetBasicCard.CheckAction(card)) return;
        if (card.GetStat("steps") != null) card.GetStat("steps").Value--;
        CardController.Selected = null;
        action.Undirected(card);
    }
}
