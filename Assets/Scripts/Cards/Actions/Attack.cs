using System;
using System.Collections;
using TMPro;
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
    public override void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        base.Directed(card, targetTransform, targetStats);
        Debug.Log(targetTransform.parent.name);
        card.StartCoroutine(attackAnimation(0.2f, card, 
            (targetTransform.parent.name == "Canvas") ? Camera.main.ScreenToWorldPoint(targetTransform.position) : targetTransform.position, 
            targetStats.GetStat("hp")));
        SoundPlayer.Play.Invoke(AttackSound);
    }

    IEnumerator attackAnimation(float smoothTime, CardController card, Vector2 direction, Stat hp)
    {
        Vector3 startPosition = card.transform.position;

        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, 3);

        yield return card.StartCoroutine(Mover.MoveCard(card.transform, direction, smoothTime));

        hp.Value -= card.GetStat("damage").Value;

        yield return card.StartCoroutine(Mover.MoveCard(card.transform, startPosition, smoothTime));

        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, startPosition.z);
    }

    public override Stat GetStat(CardController card)
    {
        Stat stat = new();
        stat.Name = "damage";
        stat.field = card.transform.Find("attack").GetComponentInChildren<TMP_Text>();
        stat.Value = damage;
        stat.maxValue = damage;
        stat.canBuff = true;
        return stat;
    }
}
