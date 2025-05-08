using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateLobbyScreen : MonoBehaviour {
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private Toggle _timerToggle;
    [SerializeField] private TMP_Dropdown _firstTurnDropdown;
    //private bool isPrivate = false;

    private void Start() {

        SetOptions(_firstTurnDropdown, Enum.GetNames(typeof(Constants.FirstTurn)) );

        void SetOptions(TMP_Dropdown dropdown, IEnumerable<string> values) {
            dropdown.options = values.Select(type => new TMP_Dropdown.OptionData { text = type }).ToList();
        }
    }

    public static UnityEvent<LobbyData> LobbyCreated = new();

    public void OnCreateClicked() {
        var lobbyData = new LobbyData {
            Name = "None",/// name of player
            Password = _passwordInput.text,
            Timer = _timerToggle.isOn,
            FirstTurn = (Constants.FirstTurn)_firstTurnDropdown.value
        };

        LobbyCreated?.Invoke(lobbyData);
    }
    public void OnClickPassword(string value)
    {
        if(value == "")
            _passwordInput.text = String.Join("", new List<int>() { 0, 0, 0, 0 }
            .Select(digit => UnityEngine.Random.Range(0, 9)));
    }
}

public struct LobbyData {
    public string Name;
    public string Password;
    public bool Timer;
    public Constants.FirstTurn FirstTurn;
}