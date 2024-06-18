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

    public int steps { get; set; }
    Card card;
    public Card Card 
    {
        get { return card; }
        set
        {
            card = value;
            Initialize();
        }
    }

    protected abstract void Initialize();
    /// <summary>
    /// ¬осстановка ходов у способности
    /// </summary>
    protected void reloadSteps()
    {
        if(Card.isCasted) steps = maxSteps;
    }
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
        bool match = Card.CompareTag(target.tag);
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
        if (toAllies) return Field.GetCards(Card.CompareTag("enemyCard"));
        else return Field.GetCards(Card.CompareTag("myCard"));
    }
    public virtual IHaveStats TryGetStats() { return null; }
}
