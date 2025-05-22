using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Reload: Action
{
    [SerializeField] AudioClip sound;
    public override void Undirected(CardController card)
    {
        List<CardController> targets = GetAllTargets(card);
        targets.Remove(card);
        foreach (CardController target in targets) Directed(card, target.transform, target.GetStats);
        SoundPlayer.Play.Invoke(sound);
    }
    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        Stat steps = targetStats.GetStat("steps");
        if (steps != null) steps.Value = steps.maxValue;
    }
}
