using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
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
        return card.GetStat("steps").Value > 0;
    }

    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        card.GetStat("steps").Value--;
        card.StartCoroutine(attackAnimation(0.5f, card, targetTransform.transform, targetStats.GetStat("hp")));
        CardController.Selected = null;
        SoundPlayer.Play.Invoke(AttackSound);
    }

    IEnumerator attackAnimation(float smoothTime, CardController card, Transform target, Stat hp)
    {
        Vector3 startPosition = card.transform.position;
        Vector3 direction = target.position;

        card.GetComponent<SpriteRenderer>().sortingOrder++;
        foreach (Transform t in card.transform)
            if (t.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer)) 
                spriteRenderer.sortingOrder++;

        while (card.transform.position != direction)
        {
            card.transform.position = Vector3.MoveTowards(card.transform.position, direction, smoothTime);
            yield return new WaitForFixedUpdate();
        }

        hp.Value -= card.GetStat("damage").Value;

        direction = startPosition;
        while (card.transform.position != direction)
        {
            card.transform.position = Vector3.MoveTowards(card.transform.position, startPosition, smoothTime);
            yield return new WaitForFixedUpdate();
        }

        card.GetComponent<SpriteRenderer>().sortingOrder--;
        foreach (Transform t in card.transform)
            if (t.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer)) 
                spriteRenderer.sortingOrder--;
    }

    public override Stat GetStat(CardController card)
    {
        TurnBasedGameplay.OnEndTurn.AddListener(isEnemy => reloadSteps(card));
        Stat stat = new();
        stat.Name = Effect.BuffedStats.damage.ToString();
        stat.field = card.transform.Find("attack").GetComponentInChildren<TMP_Text>();
        stat.Value = damage;
        stat.maxValue = damage;
        stat.canBuff = true;
        return stat;
    }
}
