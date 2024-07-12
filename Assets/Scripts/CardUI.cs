using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] TMP_Text title, damage, hp, description;
    public static UnityEvent<CardController> OnOpenCard;
    Coroutine hideUI;
    private void Start()
    {
        OnOpenCard.AddListener(openCard);
        hideUI = StartCoroutine(deactivate(0f));
    }
    void openCard(CardController card)
    {
        if (image == null) return;
        if (hideUI != null) StopCoroutine(hideUI);
        image.sprite = card.GetBasicCard.GetAvatar;
        title.text = card.GetBasicCard.Title;
        damage.text = card.GetStat("damage").maxValue.ToString();
        hp.text = card.GetStat("hp").maxValue.ToString();
        description.text = card.GetBasicCard.Description;
        gameObject.SetActive(true);
        hideUI = StartCoroutine(deactivate(4f));
    }
    IEnumerator deactivate(float time)
    {
        while(time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }
}
