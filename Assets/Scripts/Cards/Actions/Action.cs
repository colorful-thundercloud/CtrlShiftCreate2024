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
    /// Восстановка ходов у способности
    /// </summary>
    protected void reloadSteps()
    {
        if(Card.isCasted) steps = maxSteps;
    }
    /// <summary>
    /// Ненаправленная на карту способность
    /// </summary>
    public virtual void Undirected() { }
    /// <summary>
    /// Направленная на другую карту способность, активируется вторым кликом по карте-цели
    /// </summary>
    public virtual void Directed(Card card) 
    {

    }
    /// <summary>
    /// Проверяет может ли выбранная карта взаимодействовать с этой
    /// </summary>
    public bool CheckAlies(Card target)
    {
        bool match = Card.CompareTag(target.tag);
        if (toAllies) return match;
        else return !match;
    }
    /// <summary>
    /// Проверка доступности способности
    /// </summary>
    public abstract bool CheckAviability();
    /// <summary>
    /// Получить из Field все карты на которые действует Action
    /// </summary>
    protected List<Card> GetAllTargets()
    {
        if (toAllies) return Field.GetCards(Card.CompareTag("enemyCard"));
        else return Field.GetCards(Card.CompareTag("myCard"));
    }
    public virtual IHaveStats TryGetStats() { return null; }
}
