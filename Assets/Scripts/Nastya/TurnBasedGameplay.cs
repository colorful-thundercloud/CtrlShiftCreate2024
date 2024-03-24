using UnityEngine;
using UnityEngine.UI;

public class TurnBasedGameplay : MonoBehaviour
{
    public bool playerTurn = true;
    public BigBrain bb;
    public Hand playerHand, enemyHand;
    public Button endMoveBtn;
    public Field field;
    bool temp;
    void Start()
    {
        Invoke("DrawCards", 0.5f);
    }

    void DrawCards()
    {
        playerHand.DrawCards();
        enemyEndMove();
    }    

    public void playerEndMove()
    {
        // начало хода врага
        endMoveBtn.interactable = false;
        foreach (Card card in playerHand.GetCards())
            card.canDrag = false;

        foreach (Card card in field.GetCards(true))
            card.used = false;

        playerHand.DrawCards();
        bb.EnemyTurn();
    }

    public void enemyEndMove()
    {
        // начало хода игрока
        endMoveBtn.interactable = true;  
        foreach (Card card in playerHand.GetCards())
            card.canDrag = true;

        foreach (Card card in field.GetCards(false))
            card.used = false;

        enemyHand.DrawCards(true);
    }
}
