using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float CardSpeed;
    [SerializeField] Hand hand;
    [SerializeField] GameManager field;
    [SerializeField] float showWaitTime;

    List<CardController> myCardsOnBoard, playerCards, myCards;
    [SerializeField] Users player;
    Coroutine size;

    public Coroutine MakeTurn(TurnData[] turns) => StartCoroutine(test(turns));
    IEnumerator test(TurnData[] turns)
    {
        foreach (var turn in turns)
        {
            yield return new WaitForSeconds(0.15f);
            myCards = hand.GetCards();
            myCardsOnBoard = GameManager.GetCards(true);
            playerCards = GameManager.GetCards(false);
            CardController card = (turn.InHand) ? myCards.Find(card=> card.cardID == turn.CardId) : myCardsOnBoard.Find(card => card.cardID == turn.CardId);
            if (turn.InHand) //cast
            {
                if (card.GetBasicCard.GetType().IsSubclassOf(typeof(OneShot)))
                    yield return StartCoroutine(ApplyBuff(card, turn));
                else yield return StartCoroutine(SpawnUnit(card));
            }
            else // use
            {
                switch (turn.Action)
                {
                    case TurnData.CardAction.directed:
                        card.GetBasicCard.GetAction()
                            .Directed(card,
                            playerCards.Find(card => card.cardID == turn.TargetId).transform,
                            playerCards.Find(card => card.cardID == turn.TargetId).GetStats);
                        break;
                    case TurnData.CardAction.undirected:
                        card.GetBasicCard.GetAction().Undirected(card);
                        break;
                    case TurnData.CardAction.user:
                        player.Attack(card);
                        break;
                }

                yield return new WaitForSeconds(0.75f);
            }
        }
        GameManager.OnEndTurn.Invoke(true);
    }
    private IEnumerator SpawnUnit(CardController unit)
    {
        if (unit.GetBasicCard.cast(unit))
        {
            unit.isCasting = true;
            unit.Show(true);
            StartCoroutine(Mover.SmoothSizeChange(field.CardSize, unit.transform, CardSpeed));
            yield return StartCoroutine(Mover.MoveCard(unit.transform, (Vector2)field.GetEnemyField.position, CardSpeed));
            unit.cast();
        }
    }
    private IEnumerator ApplyBuff(CardController card, TurnData data)
    {
        card.isCasting = true;
        card.Show(true);
        size = StartCoroutine(Mover.SmoothSizeChange(field.CardSize * 2f, card.transform, CardSpeed));
        yield return StartCoroutine(Mover.MoveCard(card.transform, (Vector2)field.GetEnemyField.position, CardSpeed));
        yield return new WaitForSeconds(showWaitTime);

        CardController target;
        if (card.GetBasicCard.GetAction().toAllies)
            target = myCardsOnBoard.Find(t => t.cardID == data.TargetId);
        else target = playerCards.Find(t => t.cardID == data.TargetId);

        StopCoroutine(size);
        StartCoroutine(Mover.MoveCard(card.transform, target.transform.position, CardSpeed));
        yield return StartCoroutine(Mover.SmoothSizeChange(field.CardSize, card.transform, CardSpeed));
        card.GetBasicCard.GetAction().Directed(card, target.transform, target.GetStats);
        card.cast();
    }
    ///////////////
    public void UseCard(int id) => StartCoroutine(useCard(id));
    IEnumerator useCard(int id)
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(SpawnUnit(hand.FindCard(id)));
        GameManager.OnEndTurn.Invoke(true);
    }
    ///////////////
    IEnumerator turn()
    {
        yield return new WaitForSeconds(0.5f);

        myCards = hand.GetCards();

        List<CardController> units = GetListCardType(myCards, typeof(UnitCard), false);
        yield return StartCoroutine(SpawnUnit(units));


        myCardsOnBoard = GameManager.GetCards(true);
        playerCards = GameManager.GetCards(false);

        List<CardController> buffCards = GetListCardType(myCards, typeof(BuffOneshot), true);
        yield return StartCoroutine(SpawnBaffs(buffCards));

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(AttackPlayerCards());

        //GameManager.OnEndTurn.Invoke(false); // возврат хода игроку
    }
    IEnumerator SpawnBaffs(List<CardController> buffCards)
    {
        Coroutine size;
        foreach (CardController card in buffCards)
        {
            yield return new WaitForSeconds(0.15f);
            card.isCasting = true;
            card.Show(true);
            size = StartCoroutine(Mover.SmoothSizeChange(field.CardSize*2f, card.transform, CardSpeed));
            yield return StartCoroutine(Mover.MoveCard(card.transform, (Vector2)field.GetEnemyField.position, CardSpeed));
            yield return new WaitForSeconds(showWaitTime);
            CardController target;
            if (card.GetBasicCard.GetAction().toAllies)
                target = myCardsOnBoard[0];
            else target = playerCards[0];
            StopCoroutine(size);
            StartCoroutine(Mover.MoveCard(card.transform, target.transform.position, CardSpeed));
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
            playerCards = GameManager.GetCards(false);
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
            yield return new WaitForSeconds(0.15f);
            if (card.GetBasicCard.cast(card))
            {
                card.isCasting = true;
                card.Show(true);
                StartCoroutine(Mover.SmoothSizeChange(field.CardSize, card.transform, CardSpeed));
                yield return StartCoroutine(Mover.MoveCard(card.transform, (Vector2)field.GetEnemyField.position, CardSpeed));
                card.cast();
            }

            myCardsOnBoard = GameManager.GetCards(true);
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
}