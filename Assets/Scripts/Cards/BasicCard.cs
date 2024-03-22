using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BasicCard : ScriptableObject
{
    [SerializeField] Sprite Avatar;
    public Sprite GetAvatar { get { return Avatar; } }
    [SerializeField] cardType type;
    public cardType Type { get { return type; } }
    [SerializeField] private int hp;
    public int HP { get { return hp; } }
    [SerializeField] private int damage;
    public int Damage { get { return damage; } }
    [SerializeField] int cardCount;
    public int GetCardCount { get { return cardCount; } }
    [SerializeField][TextArea] string description;
    public string Description { get { return description; } }
    public void changeHP(int value)
    {
        hp = value;
    }
    public virtual void cast()
    {

    }
    public abstract void OnClick();

    protected int steps = 0;
    public virtual bool CheckSteps()
    {
        return steps != 0;
    }
    public enum cardType { Unit, Buff }
}
