using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Stat
{
    [NonSerialized] public TMP_Text field;
    [NonSerialized] public UnityEvent<int> OnChange = new(); 
    public string Name;
    public int maxValue;
    public bool canBuff = false;
    [SerializeField] int value;
    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            if (field != null) field.text = $"{(Name == "value" ? "+" : "")}{this.value}";
            OnChange.Invoke(value);
        }
    }
    public void Show(bool enable)
    {
        if (field != null) field.transform.parent.gameObject.SetActive(enable);
    }
}
