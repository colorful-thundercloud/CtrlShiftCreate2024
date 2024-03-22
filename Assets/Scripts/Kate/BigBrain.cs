using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBrain : MonoBehaviour
{
    [SerializeField] GameObject Field;
    [SerializeField] GameObject Hand;
    List<Card> myCards = new List<Card>();
    List<Card> myCardsOnBoard = new List<Card>();
    List<Card> playerCards = new List<Card>();

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            EnemyTurn();
        }
    }

    void EnemyTurn()
    {
        myCardsOnBoard = Field.GetComponent<Field>().GetCards(true);
        playerCards = Field.GetComponent<Field>().GetCards(false);
        myCards = Hand.GetComponent<Hand>().GetCards();
        //Hand.DrawCards(true); //получение карт в руку
        Debug.Log(playerCards[0].GetBasicCard.GetType());
        if (myCardsOnBoard.Count == 0)
        {
            DoUnit(3);
        }
        //если на поле все свободно, запустить растановку
        //если есть баффы применить их
        //если на поле есть свободное место и в руке есть юниты запустить скрипт постановки
    }

    void DoUnit(int howMuch)//скрипт постановки
    {
        //оценка юнитов игрока
        //оценка своих юнитов
        //сравнение
        //оценка юнитов в руке
        //если сильно в пользу нас, и наши карты слабы ничего не делаем
        //если все же мы слабее/равны, то ставим карту
    }

    void WhichCardSpawn()//если несколько карт для спавна
    {
        //мяу
    }

    void DoBaff()
    {
        //применяем баф
    }
}
