using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Health:IHaveStats
{
    /// <summary>
    /// Стартовое значение здоровья карты
    /// </summary>
    [Header("Стартовое значение здоровья карты")]
    [SerializeField] int MaxHP;
    [SerializeField] AudioClip DeathSound;
    TMP_Text ui;
    int currentHP;
    public Card card { get; set; }
    Animator anim;
    public void Initialize(Card card, Animator anim)
    {
        this.card = card;
        currentHP = MaxHP;
        ui = card.health;
        ui.text = currentHP.ToString();
        this.anim = anim;
    }

    public int parameter
    {
        get { return currentHP; }
        set
        {
            currentHP = value;
            ui.text = currentHP.ToString();
            if (currentHP <= 0) card.StartCoroutine(death());
        }
    }

    IEnumerator death()
    {
        //play animation
        anim.SetTrigger("deathTrigger");
        SoundPlayer.Play(DeathSound);
        yield return new WaitForSeconds(1f);
        Field.OnCardBeat.Invoke(card);
    }
}
