using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Users : MonoBehaviour
{
    [SerializeField] Field field;
    [SerializeField] int HP;
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
        //проверяем можно ли
    }
    private void OnMouseDown()
    {
        if (Field.SelectedCard == null) return;
        attackUser(Field.SelectedCard.Damage);
    }
    public void attackUser(int Damage)
    {
        if (checkAttack())
        {
            HP -= Damage;
            hpText.text = HP.ToString();
        }
    }
}
