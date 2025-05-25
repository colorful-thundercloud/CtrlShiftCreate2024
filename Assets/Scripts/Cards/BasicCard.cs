using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Localization;

public abstract class BasicCard : ScriptableObject
{
    [SerializeField] string title;
    [SerializeField] LocalizedString localizedTitle;
    public bool isIngoringFieldCapacity { get; protected set; }
    public string Title { get { return title; } }
    [SerializeField] Sprite Avatar;
    public Sprite GetAvatar { get { return Avatar; } }
    [SerializeField][TextArea] string description;
    public string Description { get { return description; } }
    protected Action action;
    /// <summary>
    /// Инициализирует все компоненты карты
    /// Срабатывает при появлении карты
    /// </summary>
    public abstract void initialize(CardController card);
    /// <summary>
    /// Проверка доступности способности
    /// </summary>
    public bool CheckAction(CardController card) { return action.CheckAviability(card); }
    public Action GetAction() { return action; }
    /// <summary>
    /// Действия при выкладывании карты на поле
    /// </summary>
    public abstract bool cast(CardController card);

    /// <summary>
    /// Активация действия выбранной карты на эту
    /// </summary>
    public virtual bool OnClick(CardController card) { return false; }
    /// <summary>
    /// Сохранение данной карты при выборе
    /// </summary>
    public virtual void OnSelect(CardController card) { }
    public abstract List<Stat> GetBasicStats(CardController card);
}
