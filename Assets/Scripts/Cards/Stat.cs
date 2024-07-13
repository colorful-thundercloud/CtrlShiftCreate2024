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
            OnChange.Invoke(value);
            if (field != null) field.text = this.value.ToString();
        }
    }
}
