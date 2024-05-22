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
    /// ���������� ��������� ��������
    /// </summary>
    [Header("���������� ��������� ��������")]
    [SerializeField] Type type;
    /// <summary>
    /// �� ������� ���������� ��������� ��������
    /// </summary>
    [Header("�� ������� ���������� ��������� ��������")]
    [SerializeField] int multiplier = 1;
    /// <summary>
    /// ������� ������������ � ���������� ���������
    /// </summary>
    [Header("������� ������������ � ���������� ���������")]
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
