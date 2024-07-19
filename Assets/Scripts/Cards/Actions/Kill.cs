using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Kill : Action
{
    [SerializeField] AudioClip killSound;
    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        base.Directed(card, targetTransform, targetStats);
        targetStats.GetStat("hp").Value -= targetStats.GetStat("hp").Value;
        SoundPlayer.Play.Invoke(killSound);
    }
}
