using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] TMP_Text title, damage, hp, description, multiplier, value, steps;
    public static UnityEvent<CardController> OnOpenCard = new();
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

        showStat(damage, "damage", card);
        showStat(hp, "hp", card);
        showStat(multiplier, "multiplier", card);
        showStat(value, "value", card);
        showStat(steps, "steps", card);

        description.text = card.GetBasicCard.Description;
        gameObject.SetActive(true);
        hideUI = StartCoroutine(deactivate(4f));
    }
    void showStat(TMP_Text field, string name, CardController card)
    {
        field.transform.parent.gameObject.SetActive(card.GetStat(name) != null);
        field.text = card.GetStat(name)?.maxValue.ToString();
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
