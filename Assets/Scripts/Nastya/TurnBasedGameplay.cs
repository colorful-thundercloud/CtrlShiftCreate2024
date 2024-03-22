using UnityEngine;

public class TurnBasedGameplay : MonoBehaviour
{
    public bool playerTurn = true;
    public Hand playerHand, enemyHand;
    public Field field;
    void Start()
    {
        playerHand.Invoke("DrawCards", 0.5f);
        enemyHand.Invoke("DrawCards", 0.5f);
    }

    void Update()
    {

    }

    public void playerEndMove()
    {
        playerHand.enabled = false;

    }

    public void enemyEndMove()
    {
        playerHand.enabled = true;
    }
}
