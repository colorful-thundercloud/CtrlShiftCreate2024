using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

[Serializable]
public class Health: IHaveStat
{
    /// <summary>
    /// Стартовое значение здоровья карты
    /// </summary>
    [Header("Стартовое значение здоровья карты")]
    [SerializeField] int MaxHP;
    [SerializeField] AudioClip DeathSound;

    public Stat GetStat(CardController card)
    {
        void OnHealthChange()
        {
            if (card.GetStat("hp").Value <= 0)
                card.StartCoroutine(death(card, card.GetComponent<Animator>()));
        }
        Stat stat = new();
        stat.Name = Effect.BuffedStats.hp.ToString();
        stat.field = card.transform.Find("hp").GetComponentInChildren<TMP_Text>();
        stat.Value = MaxHP;
        stat.maxValue = MaxHP;
        stat.canBuff = true;
        stat.OnChange.AddListener(OnHealthChange);
        return stat;
    }

    IEnumerator death(CardController card, Animator anim)
    {
        //play animation
        anim.SetTrigger("deathTrigger");
        SoundPlayer.Play.Invoke(DeathSound);
        yield return new WaitForSeconds(1f);
        Field.OnCardBeat.Invoke(card);
    }
}
