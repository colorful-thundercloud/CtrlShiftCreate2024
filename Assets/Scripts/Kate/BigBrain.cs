using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BigBrain : MonoBehaviour
{
    [SerializeField] Field field;
    [SerializeField] Hand hand;
    [SerializeField] Users Player;
    List<Card> myCards = new();
    List<Card> myCardsOnBoard = new();
    List<Card> playerCards = new();
    public void EnemyTurn()
    {
        List<Card> StartBoard = field.GetComponent<Field>().GetCards(true);
        playerCards = field.GetComponent<Field>().GetCards(false);
        myCards = hand.GetComponent<Hand>().GetCards();
        SpawnUnit(WhichCardsSpawnUnit(), 3 - myCardsOnBoard.Count);
        myCardsOnBoard = field.GetComponent<Field>().GetCards(true);
        DoBaff(WhichCardsSpawnBaff());
        Attack(StartBoard);
        if (myCardsOnBoard.Count != 0 && myCardsOnBoard.Count < myCards.Count || myCardsOnBoard.Count == 3 && myCards.Count > 2) StashCard();
        field.GetComponent<TurnBasedGameplay>().enemyEndMove();
    }
    List<Card> WhichCardsSpawnUnit()
    {
        List<Card> Spawn = new();
        foreach (Card card in myCards)
        {
            if (card.GetBasicCard.Type == BasicCard.cardType.Unit) Spawn.Add(card);
        }
        //добавить сортировку по урону или по хп
        if (StrengthInHealth()) myCards.Sort((x, y) => x.HP.CompareTo(y.HP));
        else myCards.Sort((x, y) => x.Damage.CompareTo(y.Damage));
        return Spawn;
    }
    bool StrengthInHealth()
    {
        bool healthMatter;
        int PlHP = 0, PlDM = 0;
        int HP = 0, DM = 0;
        foreach (Card card in playerCards)
        {
            PlHP += card.HP;
            PlDM += card.Damage;
        }
        foreach (Card card in myCardsOnBoard)
        {
            HP += card.HP;
            DM += card.Damage;
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
                        field.GetComponent<Field>().addCard(myCards[i], true);
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
            Debug.Log(cardsToSpawn[0].HP);
            for (int i = myCards.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < cardsToSpawn.Count; j++)
                {
                    if (myCards[i] == cardsToSpawn[j])
                    {
                        Card card = null;
                        if (cardsToSpawn[j].HP > cardsToSpawn[j].Damage) card = GetMyHealthlessCard();
                        //else if (cardsToSpawn[j].HP < 0) card = GetPlayerHealthlessCard(cardsToSpawn[j].HP);
                        else card = GetMyWeakestCard();
                        //else if (cardsToSpawn[j].Damage < 0) card = GetPlayerWeakestCard(cardsToSpawn[j].Damage);
                        if (card != null)
                        {
                            card.StatsChange(cardsToSpawn[j].Damage, cardsToSpawn[j].HP);
                            Field.OnCardBeat?.Invoke(cardsToSpawn[j]);
                            cardsToSpawn.Remove(cardsToSpawn[j]);
                            break;
                        }
                    }
                }
            }
        }
    }
    Card GetMyHealthlessCard()
    {
        bool Strong = false;
        if (myCardsOnBoard.Count == 0) return null;
        Card card = myCardsOnBoard[0];
        for (int i = 0; i < myCardsOnBoard.Count; i++)
        {
            if (myCardsOnBoard[i].Damage > 3 && !Strong)
            {
                card = myCardsOnBoard[i];
                Strong = true;
            }
            else if (!Strong)
            {
                if (card.HP < myCardsOnBoard[i].HP) card = myCardsOnBoard[i];
            }
            else if (myCardsOnBoard[i].Damage > 3 && Strong)
            {
                if (card.Damage < myCardsOnBoard[i].Damage) card = myCardsOnBoard[i];
            }
        }
        return card;
    }
    Card GetPlayerHealthlessCard(int Debuf)
    {
        if (playerCards.Count == 0) return null;
        Debuf *= -1;
        bool kill = false;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i].HP <= Debuf && !kill)
            {
                card = playerCards[i];
                kill = true;
            }
            else if (playerCards[i].HP <= Debuf && kill)
            {
                if (card.HP < playerCards[i].HP) card = playerCards[i];
            }
            else if (!kill)
            {
                if (card.HP > playerCards[i].HP) card = playerCards[i];
            }
        }
        return card;
    }
    Card GetMyWeakestCard()
    {
        if (myCardsOnBoard.Count == 0) return null;
        bool Strong = false;
        Card card = myCardsOnBoard[0];
        for (int i = 0; i < myCardsOnBoard.Count; i++)
        {
            if (myCardsOnBoard[i].Damage > 3 && !Strong)
            {
                card = myCardsOnBoard[i];
                Strong = true;
            }
            else if (!Strong)
            {
                if (card.HP < myCardsOnBoard[i].HP) card = myCardsOnBoard[i];
            }
            else if (myCardsOnBoard[i].Damage > 3 && Strong)
            {
                if (card.Damage < myCardsOnBoard[i].Damage) card = myCardsOnBoard[i];
            }
        }
        return card;
    }
    Card GetPlayerWeakestCard(int Debuf)
    {
        Debuf *= -1;
        if (playerCards.Count == 0) return null;
        bool kill = false;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i].Damage <= Debuf && !kill)
            {
                card = playerCards[i];
                kill = true;
            }
            else if (playerCards[i].Damage <= Debuf && kill)
            {
                if (card.Damage < playerCards[i].Damage) card = playerCards[i];
            }
            else if (!kill)
            {
                if (card.Damage > playerCards[i].Damage) card = playerCards[i];
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
        StartBoard.Sort((a, b) => a.Damage.CompareTo(b.Damage));//в возрастании
        playerCards.Sort((a, b) => b.HP.CompareTo(a.HP));//в убывании
        while (StartBoard.Count != 0)
        {
            if (playerCards.Count == 0) break;
            Card strongestCard = StrongestPlayerCard();
            int CanBeat = -1;
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].Damage > strongestCard.HP) {CanBeat = i; break;}
            if (CanBeat != -1)
            {
                StartBoard[CanBeat].attack(strongestCard);
                StartBoard.Remove(StartBoard[CanBeat]);
                continue;
            }
            Card healthlessCard = HealthlessPlayerCard();
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].Damage > strongestCard.HP) {CanBeat = i; break;}
            if (CanBeat != -1)
            {
                StartBoard[CanBeat].attack(strongestCard);
                StartBoard.Remove(StartBoard[CanBeat]);
                continue;
            }
            else
            {
                StartBoard[0].attack(strongestCard);
                StartBoard.Remove(StartBoard[0]);
            }
        }
        if (StartBoard.Count != 0)
        {
            foreach (Card card in StartBoard)
            {
                Player.attackUser(card.Damage);
            }
        }
    }
    Card StrongestPlayerCard()
    {
        if (playerCards.Count == 0) return null;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (card.Damage < playerCards[i].Damage) card = playerCards[i];
        }
        return card;
    }
    Card HealthlessPlayerCard()
    {
        if (playerCards.Count == 0) return null;
        Card card = playerCards[0];
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (card.HP > playerCards[i].HP) card = playerCards[i];
        }
        return card;
    }
}