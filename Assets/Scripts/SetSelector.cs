using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetSelector : MonoBehaviour
{
    [SerializeField] GameObject standartMenu;
    [SerializeField] DeckController playerDeck;
    [SerializeField] Hand playerHand;
    public static UnityEvent<CardSet> OnSelectSet = new();
    private void Start()
    {
        OnSelectSet.AddListener(SelectSet);
    }
    void SelectSet(CardSet set)
    {
        standartMenu.SetActive(true);
        playerDeck.SetSet(set);
        playerHand.DrawCards();
        gameObject.SetActive(false);
        LevelController.onNextLevel.Invoke();
    }
}
