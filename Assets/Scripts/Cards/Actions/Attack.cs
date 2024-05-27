using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Attack: Action, IHaveStats
{
    /// <summary>
    /// Наносимый урон
    /// </summary>
    [Header("Наносимый урон")]
    [SerializeField] int damage;
    [SerializeField] AudioClip AttackSound;
    TMP_Text ui;
    int currentDamage;
    public override void Initialize(Card card)
    {
        this.card = card;
        TurnBasedGameplay.OnEndTurn.AddListener(isEnemy=>reloadSteps());
        currentDamage = damage;
        ui = card.damage;
        ui.text = currentDamage.ToString();
    }

    public int parameter 
    {
        get { return currentDamage; } 
        set
        {
            currentDamage = value;
            ui.text = currentDamage.ToString();
        }
    }

    public override bool CheckAviability()
    {
        return steps > 0;
    }

    public override void Directed(Card target)
    {
        steps--;
        this.card.StartCoroutine(attackAnimation(0.5f, target.GetBasicCard.TryGetHealth(), target));
        Card.Selected = null;
        SoundPlayer.Play(AttackSound);
    }

    IEnumerator attackAnimation(float smoothTime, IHaveStats health, Card toAttack)
    {
        Vector3 startPosition = card.transform.position;
        Vector3 target = toAttack.transform.position;
        card.GetComponent<SpriteRenderer>().sortingOrder++;
        foreach (Transform t in card.transform)
            if (t.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer)) spriteRenderer.sortingOrder++;
        while (card.transform.position != target)
        {
            card.transform.position = Vector3.MoveTowards(card.transform.position, target, smoothTime);
            yield return new WaitForFixedUpdate();
        }
        health.parameter = health.parameter - currentDamage;
        target = startPosition;
        while (card.transform.position != target)
        {
            card.transform.position = Vector3.MoveTowards(card.transform.position, startPosition, smoothTime);
            yield return new WaitForFixedUpdate();
        }
        card.GetComponent<SpriteRenderer>().sortingOrder--;
        foreach (Transform t in card.transform)
            if (t.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer)) spriteRenderer.sortingOrder--;
    }
    public override IHaveStats TryGetStats()
    {
        return this;
    }
}
