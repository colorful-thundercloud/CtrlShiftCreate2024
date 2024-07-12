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
    /// <summary>
    /// Максимальное количество активаций за ход
    /// </summary>
    [Header("Максимальное количество активаций за ход")]
    public int maxSteps;

    /// <summary>
    /// Восстановка ходов у способности
    /// </summary>
    protected void reloadSteps(CardController card)
    {
        if(card.isCasted) card.GetStat("steps").Value = maxSteps;
    }
    /// <summary>
    /// Ненаправленная на карту способность
    /// </summary>
    public virtual void Undirected(CardController card) { }
    /// <summary>
    /// Направленная на другую карту способность, активируется вторым кликом по карте-цели
    /// </summary>
    public virtual void Directed(CardController card, CardController target) 
    {

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
    public abstract bool CheckAviability(CardController card);
    /// <summary>
    /// Получить из Field все карты на которые действует Action
    /// </summary>
    protected List<CardController> GetAllTargets(CardController card)
    {
        if (toAllies) return Field.GetCards(card.CompareTag("enemyCard"));
        else return Field.GetCards(card.CompareTag("myCard"));
    }
    public virtual Stat GetStat(CardController card) { return null; }
}
