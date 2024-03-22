using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicCard: ScriptableObject
{
    [SerializeField] Sprite Avatar;
    public Sprite GetAvatar { get { return Avatar; } }
    [SerializeField] private int hp;
    public int HP { get { return hp; } }
    [SerializeField] private int damage;
    [SerializeField] int cardCount;
    public int GetCardCount { get { return cardCount; } }
    public void changeHP(int value)
    {
        hp = value;
    }
    public virtual void cast()
    {

    }
    public 
}
