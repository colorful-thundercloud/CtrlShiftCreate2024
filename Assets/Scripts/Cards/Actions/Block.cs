using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block : Action
{
    [SerializeField] int blockTime;
    [SerializeField] AudioClip blockSound;
    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        base.Directed(card, targetTransform, targetStats);  
        void decreeseTime(bool myTurn)
        {
            Stat block = targetStats.GetStat("Blocked");
            if (block == null) return;
            block.Value--;
            if (block.Value == 0) GameManager.OnEndTurn.RemoveListener(decreeseTime);
        }

        Stat block = targetStats.GetStat("Blocked");
        Stat stat = (block != null)? block : new();
        stat.Name = "Blocked";
        stat.Value = blockTime;
        stat.maxValue = blockTime;
        targetStats.AddStat(stat);

        SoundPlayer.Play.Invoke(blockSound);
        GameManager.OnEndTurn.AddListener(decreeseTime);
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
