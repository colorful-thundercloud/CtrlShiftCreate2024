using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Health
{
    /// <summary>
    /// Стартовое значение здоровья карты
    /// </summary>
    [Header("Стартовое значение здоровья карты")]
    [SerializeField] int MaxHP;
    [SerializeField] AudioClip DeathSound;
    [NonSerialized] public UnityEvent OnDeath = new();

    public Stat GetStat(MonoBehaviour mono, CardController card = null)
    {
        void OnHealthChange(int newValue)
        {
            if (newValue <= 0)
                mono.StartCoroutine(death(mono.GetComponent<Animator>(), card));
        }
        Stat stat = new();
        stat.Name = Effect.BuffedStats.hp.ToString();
        stat.field = mono.transform.Find("hp").GetComponentInChildren<TMP_Text>();
        stat.Value = MaxHP;
        stat.maxValue = MaxHP;
        stat.canBuff = true;
        stat.OnChange.AddListener(OnHealthChange);
        return stat;
    }

    IEnumerator death(Animator anim, CardController card)
    {
        if (anim != null) anim?.SetTrigger("deathTrigger");
        SoundPlayer.Play.Invoke(DeathSound);
        yield return new WaitForSeconds(1f);
        if (card != null) GameManager.OnCardBeat.Invoke(card);
        OnDeath.Invoke();
    }
    public void Heal(CardStats stats)
    {
        Stat hp = stats.GetStat(Effect.BuffedStats.hp.ToString());
        hp.Value = hp.maxValue;
    }
}
