using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class Users : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] GameManager field;
    //[SerializeField] Light2D lighting;

    [SerializeField] Health hp;
    [SerializeField] CardStats stats;
    //Animator anim;

    private void Start()
    {
        stats = new(GetBasicStats());
        hp.OnDeath.AddListener(Death);
        LevelController.onNextLevel.AddListener(heal);
    }
    void heal() => hp.Heal(stats);
    public void NewStats(Health hp)
    {
        this.hp = hp;
        stats = new(GetBasicStats());
        hp.OnDeath.AddListener(Death);
    }
    public List<Stat> GetBasicStats()
    {
        List<Stat> stats = new();

        stats.Add(hp.GetStat(this));

        return stats;
    }
    private bool checkAttack()
    {
        if (gameObject.tag == "enemyCard") return GameManager.GetCards(true).Count == 0;
        else return GameManager.GetCards(false).Count == 0;
    }
    private void OnMouseEnter()
    {
        if (CardController.Selected == null) return;
        if (gameObject.tag == "myCard") return;
        //lighting.color = Color.red;
        //StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f));
    }
    private void OnMouseExit()
    {
        if (CardController.Selected == null) return;
        if (gameObject.tag == "myCard") return;
        //StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f, false));
    }
    public void Attack(CardController attacker)
    {
        if (checkAttack())
        {
            GameManager.UpdateTurns.Invoke(new TurnData(false, attacker.cardID, TurnData.CardAction.user, 0, attacker.GetBasicCard.Title));
            attacker.GetBasicCard.GetAction()
                .Directed(attacker, transform, stats);
        }
    }
    public Health GetHealth { get { return hp; } }
    void Death()
    {
        if (gameObject.tag == "myCard")
            GameManager.OnGameOver.Invoke(false);
        else
        {
            field.NextTurn();
            GameManager.OnGameOver.Invoke(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CardController.Selected == default) return;
        Attack(CardController.Selected);
        CardController.Selected = default;
    }
}
