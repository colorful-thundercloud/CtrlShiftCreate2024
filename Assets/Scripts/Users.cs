using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Users : MonoBehaviour
{
    [SerializeField] Field field;
    [SerializeField] Light2D lighting;
    [SerializeField] int HP;
    public int Hp { get { return HP; } }
    [SerializeField] TMP_Text hpText;
    private void Start()
    {
        hpText.text = HP.ToString();
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
        attackUser(Field.SelectedCard.Damage);
        if(checkAttack())
        {
            Field.SelectedCard.turnOfLight();
            Field.SelectedCard = null;
        }
    }
    public void attackUser(int Damage)
    {
        if (checkAttack())
        {
            HP -= Damage;
            hpText.text = HP.ToString();
            StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f, false));
        }
    }
}
