using UnityEngine;
using UnityEngine.UI;

public class TurnBasedGameplay : MonoBehaviour
{
    public bool playerTurn = true;
    public BigBrain bb;
    public Hand playerHand, enemyHand;
    public Button endMoveBtn;
    public Field field;
    void Start()
    {
        Invoke("DrawCards", 0.5f);
    }

    void DrawCards()
    {
        playerHand.DrawCards();
        enemyHand.DrawCards(true);
        enemyEndMove();
    }    

    public void playerEndMove()
    {
        // начало хода врага
        endMoveBtn.interactable = false;
        foreach (Card card in playerHand.GetCards())
            card.canDrag = false;

        if (field.GetCards(true).Count > 0)
            foreach (Card card in field.GetCards(true))
                card.used = false;

        if (field.GetCards(false).Count > 0)
            foreach (Card card in field.GetCards(false))
                card.used = true;

        Field.SelectedCard?.turnOfLight();
        Field.SelectedCard = null;
        enemyHand.DrawCards(true);
        
        bb.EnemyTurn();
    }

    public void enemyEndMove()
    {
        // начало хода игрока
        endMoveBtn.interactable = true;
        playerHand.DrawCards();

        foreach (Card card in playerHand.GetCards())
            card.canDrag = true;

        if (GetComponent<Field>().GetCards(false).Count > 0)
            foreach (Card card in field.GetCards(false))
                card.used = false;

    }
}
