using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Effect : Action
{
    public enum BuffedStats
    {
        damage,
        hp
    }
    /// <summary>
    /// Определяет баффаемый параметр
    /// </summary>
    [Header("Определяет баффаемый параметр")]
    [SerializeField] BuffedStats buffedStat;
    /// <summary>
    /// На сколько умножается выбранный параметр
    /// </summary>
    [Header("На сколько умножается выбранный параметр")]
    [SerializeField] int multiplier = 1;
    /// <summary>
    /// Сколько прибавляется к выбранному параметру
    /// </summary>
    [Header("Сколько прибавляется к выбранному параметру")]
    [SerializeField] int value = 0;
    [SerializeField] AudioClip buffSound;
    public override bool CheckAviability(CardController card)
    {
        return card.GetStat("steps").Value > 0;
    }


    public override void Undirected(CardController card)
    {
        List<CardController> targets = GetAllTargets(card);
        foreach (CardController target in targets) Directed(card, target.transform, target.GetStats);
    }
    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        Stat victim = targetStats.GetStat(buffedStat.ToString());
        if (victim == null) return;
        SoundPlayer.Play.Invoke(buffSound);
        victim.Value *= multiplier;
        victim.Value += value;
    }
}
