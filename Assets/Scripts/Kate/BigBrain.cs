using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BigBrain : MonoBehaviour
{
    [SerializeField] GameObject Field;
    [SerializeField] GameObject Hand;
    [SerializeField] GameObject Player;
    List<Card> myCards = new();
    List<Card> myCardsOnBoard = new();
    List<Card> playerCards = new();
    public void EnemyTurn()
    {
        myCardsOnBoard = Field.GetComponent<Field>().GetCards(true);//карты босса на столе
        List<Card> StartBoard = myCardsOnBoard;
        playerCards = Field.GetComponent<Field>().GetCards(false);//карты игрока на столе
        myCards = Hand.GetComponent<Hand>().GetCards();//карты в руке
        SpawnUnit(WhichCardsSpawnUnit(), 3 - myCardsOnBoard.Count);//спавним юнитов в свободное место
        myCardsOnBoard = Field.GetComponent<Field>().GetCards(true);
        //DoBaff(WhichCardsSpawnBaff());
        Attack(StartBoard);
        if (myCardsOnBoard.Count != 0 && myCardsOnBoard.Count < myCards.Count || myCardsOnBoard.Count == 3 && myCards.Count > 1) StashCard();
        Field.GetComponent<TurnBasedGameplay>().enemyEndMove();
    }
    List<Card> WhichCardsSpawnUnit()
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
        if (HowMuch == 0) return;
        if (cardsToSpawn.Count != 0)
        {
            for (int i = myCards.Count - 1; i >= 0; i--)
            {
                if (HowMuch == 0) return;
                for (int j = 0; j < cardsToSpawn.Count; j++)
                {
                    if (myCards[i] == cardsToSpawn[j])
                    {
                        myCards[i].EnemyCast();
                        myCards[i].OnMouseUp();
                        Field.GetComponent<Field>().addCard(myCards[i], true);
                        cardsToSpawn.Remove(cardsToSpawn[j]);
                        HowMuch--;
                        break;
                    }
                }
            }
        }
    }
    List<Card> WhichCardsSpawnBaff()
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
                        Debug.Log("Help");
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
        //сброс всех карт с руки
    }
    void Attack(List<Card> StartBoard)
    {
        if (StartBoard.Count == 0) return;
        StartBoard.Sort((a, b) => a.GetBasicCard.Damage.CompareTo(b.GetBasicCard.Damage));//в возрастании
        playerCards.Sort((a, b) => b.GetBasicCard.HP.CompareTo(a.GetBasicCard.HP));//в убывании
        while (StartBoard.Count != 0)
        {
            if (playerCards.Count == 0) break;
            Card strongestCard = StrongestPlayerCard();
            int CanBeat = -1;
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].GetBasicCard.Damage > strongestCard.GetBasicCard.HP) {CanBeat = i; break;}
            if (CanBeat != -1) CanBeat = -1;//убиваем карту, удаляем атакующую карту из массива StartBoard и массива strenght и continue
            Card healthlessCard = HealthlessPlayerCard();
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].GetBasicCard.Damage > strongestCard.GetBasicCard.HP) {CanBeat = i; break;}
            if (CanBeat != -1) CanBeat = -1;//убиваем карту, удаляем атакующую карту из массива StartBoard и массива strenght и continue
            //если дошли досюда просто атакуем слабейшей картой сильнейшую
        }
        if (StartBoard.Count != 0)
        {
            foreach (Card card in StartBoard)
            {
                //атакуем игрока напрямую
            }
        }
    }
    Card StrongestPlayerCard()
    {
        if (playerCards.Count == 0) return null;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (card.GetBasicCard.Damage < playerCards[i].GetBasicCard.Damage) card = playerCards[i];
        }
        return card;
    }
    Card HealthlessPlayerCard()
    {
        if (playerCards.Count == 0) return null;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (card.GetBasicCard.HP > playerCards[i].GetBasicCard.HP) card = playerCards[i];
        }
        return card;
    }
}