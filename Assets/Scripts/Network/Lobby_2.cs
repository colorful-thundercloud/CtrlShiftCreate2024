using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lobby_2 : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton= null;

    public static string DisplayName { get; private set; }

    private const string NameKey = "PlayerName";

    private void Start()
    {
        SetUpInputField();
    }
    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(NameKey)) { return; }
        string defaultName= PlayerPrefs.GetString(NameKey);
        nameInputField.text = defaultName;
        setPlayerName(defaultName);
        
    }
    public void setPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }
    public void SavePlayerName()
    {
        DisplayName = nameInputField.name;
        PlayerPrefs.SetString(NameKey, DisplayName);
    }

}

