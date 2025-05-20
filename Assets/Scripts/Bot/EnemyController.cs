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

    public Coroutine MakeTurn(TurnData[] turns) => StartCoroutine(multiplayerAI(turns));
    IEnumerator multiplayerAI(TurnData[] turns)
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
                UseAction(card, turn);
                yield return new WaitForSeconds(0.75f);
            }
        }
        GameManager.OnEndTurn.Invoke(true);
    }
    #region CardUsing
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
    private void UseAction(CardController card, TurnData turn)
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
    }
    /////////////// Multiple

    IEnumerator SpawnUnits(List<CardController> cardsToSpawn)
    {
        foreach (CardController card in cardsToSpawn)
        {
            if (!field.CheckCount(true)) break;
            yield return StartCoroutine(SpawnUnit(card));
            myCardsOnBoard = GameManager.GetCards(true);
            yield return new WaitForSeconds(0.25f);
        }
    }
    IEnumerator SpawnBaffs(List<CardController> buffCards)
    {
        foreach (CardController card in buffCards)
        {
            int targetId = card.GetBasicCard.GetAction().toAllies? 
                myCardsOnBoard[0].cardID : playerCards[0].cardID; /// ИИ всегда накладывает баф только на первую карту
            yield return StartCoroutine(ApplyBuff(card, new(true, card.cardID, TurnData.CardAction.directed, targetId, card.Title.text)));
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator UseActions()
    {
        myCardsOnBoard = GameManager.GetCards(true);
        foreach (var card in myCardsOnBoard)
        {
            if (!card.GetBasicCard.CheckAction(card)) continue;
            TurnData.CardAction action = card.GetBasicCard.GetAction().directed ? 
                TurnData.CardAction.directed : TurnData.CardAction.undirected;
            bool toAlies = card.GetBasicCard.GetAction().toAllies;
            int targetId;
            do
            {
                playerCards = GameManager.GetCards(false);
                if (action == TurnData.CardAction.directed && playerCards.Count == 0)
                    action = TurnData.CardAction.user;
                targetId = action == TurnData.CardAction.user ? 0 : GetTargetId(toAlies);
                TurnData data = new(false, card.cardID, action, targetId, card.Title.text);
                UseAction(card, data);
                yield return new WaitForSeconds(1.25f);
            }
            while (card.GetBasicCard.CheckAction(card));
        }
    }
    ///
    private int GetTargetId(bool toAlies) => 
        toAlies ? myCardsOnBoard[0].cardID : playerCards[0].cardID;
    public void UseCard(int id) => StartCoroutine(useCard(id));
    IEnumerator useCard(int id)
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(SpawnUnit(hand.FindCard(id)));
        GameManager.OnEndTurn.Invoke(true);
    }
    #endregion
    ///////////////
    public void SingleAI() => StartCoroutine(singleAI());
    IEnumerator singleAI()
    {
        yield return new WaitForSeconds(1f);

        //Cast units
        yield return StartCoroutine(SpawnUnits(GetHandCards()));

        //ApplyBafs
        myCardsOnBoard = GameManager.GetCards(true);
        playerCards = GameManager.GetCards(false);
        yield return StartCoroutine(SpawnBaffs(GetHandCards(false)));

        //Use skills
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(UseActions());

        GameManager.OnEndTurn.Invoke(true); // возврат хода игроку
    }
    List<CardController> GetHandCards(bool spawnable = true)
    {
        List<CardController> cards = new();
        foreach (var card in hand.GetCards())
        {
            bool hp = card.GetStat(Effect.BuffedStats.hp.ToString()) != default;
            if (hp && spawnable || !hp && !spawnable)
                cards.Add(card);
        }
        return cards;
    }
}