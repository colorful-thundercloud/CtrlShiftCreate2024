using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSet", menuName = "CardSet", order = 0)]
public class CardSet : ScriptableObject
{
    [System.Serializable]
    enum cardType { unit, buff };

    [System.Serializable]
    public class Card {
        [SerializeField]
        public BasicCard card;
        [SerializeField]
        cardType cardType;
        [SerializeField]
        GameObject cardPrefab;
        [SerializeField]
        public int amountInDeck;
    }

    
    [SerializeField]
    public List<Card> cards;

}