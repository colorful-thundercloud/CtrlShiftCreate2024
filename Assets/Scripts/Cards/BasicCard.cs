using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicCard: ScriptableObject
{
    public int hp;
    Animator anim;
    public void changeHP(int value)
    {
        hp = value;
    }
    public virtual void cast()
    {

    }
}
