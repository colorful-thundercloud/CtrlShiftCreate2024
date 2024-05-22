using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Action
{
    /// <summary>
    /// ƒействует ли способность на союзные карты
    /// </summary>
    [Header("ƒействует ли способность на союзные карты")]
    public bool toAllies = false;
    /// <summary>
    /// ћаксимальное количество активаций за ход
    /// </summary>
    [Header("ћаксимальное количество активаций за ход")]
    public int maxSteps;

    protected int steps;
    public Card card { get; set; }

    public abstract void Initialize(Card card);
    /// <summary>
    /// ¬осстановка ходов у способности
    /// </summary>
    protected void reloadSteps() => steps = maxSteps;
    /// <summary>
    /// Ќенаправленна€ на карту способность
    /// </summary>
    public virtual void Undirected() { }
    /// <summary>
    /// Ќаправленна€ на другую карту способность, активируетс€ вторым кликом по карте-цели
    /// </summary>
    public virtual void Directed(Card card) 
    {

    }
    /// <summary>
    /// ѕровер€ет может ли выбранна€ карта взаимодействовать с этой
    /// </summary>
    public bool CheckAlies(Card target)
    {
        bool match = card.CompareTag(target.tag);
        if (toAllies) return match;
        else return !match;
    }
    /// <summary>
    /// ѕроверка доступности способности
    /// </summary>
    public abstract bool CheckAviability();
    /// <summary>
    /// ѕолучить из Field все карты на которые действует Action
    /// </summary>
    protected List<Card> GetAllTargets()
    {
        if (toAllies) return Field.GetCards(card.CompareTag("enemyCard"));
        else return Field.GetCards(card.CompareTag("myCard"));
    }
    public virtual IHaveStats TryGetStats() { return null; }
}
