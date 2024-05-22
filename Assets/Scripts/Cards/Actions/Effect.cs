using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Effect : Action
{
    public enum Type
    {
        Damage,
        Health
    }
    /// <summary>
    /// Определяет баффаемый параметр
    /// </summary>
    [Header("Определяет баффаемый параметр")]
    [SerializeField] Type type;
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
    public override bool CheckAviability()
    {
        return steps > 0;
    }

    public override void Initialize(Card card)
    {
        this.card = card;
    }

    public override void Undirected()
    {
        List<Card> targets = GetAllTargets();
        foreach (Card card in targets) Directed(card);
    }
    public override void Directed(Card target)
    {
        IHaveStats victim = (type==Type.Damage)? target.GetBasicCard.TryGetAttack():target.GetBasicCard.TryGetHealth();
        if (victim == null) return;
        SoundPlayer.Play(buffSound);
        victim.parameter *= multiplier;
        victim.parameter += value;
    }
}
