using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class BasicCard : ScriptableObject
{
    [SerializeField] string title;
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
    public abstract void initialize(Card card);
    /// <summary>
    /// Проверка доступности способности
    /// </summary>
    public bool CheckAction() { return action.CheckAviability(); }
    /// <summary>
    /// Действия при выкладывании карты на поле
    /// </summary>
    public abstract bool cast();

    /// <summary>
    /// Активация действия выбранной карты на эту
    /// </summary>
    public virtual bool OnClick() { return false; }
    /// <summary>
    /// Сохранение данной карты при выборе
    /// </summary>
    public virtual void OnSelect() { }
    public virtual IHaveStats TryGetAttack() { return null; }
    public virtual Health TryGetHealth() { return null; }
}
