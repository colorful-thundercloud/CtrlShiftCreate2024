using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBrain : MonoBehaviour
{
    [SerializeField] GameObject Field;
    [SerializeField] GameObject Hand;
    List<Card> myCards = new();
    List<Card> myCardsOnBoard = new();
    List<Card> playerCards = new();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnemyTurn();
        }
    }
    public void EnemyTurn()
    {
        myCardsOnBoard = Field.GetComponent<Field>().GetCards(true);//карты босса на столе
        List<Card> StartBoard = myCardsOnBoard;
        playerCards = Field.GetComponent<Field>().GetCards(false);//карты игрока на столе
        Hand.GetComponent<Hand>().DrawCards(); //получение карт в руку
        myCards = Hand.GetComponent<Hand>().GetCards();//карты в руке
        SpawnUnit(WhichCardsSpawnUnit(), 3 - myCardsOnBoard.Count);//спавним юнитов в свободное место
        myCardsOnBoard = Field.GetComponent<Field>().GetCards(true);
        DoBaff(WhichCardsSpawnBaff());
        Attack(StartBoard);
        if (myCardsOnBoard.Count != 0 && myCardsOnBoard.Count < myCards.Count) StashCard();
        //конец хода
    }
    List<Card> WhichCardsSpawnUnit()//
    {
        List<Card> Spawn = new();
        foreach (Card card in myCards)
        {
            if (card.GetBasicCard.Type == BasicCard.cardType.Unit) Spawn.Add(card);
        }
        //добавить сортировку по урону или по хп
        if (StrengthInHealth()) myCards.Sort((x, y) => x.GetBasicCard.HP.CompareTo(y.GetBasicCard.HP));
        else myCards.Sort((x, y) => x.GetBasicCard.Damage.CompareTo(y.GetBasicCard.Damage));
        return Spawn;
    }
    bool StrengthInHealth()
    {
        bool healthMatter;
        int PlHP = 0, PlDM = 0;
        int HP = 0, DM = 0;
        foreach (Card card in playerCards)
        {
            PlHP += card.GetBasicCard.HP;
            PlDM += card.GetBasicCard.Damage;
        }
        foreach (Card card in myCardsOnBoard)
        {
            HP += card.GetBasicCard.HP;
            DM += card.GetBasicCard.Damage;
        }
        if (PlDM >= HP || PlHP < DM) healthMatter = true;
        else healthMatter = false;
        return healthMatter;
    }
    void SpawnUnit(List<Card> cardsToSpawn, int HowMuch)
    {
        if (cardsToSpawn.Count != 0)
        {
            for (int i = myCards.Count - 1; i >= 0; i--)
            {
                if (HowMuch == 0) break;
                for (int j = 0; j < cardsToSpawn.Count; j++)
                {
                    if (myCards[i] == cardsToSpawn[j])
                    {
                        myCards[i].EnemyCast();
                        myCards[i].OnMouseUp();
                        cardsToSpawn.Remove(cardsToSpawn[j]);
                        HowMuch--;
                        break;
                    }
                }
            }
        }
    }
    List<Card> WhichCardsSpawnBaff()//
    {
        List<Card> Spawn = new();
        foreach (Card card in myCards)
        {
            if (card.GetBasicCard.Type == BasicCard.cardType.Buff) Spawn.Add(card);
        }
        return Spawn;
    }
    void DoBaff(List<Card> cardsToSpawn)
    {
        if (cardsToSpawn.Count != 0)
        {
            for (int i = myCards.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < cardsToSpawn.Count; j++)
                {
                    if (myCards[i] == cardsToSpawn[j])
                    {
                        Card card = null;
                        if (cardsToSpawn[j].GetBasicCard.HP > 0) card = GetMyHealthlessCard();
                        else if (cardsToSpawn[j].GetBasicCard.HP < 0) card = GetPlayerHealthlessCard(cardsToSpawn[j].GetBasicCard.HP);
                        else if (cardsToSpawn[j].GetBasicCard.Damage > 0) card = GetMyWeakestCard();
                        else if (cardsToSpawn[j].GetBasicCard.Damage < 0) card = GetPlayerWeakestCard(cardsToSpawn[j].GetBasicCard.Damage);
                        else Debug.Log("Help");
                        //каст на карту card бафф
                        cardsToSpawn.Remove(cardsToSpawn[j]);
                        break;
                    }
                }
            }
        }
    }
    Card GetMyHealthlessCard()
    {
        bool Strong = false;
        Card card = myCardsOnBoard[0];
        for (int i = 0; i < myCardsOnBoard.Count; i++)
        {
            if (myCardsOnBoard[i].GetBasicCard.Damage > 3 && !Strong)
            {
                card = myCardsOnBoard[i];
                Strong = true;
            }
            else if (!Strong)
            {
                if (card.GetBasicCard.HP < myCardsOnBoard[i].GetBasicCard.HP) card = myCardsOnBoard[i];
            }
            else if (myCardsOnBoard[i].GetBasicCard.Damage > 3 && Strong)
            {
                if (card.GetBasicCard.Damage < myCardsOnBoard[i].GetBasicCard.Damage) card = myCardsOnBoard[i];
            }
        }
        return card;
    }
    Card GetPlayerHealthlessCard(int Debuf)
    {
        Debuf *= -1;
        bool kill = false;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i].GetBasicCard.HP <= Debuf && !kill)
            {
                card = playerCards[i];
                kill = true;
            }
            else if (playerCards[i].GetBasicCard.HP <= Debuf && kill)
            {
                if (card.GetBasicCard.HP < playerCards[i].GetBasicCard.HP) card = playerCards[i];
            }
            else if (!kill)
            {
                if (card.GetBasicCard.HP > playerCards[i].GetBasicCard.HP) card = playerCards[i];
            }
        }
        return card;
    }
    Card GetMyWeakestCard()
    {
        bool Strong = false;
        Card card = myCardsOnBoard[0];
        for (int i = 0; i < myCardsOnBoard.Count; i++)
        {
            if (myCardsOnBoard[i].GetBasicCard.Damage > 3 && !Strong)
            {
                card = myCardsOnBoard[i];
                Strong = true;
            }
            else if (!Strong)
            {
                if (card.GetBasicCard.HP < myCardsOnBoard[i].GetBasicCard.HP) card = myCardsOnBoard[i];
            }
            else if (myCardsOnBoard[i].GetBasicCard.Damage > 3 && Strong)
            {
                if (card.GetBasicCard.Damage < myCardsOnBoard[i].GetBasicCard.Damage) card = myCardsOnBoard[i];
            }
        }
        return card;
    }
    Card GetPlayerWeakestCard(int Debuf)
    {
        Debuf *= -1;
        bool kill = false;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i].GetBasicCard.Damage <= Debuf && !kill)
            {
                card = playerCards[i];
                kill = true;
            }
            else if (playerCards[i].GetBasicCard.Damage <= Debuf && kill)
            {
                if (card.GetBasicCard.Damage < playerCards[i].GetBasicCard.Damage) card = playerCards[i];
            }
            else if (!kill)
            {
                if (card.GetBasicCard.Damage > playerCards[i].GetBasicCard.Damage) card = playerCards[i];
            }
        }
        return card;
    }
    void StashCard()
    {
        //пока ничего
    }
    void Attack(List<Card> StartBoard)
    {
        for (int i = 0; i < StartBoard.Count; i++)
        {
            //
        }
    }
}