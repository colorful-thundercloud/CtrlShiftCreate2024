using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[Serializable]
public class Attack: Action, IHaveStat
{
    /// <summary>
    /// Наносимый урон
    /// </summary>
    [Header("Наносимый урон")]
    [SerializeField] int damage;
    [SerializeField] AudioClip AttackSound;
    public override bool CheckAviability(CardController card)
    {
        bool aviable = true;
        if (card.GetStat("Blocked") != null) aviable = card.GetStat("Blocked").Value == 0;
        if (card.GetStat("steps").Value == 0) aviable = false;
        return aviable;
    }

    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        card.GetStat("steps").Value--;
        card.StartCoroutine(attackAnimation(0.2f, card, targetTransform.transform, targetStats.GetStat(Effect.BuffedStats.hp.ToString())));
        CardController.Selected = null;
        SoundPlayer.Play.Invoke(AttackSound);
    }

    IEnumerator attackAnimation(float smoothTime, CardController card, Transform target, Stat hp)
    {
        Vector2 startPosition = card.transform.position;
        Vector2 direction = target.position;

        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, 3);

        yield return card.StartCoroutine(Mover.MoveCard(card, direction, smoothTime));

        hp.Value -= card.GetStat("damage").Value;

        yield return card.StartCoroutine(Mover.MoveCard(card, startPosition, smoothTime));

        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, 4);
    }

    public override Stat GetStat(CardController card)
    {
        Stat stat = new();
        stat.Name = Effect.BuffedStats.damage.ToString();
        stat.field = card.transform.Find("attack").GetComponentInChildren<TMP_Text>();
        stat.Value = damage;
        stat.maxValue = damage;
        stat.canBuff = true;
        return stat;
    }
}
