using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
[DisallowMultipleComponent]
public class CustomDropdown : MonoBehaviour, IPointerClickHandler
{
    public List<int> enabledOptions = new ();

    public void OnPointerClick(PointerEventData eventData)
    {
        var dropDownList = GetComponentInChildren<Canvas>();
        if (!dropDownList) return;

        var toggles = dropDownList.GetComponentsInChildren<Toggle>(true);
        foreach (var toggle in toggles) toggle.interactable = false;
        foreach (int id in enabledOptions) toggles[id+1].interactable = true;
        toggles[1].interactable = true;
    }
}