using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnBasedGameplay : MonoBehaviour
{
    public Hand playerHand, enemyHand;
    public Button endMoveBtn;
    public static UnityEvent<bool> OnEndTurn = new();
    public static bool myTurn = true;
    void Awake()
    {
        //Invoke("DrawCards", 0.5f);
        OnEndTurn.AddListener(onNextTurn);
    }

    void DrawCards()
    {
        endMoveBtn.interactable = true;
    }

    public void NextTurn(bool isEnemyTurn) => OnEndTurn.Invoke(isEnemyTurn);
    void onNextTurn(bool isEnemyTurn)
    {
        endMoveBtn.interactable = !isEnemyTurn;
        myTurn = !isEnemyTurn;
        CardController.Selected = null;
        if(isEnemyTurn) enemyHand.DrawCards(true);
        else playerHand.DrawCards();
    }
}
