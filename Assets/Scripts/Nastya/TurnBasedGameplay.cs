using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnBasedGameplay : MonoBehaviour
{
    public Hand playerHand, enemyHand;
    public Button endMoveBtn;
    public static UnityEvent<bool> OnEndTurn = new();
    void Start()
    {
        Invoke("DrawCards", 0.5f);
        OnEndTurn.AddListener(onNextTurn);
    }

    void DrawCards()
    {
        playerHand.DrawCards();
        enemyHand.DrawCards(true);
        enemyEndMove();
    }

    public void NextTurn(bool isEnemyTurn) => OnEndTurn.Invoke(isEnemyTurn);
    void onNextTurn(bool isEnemyTurn)
    {
        endMoveBtn.interactable = !isEnemyTurn;
        Card.Selected = null;
    }

    public void playerEndMove()
    {
        /*// начало хода врага
        
        foreach (Card card in playerHand.GetCards())
            card.canDrag = false;

        if (Field.GetCards(true).Count > 0)
            foreach (Card card in Field.GetCards(true))
                card.used = false;

        if (Field.GetCards(false).Count > 0)
            foreach (Card card in Field.GetCards(false))
                card.used = true;
        Card.Selected = null;
        enemyHand.DrawCards(true);

        bb.EnemyTurn();*/
    }

    public void enemyEndMove()
    {
        // начало хода игрока
        endMoveBtn.interactable = true;
        playerHand.DrawCards();

        /*foreach (Card card in playerHand.GetCards())
            card.canDrag = true;

        if (Field.GetCards(false).Count > 0)
            foreach (Card card in Field.GetCards(false))
                card.used = false;*/

    }
}
