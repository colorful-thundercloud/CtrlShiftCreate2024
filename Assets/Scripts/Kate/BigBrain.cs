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
    bool Buff = false;
    List<Card> myCards = new();
    List<Card> myCardsOnBoard = new();
    List<Card> playerCards = new();
    public void EnemyTurn()
    {
        if (Player.Hp <= 0 || GetComponent<Users>().Hp <= 0) field.GetComponent<TurnBasedGameplay>().enemyEndMove();
        myCardsOnBoard = StartBoard = field.GetCards(true);
        playerCards = field.GetCards(false);
        myCards = hand.GetCards();
        StartCoroutine(SpawnUnit(WhichCardsSpawnUnit()));
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
        myCardsOnBoard = field.GetCards(true);
        StartCoroutine(DoBaff(WhichCardsSpawnBaff()));
    }
    IEnumerator MoveCard (Card card)
    {
        float moveSpeed = 10f;
        Vector3 targetPosition = field.GetEnemyField().position;

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
        field.addCard(card, true);
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
                        Card card;
                        if (cardsToSpawn[j].HP >= cardsToSpawn[j].Damage) {card = GetMyHealthlessCard(); if (card == null) card = GetMyWeakestCard();}
                        //else if (cardsToSpawn[j].HP < 0) card = GetPlayerHealthlessCard(cardsToSpawn[j].HP);
                        else card = GetMyWeakestCard();
                        //else if (cardsToSpawn[j].Damage < 0) card = GetPlayerWeakestCard(cardsToSpawn[j].Damage);
                        if (card != null)
                        {
                            Buff = false;
                            yield return StartCoroutine(Buffer(card, cardsToSpawn[j]));
                            cardsToSpawn.Remove(cardsToSpawn[j]);
                            break;
                        }
                    }
                }
            }
        }
        StartCoroutine(Attack(StartBoard));
    }
    IEnumerator Buffer(Card card, Card buff)
    {
        float moveSpeed = 9f;
        buff.isCasted= true;
        buff.gameObject.transform.position = new Vector3(buff.gameObject.transform.position.x, buff.gameObject.transform.position.y, -2f);
        foreach (Transform child in buff.gameObject.transform) child.gameObject.SetActive(true);
        Vector3 targetPosition = field.gameObject.transform.position;
        if (!Buff && Vector2.Distance(buff.gameObject.transform.position, targetPosition) > 0.1f)
        {
            if (buff.gameObject.transform.localScale.x < 1.5f) buff.gameObject.transform.localScale *= 1.1f;
            Vector3 direction = (targetPosition - buff.gameObject.transform.position).normalized;
            buff.gameObject.transform.position += moveSpeed * Time.deltaTime * direction;
            yield return null;
            yield return StartCoroutine(Buffer(card, buff));
        }
        else if (!Buff)
        {
            moveSpeed = 11f;
            Buff = true;
            yield return new WaitForSeconds(0.5f);
        }
        targetPosition = card.gameObject.transform.position;
        if (buff == null) yield break;
        if (Buff && Vector2.Distance(buff.gameObject.transform.position, targetPosition) > 0.1f)
        {
            if (buff.gameObject.transform.localScale.x > 1f) buff.gameObject.transform.localScale /= 1.1f;
            Vector3 direction = (targetPosition - buff.gameObject.transform.position).normalized;
            buff.gameObject.transform.position += moveSpeed * Time.deltaTime * direction;
            yield return null;
            yield return StartCoroutine(Buffer(card, buff));
        }
        else if (Buff) UpplyBuff(card, buff);
    }
    void UpplyBuff(Card card, Card buff)
    {
        card.StatsChange(buff.Damage, buff.HP);
        Field.OnCardBeat?.Invoke(buff);
    }
    Card GetMyHealthlessCard()
    {
        myCardsOnBoard.Sort((a, b) => b.HP.CompareTo(a.HP));//убывание
        Card card = myCardsOnBoard[myCardsOnBoard.Count - 1];
        for (int i = 0; i < myCardsOnBoard.Count; i++)
        {
            if (myCardsOnBoard[i].HP < 5 && myCardsOnBoard[i].HP > card.HP)
            {
                card = myCardsOnBoard[i];
            }
            if (myCardsOnBoard[i].HP == card.HP)
            {
                if (myCardsOnBoard[i].Damage > card.Damage) card = myCardsOnBoard[i];
            }
        }
        if (card.HP > 4) card = null;
        return card;
    }
    Card GetMyWeakestCard()
    {
        myCardsOnBoard.Sort((a, b) => b.Damage.CompareTo(a.Damage));//убывание
        Card card;
        if (StartBoard.Count != 0)
        {
            card = StartBoard[StartBoard.Count - 1];
            for (int i = 0; i < StartBoard.Count; i++)
            {
                if (StartBoard[i].Damage < 5 && StartBoard[i].Damage > card.Damage)
                {
                    card = StartBoard[i];
                }
                if (StartBoard[i].Damage == card.Damage)
                {
                    if (StartBoard[i].HP > card.HP) card = StartBoard[i];
                }
            }
        }
        else
        {
            card = myCardsOnBoard[myCardsOnBoard.Count - 1];
            for (int i = 0; i < myCardsOnBoard.Count; i++)
            {
                if (myCardsOnBoard[i].Damage < 5 && myCardsOnBoard[i].Damage > card.Damage)
                {
                    card = myCardsOnBoard[i];
                }
                if (myCardsOnBoard[i].Damage == card.Damage)
                {
                    if (myCardsOnBoard[i].HP > card.HP) card = myCardsOnBoard[i];
                }
            }
        }
        if (card.Damage > 4) card = null;
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
        /*myCards = hand.GetCards();
        myCards.Sort((a, b) => a.Damage.CompareTo(b.Damage));//возрастание
        for (int i = myCards.Count; i > 0; i--)
        {
            hand.BeatCard(myCards[0]);
            myCards.Remove(myCards[0]);
        }
        Debug.Log("stash");*/
    }
    IEnumerator Attack(List<Card> StartBoard)
    {
        yield return new WaitForSeconds(1f);
        StartBoard.Sort((a, b) => a.Damage.CompareTo(b.Damage));//в возрастании
        playerCards.Sort((a, b) => b.HP.CompareTo(a.HP));//в убывании
        while (StartBoard.Count != 0)
        {
            if (playerCards.Count == 0) break;
            Card strongestCard = StrongestPlayerCard();
            int CanBeat = -1;
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].Damage > strongestCard.HP && strongestCard.Damage > 4) {CanBeat = i; break;}
            if (CanBeat != -1)
            {
                if (StartBoard[CanBeat].Damage >= strongestCard.HP) playerCards.Remove(strongestCard);
                StartBoard[CanBeat].attack(strongestCard);
                StartBoard.Remove(StartBoard[CanBeat]);
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            Card healthlessCard = HealthlessPlayerCard();
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].Damage > healthlessCard.HP) {CanBeat = i; break;}
            if (CanBeat != -1)
            {
                if (StartBoard[CanBeat].Damage >= healthlessCard.HP) playerCards.Remove(healthlessCard);
                StartBoard[CanBeat].attack(healthlessCard);
                StartBoard.Remove(StartBoard[CanBeat]);
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            if (StartBoard.Count > 0 && playerCards.Count > 0 && strongestCard != null)
            {
                if (StartBoard[0].Damage >= strongestCard.HP) playerCards.Remove(strongestCard);
                StartBoard[0].attack(strongestCard);
                StartBoard.Remove(StartBoard[0]);
                yield return new WaitForSeconds(0.5f);
                continue;
            }
        }
        if (StartBoard.Count != 0)
        {
            foreach (Card card in StartBoard)
            {
                if (Player.Hp <= 0) field.GetComponent<TurnBasedGameplay>().enemyEndMove();
                Player.attackUser(card);
                yield return new WaitForSeconds(0.5f);
            }
        }
        if (myCards.Count > 1) StashCard();
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