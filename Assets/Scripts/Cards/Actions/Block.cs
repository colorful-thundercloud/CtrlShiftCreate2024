using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block : Action
{
    [SerializeField] int blockTime;
    public override bool CheckAviability(CardController card)
    {
        Stat steps = card.GetStat("steps");
        if (steps != null) return steps.Value > 0;
        else return true;
    }
    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        void decreeseTime(bool isEnemy)
        {
            Stat block = targetStats.GetStat("Blocked");
            if (block == null) return;
            block.Value--;
            if (block.Value == 0) TurnBasedGameplay.OnEndTurn.RemoveListener(decreeseTime);
        }
        Stat stat = new();
        stat.Name = "Blocked";
        stat.Value = blockTime;
        stat.maxValue = blockTime;
        targetStats.AddStat(stat);
        TurnBasedGameplay.OnEndTurn.AddListener(decreeseTime);
    }
    public override Stat GetStat(CardController card)
    {
        Stat stat = new();
        stat.Name = "BlockTime";
        stat.Value = blockTime;
        stat.maxValue = blockTime;
        return stat;
    }
}
