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
    /// ����������� ����� � �����������
    /// </summary>
    protected void reloadSteps()
    {
        if(Card.isCasted) steps = maxSteps;
    }
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
        bool match = Card.CompareTag(target.tag);
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
        if (toAllies) return Field.GetCards(Card.CompareTag("enemyCard"));
        else return Field.GetCards(Card.CompareTag("myCard"));
    }
    public virtual IHaveStats TryGetStats() { return null; }
}
