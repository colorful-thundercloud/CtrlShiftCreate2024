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
    /// �������������� ��� ���������� �����
    /// ����������� ��� ��������� �����
    /// </summary>
    public abstract void initialize(Card card);
    /// <summary>
    /// �������� ����������� �����������
    /// </summary>
    public bool CheckAction() { return action.CheckAviability(); }
    /// <summary>
    /// �������� ��� ������������ ����� �� ����
    /// </summary>
    public abstract bool cast();

    /// <summary>
    /// ��������� �������� ��������� ����� �� ���
    /// </summary>
    public virtual bool OnClick() { return false; }
    /// <summary>
    /// ���������� ������ ����� ��� ������
    /// </summary>
    public virtual void OnSelect() { }
    public virtual IHaveStats TryGetAttack() { return null; }
    public virtual Health TryGetHealth() { return null; }
}
