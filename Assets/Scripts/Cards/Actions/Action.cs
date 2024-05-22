using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Action
{
    /// <summary>
    /// ��������� �� ����������� �� ������� �����
    /// </summary>
    [Header("��������� �� ����������� �� ������� �����")]
    public bool toAllies = false;
    /// <summary>
    /// ������������ ���������� ��������� �� ���
    /// </summary>
    [Header("������������ ���������� ��������� �� ���")]
    public int maxSteps;

    protected int steps;
    public Card card { get; set; }

    public abstract void Initialize(Card card);
    /// <summary>
    /// ����������� ����� � �����������
    /// </summary>
    protected void reloadSteps() => steps = maxSteps;
    /// <summary>
    /// �������������� �� ����� �����������
    /// </summary>
    public virtual void Undirected() { }
    /// <summary>
    /// ������������ �� ������ ����� �����������, ������������ ������ ������ �� �����-����
    /// </summary>
    public virtual void Directed(Card card) 
    {

    }
    /// <summary>
    /// ��������� ����� �� ��������� ����� ����������������� � ����
    /// </summary>
    public bool CheckAlies(Card target)
    {
        bool match = card.CompareTag(target.tag);
        if (toAllies) return match;
        else return !match;
    }
    /// <summary>
    /// �������� ����������� �����������
    /// </summary>
    public abstract bool CheckAviability();
    /// <summary>
    /// �������� �� Field ��� ����� �� ������� ��������� Action
    /// </summary>
    protected List<Card> GetAllTargets()
    {
        if (toAllies) return Field.GetCards(card.CompareTag("enemyCard"));
        else return Field.GetCards(card.CompareTag("myCard"));
    }
    public virtual IHaveStats TryGetStats() { return null; }
}
