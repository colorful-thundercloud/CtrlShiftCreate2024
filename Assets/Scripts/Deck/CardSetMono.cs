using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSetMono : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] CardSet set;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetSelector.OnSelectSet.Invoke(set);
    }
}
