using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] TMP_Text title, damage, hp, description;
    public static Action<BasicCard> OnOpenCard;
    private void Start()
    {
        OnOpenCard += ctx => openCard(ctx);
    }
    private void OnDestroy()
    {
        OnOpenCard -= openCard;
    }
    void openCard(BasicCard card)
    {
        if (image == null) return;
        image.sprite = card.GetAvatar;
        title.text = card.Title;
        damage.text = card.Damage.ToString();
        hp.text = card.HP.ToString();
        description.text = card.Description;
        gameObject.SetActive(true);
    }
}
