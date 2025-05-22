using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Action
{
    /// <summary>
    /// Действует ли способность на союзные карты
    /// </summary>
    [Header("Действует ли способность на союзные карты")]
    public bool toAllies = false;

    public bool directed = true;
    /// <summary>
    /// Ненаправленная на карту способность
    /// </summary>
    public virtual void Undirected(CardController card)
    {
        if (card.GetStat("steps") != null) card.GetStat("steps").Value--;
    }
    /// <summary>
    /// Направленная на другую карту способность, активируется вторым кликом по карте-цели
    /// </summary>
    public virtual void Directed(CardController card, Transform targetTransform, CardStats targetStats)
    {
        if (card.GetStat("steps")!=null) card.GetStat("steps").Value--;
    }
    /// <summary>
    /// Проверяет может ли выбранная карта взаимодействовать с этой
    /// </summary>
    public bool CheckAlies(CardController card, CardController target)
    {
        bool match = card.CompareTag(target.tag);
        if (toAllies) return match;
        else return !match;
    }
    /// <summary>
    /// Проверка доступности способности
    /// </summary>
    public virtual bool CheckAviability(CardController card)
    {
        bool aviable = true;
        if (card.GetStat("Blocked") != null) aviable = card.GetStat("Blocked").Value == 0;
        Stat steps = card.GetStat("steps");
        if (steps != null && steps.Value <= 0) aviable = false;
        return aviable;
    }
    /// <summary>
    /// Получить из Field все карты на которые действует Action
    /// </summary>
    public List<CardController> GetAllTargets(CardController card)
    {
        if (toAllies) return GameManager.GetCards(card.CompareTag("enemyCard"));
        else return GameManager.GetCards(card.CompareTag("myCard"));
    }
    public virtual Stat GetStat(CardController card) { return null; }
}
