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

    // Update is called once per frame
    void Update()
    {
        
    }
}
