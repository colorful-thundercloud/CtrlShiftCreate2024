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
    List<Card> StartBoard;
    List<Card> myCards = new();
    List<Card> myCardsOnBoard = new();
    List<Card> playerCards = new();
    public void EnemyTurn()
    {
        StartBoard = field.GetComponent<Field>().GetCards(true);
        playerCards = field.GetComponent<Field>().GetCards(false);
        myCards = hand.GetComponent<Hand>().GetCards();
        StartCoroutine(SpawnUnit(WhichCardsSpawnUnit()));
        //стопе здесь
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
    IEnumerator SpawnUnit(List<Card> cardsToSpawn)
    {
        int HowMuch = 3 - myCardsOnBoard.Count;
        if (cardsToSpawn.Count != 0 && HowMuch != 0)
        {
            for (int i = myCards.Count - 1; i >= 0; i--)
            {
                if (HowMuch == 0) break;
                for (int j = 0; j < cardsToSpawn.Count; j++)
                {
                    if (myCards[i] == cardsToSpawn[j])
                    {
                        yield return StartCoroutine(MoveCard(myCards[i]));
                        cardsToSpawn.Remove(cardsToSpawn[j]);
                        HowMuch--;
                        break;
                    }
                }
            }
        }
        yield return new WaitForSeconds(1);
        myCardsOnBoard = field.GetComponent<Field>().GetCards(true);
        StartCoroutine(DoBaff(WhichCardsSpawnBaff()));
    }
    IEnumerator MoveCard(Card card)
    {
        float moveSpeed = 7f;
        Vector3 targetPosition = field.gameObject.transform.position;

        if (Vector3.Distance(card.gameObject.transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - card.gameObject.transform.position).normalized;
            card.gameObject.transform.position += moveSpeed * Time.deltaTime * direction;
            yield return null;
            yield return StartCoroutine(MoveCard(card));
        }
        else UpplyCard(card);
    }
    void UpplyCard(Card card)
    {
        card.OnMouseUp();
        field.GetComponent<Field>().addCard(card, true);
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
    IEnumerator DoBaff(List<Card> cardsToSpawn)
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
                        if (cardsToSpawn[j].HP > cardsToSpawn[j].Damage) card = GetMyHealthlessCard();
                        //else if (cardsToSpawn[j].HP < 0) card = GetPlayerHealthlessCard(cardsToSpawn[j].HP);
                        else card = GetMyWeakestCard();
                        //else if (cardsToSpawn[j].Damage < 0) card = GetPlayerWeakestCard(cardsToSpawn[j].Damage);
                        if (card != null)
                        {
                            yield return StartCoroutine(Buffer(card, cardsToSpawn[j]));
                            cardsToSpawn.Remove(cardsToSpawn[j]);
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(StartAttacking(StartBoard));
        if (myCardsOnBoard.Count != 0 && myCardsOnBoard.Count < myCards.Count || myCardsOnBoard.Count == 3 && myCards.Count > 2) StashCard();
    }
    IEnumerator Buffer(Card card, Card buff)
    {
        float moveSpeed = 7f;
        Vector3 targetPosition = card.gameObject.transform.position;

        if (Vector3.Distance(buff.gameObject.transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - buff.gameObject.transform.position).normalized;
            buff.gameObject.transform.position += moveSpeed * Time.deltaTime * direction;
            yield return null;
            yield return StartCoroutine(Buffer(card, buff));
        }
        else UpplyBuff(card, buff);
    }
    void UpplyBuff(Card card, Card buff)
    {
        card.StatsChange(buff.Damage, buff.HP);
        Field.OnCardBeat?.Invoke(buff);
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
    IEnumerator Attack(List<Card> StartBoard)
    {
        if (StartBoard.Count != 0)
        {
            playerCards = field.GetComponent<Field>().GetCards(false);
            playerCards.Sort((a, b) => b.HP.CompareTo(a.HP));//в убывании
            if (playerCards.Count == 0) yield break;
            Card strongestCard = StrongestPlayerCard();
            int CanBeat = -1;
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].Damage > strongestCard.HP) { CanBeat = i; break; }
            if (CanBeat != -1)
            {
                StartBoard[CanBeat].attack(strongestCard);
                StartBoard.Remove(StartBoard[CanBeat]);
            }
            CanBeat = -1;
            Card healthlessCard = HealthlessPlayerCard();
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].Damage > healthlessCard.HP) { CanBeat = i; break; }
            if (CanBeat == -1) CanBeat = 0;
            StartBoard[CanBeat].attack(healthlessCard);
            StartBoard.Remove(StartBoard[CanBeat]);
            StartCoroutine(Attack(StartBoard));
        }
    }

    IEnumerator StartAttacking(List<Card> StartBoard)
    {

        if (StartBoard.Count == 0)
        {
            field.GetComponent<TurnBasedGameplay>().enemyEndMove();
            yield break;
        }
        StartBoard.Sort((a, b) => a.Damage.CompareTo(b.Damage));//в возрастании

        yield return StartCoroutine(Attack(StartBoard));

        if (StartBoard.Count != 0)
        {
            foreach (Card card in StartBoard)
            {
                Player.attackUser(card.Damage);
            }
        }
        field.GetComponent<TurnBasedGameplay>().enemyEndMove();
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