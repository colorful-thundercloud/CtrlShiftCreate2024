using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBrain : MonoBehaviour
{
    [SerializeField] GameObject Field;
    [SerializeField] GameObject Hand;
    List<BasicCard> myCards = new List<BasicCard>();
    List<BasicCard> playerCards = new List<BasicCard>();

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            EnemyTurn();
        }
    }

    void EnemyTurn()
    {
        myCards = Field.GetComponent<Field>().GetCards(true);
        playerCards = Field.GetComponent<Field>().GetCards(false);
        Debug.Log("Enemy's Cards:");
        foreach (BasicCard card in myCards)
        {
            Debug.Log("HP: " + card.HP + ", Damage: " + card.Damage);
        }
        //получение карт в руку
        //если на поле все свободно, запустить растановку
        //если есть баффы применить их
        //если на поле есть свободное место и в руке есть юниты запустить скрипт постановки
    }

    void DoIUnit()//скрипт постановки
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
