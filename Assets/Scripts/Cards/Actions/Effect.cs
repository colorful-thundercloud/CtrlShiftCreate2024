using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public override Stat GetStat(CardController card)
    {
        color = (buffedStat == BuffedStats.damage) ? Color.red : new Color(0, 0.5f, 0, 1);
        TurnBasedGameplay.OnEndTurn.AddListener(isEnemy => reloadSteps(card));/*

        Transform buff = card.transform.Find("buff");
        buff.gameObject.SetActive(true);
        buff.GetComponent<SpriteRenderer>().color = (buffedStat == BuffedStats.damage) ? Color.red : Color.green;*/

        Stat stat = new();
        stat.Name = "multiplier";
        stat.field = card.transform.Find("multiplier").GetComponentInChildren<TMP_Text>();
        stat.field.color = color;
        card.transform.Find("multiplier").GetComponentInChildren<SpriteRenderer>().color = color;
        stat.Value = multiplier;
        stat.maxValue = multiplier;
        stat.canBuff = false;
        return stat;
    }
    public Stat GetSecondStat(CardController card)
    {
        TurnBasedGameplay.OnEndTurn.AddListener(isEnemy => reloadSteps(card));
        Stat stat = new();
        stat.Name = "value";
        stat.field = card.transform.Find("value").GetComponentInChildren<TMP_Text>();
        stat.field.color = color;
        card.transform.Find("value").GetComponentInChildren<SpriteRenderer>().color = color;
        stat.Value = value;
        stat.maxValue = value;
        stat.canBuff = false;
        return stat;
    }
}
