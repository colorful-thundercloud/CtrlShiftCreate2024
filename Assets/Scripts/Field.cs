using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] Transform myField, enemyField;
    [SerializeField] float distance;
    List<GameObject> myCards = new List<GameObject>();
    List<GameObject> enemyCards = new List<GameObject>();
    public static Action<Card> OnCast;
    public static Action<Card> OnEnemyCast;
    private void Start()
    {
        OnCast += ctx => addCard(ctx, false);
        OnEnemyCast += ctx => addCard(ctx, true);
    }
    void addCard(Card card, bool isEnemy)
    {
        List<GameObject> field = (isEnemy) ? enemyCards : myCards;
        field.Add(card.gameObject);
        for(int i = 0; i < field.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.y = (isEnemy) ? enemyField.position.y : myField.position.y;
            pos.x = distance * i;
            field[i].transform.position = pos;
        }
        float center = field[field.Count - 1].transform.position.x / 2;
        Debug.Log(center);
        foreach (GameObject item in field) item.transform.Translate(-center, 0, 0);
        if (isEnemy) enemyCards = field;
        else myCards = field;
    }
    public List<BasicCard> GetCards(bool isEnemy)
    {
        List<BasicCard> cards;
        if(isEnemy) cards = enemyCards.ConvertAll(n => n.GetComponent<BasicCard>());
        else cards = myCards.ConvertAll(n => n.GetComponent<BasicCard>());
        return cards;
    }
}
