using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class SetSelector : NetworkBehaviour
{
    [SerializeField] Animator moneyAnim;
    [SerializeField] AudioClip moneySound;
    [SerializeField] GameObject standartMenu;
    [SerializeField] DeckController playerDeck;
    [SerializeField] Hand playerHand;
    [SerializeField] TMP_Text turnText;
    public static UnityEvent<CardSet> OnSelectSet = new();
    private void Start()
    {
        //OnSelectSet.AddListener(startSelect);
    }
    private void startSelect(CardSet set)
    {
        Time.timeScale = 1;
        standartMenu.SetActive(true);
        playerDeck.SetSet(set);
        //playerHand.DrawCards();
        gameObject.SetActive(false);
        LevelController.onNextLevel.Invoke();

        if (IsServer)
        {
            bool turn = Random.Range(0, 2) == 0;
            turnClientRpc(!turn);
            StartCoroutine(setTurn(turn));
        }
    }
    [ClientRpc]
    private void turnClientRpc(bool turn)
    {
        StartCoroutine(setTurn(turn));
    }
    private IEnumerator setTurn(bool turn)
    {
        turnText.text = (turn) ? "Ход противника" : "Твой ход";
        SoundPlayer.Play.Invoke(moneySound);
        moneyAnim.SetTrigger((!turn) ? "MyLot" : "EnemyLot");
        yield return new WaitForSeconds(3f);
        GameManager.OnEndTurn.Invoke(turn);
    }
}
