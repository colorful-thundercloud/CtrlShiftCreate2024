using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSet", menuName = "CardSet", order = 0)]
public class CardSet : ScriptableObject
{   
    [SerializeField]
    public List<BasicCard> cards;

}