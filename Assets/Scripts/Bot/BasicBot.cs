using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Бот")]
public class BasicBot : ScriptableObject
{
    public Sprite icon;
    public Health hp;
    public CardSet cardSet;
    public AudioClip music;
}
