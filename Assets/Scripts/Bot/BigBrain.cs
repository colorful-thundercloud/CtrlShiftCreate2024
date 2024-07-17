using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBrain : MonoBehaviour
{
    [SerializeField] float CardSpeed;
    [SerializeField] Hand hand;
    [SerializeField] Field field;
    [SerializeField] float showWaitTime;

    List<CardController> myCardsOnBoard, playerCards, myCards;
    [SerializeField] Users player;

    private void Start()
    {
        TurnBasedGameplay.OnEndTurn.AddListener(onNewTurn);
    }
    void onNewTurn(bool isEnemy)
    {
        if (!isEnemy) return;

        myCards = hand.GetCards();

        StartCoroutine(turn());
    }
    IEnumerator turn()
    {
        yield return StartCoroutine(SpawnUnit(myCards));

        myCards = hand.GetCards();

        myCardsOnBoard = Field.GetCards(true);
        playerCards = Field.GetCards(false);

        List<CardController> buffCards = GetListCardType(myCards, typeof(BuffOneshot), true);
        yield return StartCoroutine(SpawnBaffs(buffCards));

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(AttackPlayerCards());

        TurnBasedGameplay.OnEndTurn.Invoke(false); // возврат хода игроку
    }
    IEnumerator SpawnBaffs(List<CardController> buffCards)
    {
        Coroutine size;
        foreach (CardController card in buffCards)
        {
            card.Show(true);
            size = StartCoroutine(Mover.SmoothSizeChange(field.CardSize*2f, card.transform, CardSpeed));
            yield return StartCoroutine(Mover.MoveCard(card, field.GetEnemyField.position, CardSpeed));
            yield return new WaitForSeconds(showWaitTime);
            CardController target;
            if (card.GetBasicCard.GetAction().toAllies)
                target = myCardsOnBoard[0];
            else target = playerCards[0];
            StopCoroutine(size);
            StartCoroutine(Mover.MoveCard(card, target.transform.position, CardSpeed));
            yield return StartCoroutine(Mover.SmoothSizeChange(field.CardSize, card.transform, CardSpeed));
            card.GetBasicCard.GetAction().Directed(card, target.transform, target.GetStats);
            card.cast();
            //card.GetBasicCard.GetAction()
            //    .Directed(card, transform, target.GetStats);
        }
    }
    IEnumerator AttackPlayerCards()
    {
        List<CardController> aviableUnits = GetListCardType(myCardsOnBoard, typeof(UnitCard), true);
        if (aviableUnits.Count == 0) yield break;
        foreach(CardController card in aviableUnits)
        {
            playerCards = Field.GetCards(false);
            if (playerCards.Count != 0)
            {
                card.GetBasicCard.GetAction()
                    .Directed(card, playerCards[0].transform, playerCards[0].GetStats);
            }
            else player.Attack(card);

            yield return new WaitForSeconds(1.25f);
        }
        yield return StartCoroutine(AttackPlayerCards());
    }
    IEnumerator SpawnUnit(List<CardController> cardsToSpawn)
    {
        foreach (CardController card in cardsToSpawn)
        {
            if (!field.CheckCount(true)) break;
            if (typeof(UnitCard) != card.GetBasicCard.GetType()) continue;
            if (card.GetBasicCard.cast(card))
            {
                card.Show(true);
                StartCoroutine(Mover.SmoothSizeChange(field.CardSize, card.transform, CardSpeed));
                yield return StartCoroutine(Mover.MoveCard(card, field.GetEnemyField.position, CardSpeed));
                card.cast();
            }

            myCardsOnBoard = Field.GetCards(true);
        }
    }
    List<CardController> GetListCardType(List<CardController> toSearch, Type type, bool aviable)
    {
        List<CardController> cards = new();
        foreach(CardController card in toSearch)
            if(type == card.GetBasicCard.GetType()&& aviable == card.GetBasicCard.CheckAction(card))
                cards.Add(card);
        return cards;
    }
    /*
    [SerializeField] Users Player;
    List<Card> StartBoard;
    bool Buff = false;
    List<Card> myCards = new();
    List<Card> myCardsOnBoard = new();
    List<Card> playerCards = new();
    public void EnemyTurn()
    {
        //if (Player.Hp <= 0 || GetComponent<Users>().Hp <= 0) field.GetComponent<TurnBasedGameplay>().enemyEndMove();
        myCardsOnBoard = StartBoard = Field.GetCards(true);
        playerCards = Field.GetCards(false);
        myCards = hand.GetCards();
        StartCoroutine(SpawnUnit(WhichCardsSpawnUnit()));
    }
    List<Card> WhichCardsSpawnUnit()
    {
        List<Card> Spawn = new();
        foreach (Card card in myCards)
        {
            //if (card.GetBasicCard.Type == BasicCard.cardType.Unit) Spawn.Add(card);
        }
        //добавить сортировку по урону или по хп
        if (StrengthInHealth()) myCards.Sort((x, y) => x.HP.CompareTo(y.HP));
        else myCards.Sort((x, y) => x.Damage.CompareTo(y.damage));
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
            PlDM += card.damage;
        }
        foreach (Card card in myCardsOnBoard)
        {
            HP += card.HP;
            DM += card.damage;
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
        myCardsOnBoard = Field.GetCards(true);
        StartCoroutine(DoBaff(WhichCardsSpawnBaff()));
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
            //if (card.GetBasicCard.Type == BasicCard.cardType.Buff) Spawn.Add(card);
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
                        if (cardsToSpawn[j].HP >= cardsToSpawn[j].damage) {card = GetMyHealthlessCard(); if (card == null) card = GetMyHealthyestCard();}
                        //else if (cardsToSpawn[j].HP < 0) card = GetPlayerHealthlessCard(cardsToSpawn[j].HP);
                        else card = GetMyHealthlessCard();
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
        float moveSpeed = 15f;
        buff.isCasted= true;
        buff.gameObject.transform.position = new Vector3(buff.gameObject.transform.position.x, buff.gameObject.transform.position.y, -10f);
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
        card.StatsChange(buff.damage, buff.HP);
        SoundPlayer.Play(buff.CastSound);
        Field.OnCardBeat?.Invoke(buff);
    }
    Card GetMyHealthlessCard()
    {
        myCardsOnBoard.Sort((a, b) => b.HP.CompareTo(a.HP));//убывание
        Card card = myCardsOnBoard[myCardsOnBoard.Count - 1];
        for (int i = 0; i < myCardsOnBoard.Count; i++)
        {
            if (myCardsOnBoard[i].HP < 6 && myCardsOnBoard[i].HP > card.HP)
            {
                card = myCardsOnBoard[i];
            }
            if (myCardsOnBoard[i].HP == card.HP)
            {
                if (myCardsOnBoard[i].damage > card.damage) card = myCardsOnBoard[i];
            }
        }
        if (card.HP > 4) card = null;
        return card;
    }
    Card GetMyHealthyestCard()
    {
        myCardsOnBoard.Sort((a, b) => b.Damage.CompareTo(a.damage));//убывание
        Card card;
        if (StartBoard.Count != 0)
        {
            card = StartBoard[StartBoard.Count - 1];
            for (int i = 0; i < StartBoard.Count; i++)
            {
                if (StartBoard[i].damage < 6 && StartBoard[i].HP > card.HP)
                {
                    card = StartBoard[i];
                }
                if (StartBoard[i].damage == card.damage)
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
                if (myCardsOnBoard[i].damage < 6 && myCardsOnBoard[i].HP > card.HP)
                {
                    card = myCardsOnBoard[i];
                }
                if (myCardsOnBoard[i].damage == card.damage)
                {
                    if (myCardsOnBoard[i].HP > card.HP) card = myCardsOnBoard[i];
                }
            }
        }
        if (card.damage > 4) card = null;
        return card;
    }
    void StashCard()
    {
        *//*myCards = hand.GetCards();
        myCards.Sort((a, b) => a.Damage.CompareTo(b.Damage));//возрастание
        for (int i = myCards.Count; i > 0; i--)
        {
            hand.BeatCard(myCards[0]);
            myCards.Remove(myCards[0]);
        }
        Debug.Log("stash");*//*
    }
    IEnumerator Attack(List<Card> StartBoard)
    {
        yield return new WaitForSeconds(1f);
        StartBoard.Sort((a, b) => a.Damage.CompareTo(b.damage));//в возрастании
        playerCards.Sort((a, b) => b.HP.CompareTo(a.HP));//в убывании
        while (StartBoard.Count != 0)
        {
            if (playerCards.Count == 0) break;
            Card strongestCard = StrongestPlayerCard();
            int CanBeat = -1;
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].damage > strongestCard.HP) {CanBeat = i; break;}
            if (CanBeat != -1)
            {
                if (StartBoard[CanBeat].damage >= strongestCard.HP) playerCards.Remove(strongestCard);
                //StartBoard[CanBeat].attack(strongestCard);
                StartBoard.Remove(StartBoard[CanBeat]);
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            Card healthlessCard = HealthlessPlayerCard();
            for (int i = 0; i < StartBoard.Count; i++) if (StartBoard[i].damage > healthlessCard.HP) {CanBeat = i; break;}
            if (CanBeat != -1)
            {
                if (StartBoard[CanBeat].damage >= healthlessCard.HP) playerCards.Remove(healthlessCard);
                //StartBoard[CanBeat].attack(healthlessCard);
                StartBoard.Remove(StartBoard[CanBeat]);
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            else if (StartBoard.Count > 0)
            {
                if (StartBoard[0].damage >= strongestCard.HP) playerCards.Remove(strongestCard);
                //StartBoard[0].attack(strongestCard);
                StartBoard.Remove(StartBoard[0]);
                yield return new WaitForSeconds(0.5f);
                continue;
            }
        }
        if (StartBoard.Count != 0)
        {
            yield return new WaitForSeconds(1.1f);
            for (int i = 0; i < StartBoard.Count; i++)
            {
                //if (Player.Hp <= 0) field.GetComponent<TurnBasedGameplay>().enemyEndMove();
                //Player.attackUser(StartBoard[i]);
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
            if (card.damage < playerCards[i].damage) card = playerCards[i];
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
    }*/
}