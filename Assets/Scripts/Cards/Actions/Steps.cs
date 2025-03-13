using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Steps
{
    /// <summary>
    /// Максимальное количество активаций за ход
    /// </summary>
    [Header("Максимальное количество активаций за ход")]
    [SerializeField] int maxSteps;

    /// <summary>
    /// Восстановка ходов у способности
    /// </summary>
    public void reloadSteps(CardController card, bool myTurn)
    {
        if (card == null) return;
        if (myTurn != card?.CompareTag("enemyCard"))
            if (card.isCasted) card.GetStat("steps").Value = maxSteps;
    }
    public Stat GetStat(CardController card)
    {
        Stat stat = new();
        stat.Name = "steps";
        stat.field = card.transform.Find("steps").GetComponentInChildren<TMP_Text>();
        stat.Value = 0;
        stat.maxValue = maxSteps;
        return stat;
    }
}
