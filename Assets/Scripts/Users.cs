using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Users : MonoBehaviour
{
    [SerializeField] Field field;
    [SerializeField] GameObject winWindow, defeatWindow;
    [SerializeField] Light2D lighting;

    [SerializeField] Health hp;
    private void Start()
    {
        //hp = new();
    }
    private bool checkAttack()
    {
        if (gameObject.tag == "enemyCard") return Field.GetCards(true).Count == 0;
        else return Field.GetCards(false).Count == 0;
    }
    private void OnMouseEnter()
    {
        if (CardController.Selected == null) return;
        if (gameObject.tag == "myCard") return;
        lighting.color = Color.red;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f));
    }
    private void OnMouseExit()
    {
        if (CardController.Selected == null) return;
        if (gameObject.tag == "myCard") return;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f, false));
    }
    private void OnMouseDown()
    {
        if (CardController.Selected == null) return;
        if(checkAttack())
        {
            //Card.Selected.attack(this);
        }
    }
    public Health GetHealth { get { return hp; } }
    void Death()
    {
        Time.timeScale = 0;
        if (gameObject.tag == "myCard") defeatWindow.gameObject.SetActive(true);
        else winWindow.gameObject.SetActive(true);
    }
}
