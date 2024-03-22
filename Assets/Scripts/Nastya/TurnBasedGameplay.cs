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
        enemyHand.DrawCards(true);
    }    

    public void playerEndMove()
    {
        // начало хода врага
        endMoveBtn.interactable = false;
        foreach (Card card in playerHand.GetCards())
            card.canDrag = false;

        enemyHand.DrawCards();

        // временная фигня начинается тут
        if (enemyHand.GetCards().Count > 0)
        {
            if (!temp)
            {
                enemyHand.GetCards()[0].transform.position += new Vector3(-2, 1, 0);
                temp = true;
            }
            else
            {
                enemyHand.GetCards()[0].transform.position -= new Vector3(-2, 1, 0);
                temp = false;
            }
        }
        // временная фигня кончается здесь

        Invoke("enemyEndMove", 2f);
    }

    public void enemyEndMove()
    {
        // начало хода игрока
        endMoveBtn.interactable = true;  
        foreach (Card card in playerHand.GetCards())
            card.canDrag = true;

        playerHand.DrawCards();
    }
}
