using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Effect : Action
{
    Color color;
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
    public BuffedStats GetBuffedStat => buffedStat;
    /// <summary>
    /// Сколько прибавляется к выбранному параметру
    /// </summary>
    [Header("На сколько изменяется выбранный параметр")]
    [SerializeField] int value = 0;
    [SerializeField] AudioClip buffSound;
    [SerializeField] bool multiply;

    public override void Undirected(CardController card)
    {
        List<CardController> targets = GetAllTargets(card);
        targets.Remove(card);
        foreach (CardController target in targets) Directed(card, target.transform, target.GetStats);
    }
    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        Stat victim = targetStats.GetStat(buffedStat.ToString());
        if (victim == null) return;
        SoundPlayer.Play.Invoke(buffSound);
        victim.Value = (multiply) ? victim.Value * value : victim.Value + value;
    }
    public override Stat GetStat(CardController card)
    {
        color = (buffedStat == BuffedStats.damage) ? Color.red : new Color(0, 0.5f, 0, 1);

        Stat stat = new();
        stat.Name = "value";
        stat.field = card.transform.Find("value").GetComponentInChildren<TMP_Text>();
        stat.field.color = color;
        stat.Value = value;
        stat.maxValue = value;
        stat.canBuff = false;
        return stat;
    }
}
