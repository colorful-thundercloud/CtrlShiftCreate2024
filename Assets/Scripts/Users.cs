using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Users : HaveStats
{
    [SerializeField] Field field;
    [SerializeField] GameObject winWindow, defeatWindow;
    [SerializeField] Light2D lighting;
    [SerializeField] int HP;
    public int Hp { get { return currentHP; } }
    private void Start()
    {
        currentHP = HP;
        hp.text = HP.ToString();
    }
    private bool checkAttack()
    {
        if (gameObject.tag == "enemyCard") return field.GetCards(true).Count == 0;
        else return field.GetCards(false).Count == 0;
    }
    private void OnMouseEnter()
    {
        if (Field.SelectedCard == null) return;
        if (gameObject.tag == "myCard") return;
        lighting.color = Color.red;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f));
    }
    private void OnMouseExit()
    {
        if (Field.SelectedCard == null) return;
        if (gameObject.tag == "myCard") return;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f, false));
    }
    private void OnMouseDown()
    {
        if (Field.SelectedCard == null) return;
        if(checkAttack())
        {
            Field.SelectedCard.attackUser(this);
            Field.SelectedCard.used = true;
            Field.SelectedCard.turnOfLight();
            Field.SelectedCard = null;
        }
    }
    public void attackUser(Card attacker)
    {
        if (checkAttack())
        {
            attacker.attackUser(this);
            StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f, false));
        }
    }
    public override void StatsChange(int atk = 0, int health = 0)
    {
        base.StatsChange(atk, health);
        if (currentHP <= 0) Death();
    }
    void Death()
    {
        Time.timeScale = 0;
        if (gameObject.tag == "myCard") defeatWindow.gameObject.SetActive(true);
        else winWindow.gameObject.SetActive(true);
    }
}
