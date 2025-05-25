using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomScreen : MonoBehaviour {
    [SerializeField] private float _startingTime = 3;
    [SerializeField] private LobbyPlayerPanel _playerPanelPrefab;
    [SerializeField] private Transform _playerPanelParent;
    [SerializeField] private TMP_Text _waitingText, _passwordField, _firstTurn;
    [SerializeField] private TMP_Dropdown SetChanger;
    [SerializeField] private Toggle _readyButton;
    private bool allReady = false;
    private string lobbyId;

    private readonly List<LobbyPlayerPanel> _playerPanels = new();

    public static UnityEvent<string> StartPressed = new(); 

    private void OnEnable() {
        Loading.OnStart.Invoke("");
        _readyButton.isOn = false;

        List<CardSet> sets = Resources.LoadAll<CardSet>("Sets").ToList();
        sets.Sort((x, y) => x.Price.CompareTo(y.Price));
        SetChanger.options = sets.Select(set => new TMP_Dropdown.OptionData { text = set.Name, image = set.Icon }).ToList();

        List<string> MySets = PlayerPrefs.HasKey("MySets") ? PlayerPrefs.GetString("MySets").Split(", ").ToList() : new();
        SetChanger.gameObject.GetComponent<CustomDropdown>().enabledOptions = MySets.Select(MySet => sets.FindIndex(set => set.name == MySet)).ToList();

        foreach (Transform child in _playerPanelParent) Destroy(child.gameObject);
        _playerPanels.Clear();

        LobbyOrchestrator.LobbyPlayersUpdated.AddListener(NetworkLobbyPlayersUpdated);
        MatchmakingService.CurrentLobbyRefreshed.AddListener(OnCurrentLobbyRefreshed);
        SetChanger.value = 0;
    }

    private void OnDisable() {
        LobbyOrchestrator.LobbyPlayersUpdated.RemoveListener(NetworkLobbyPlayersUpdated);
        MatchmakingService.CurrentLobbyRefreshed.RemoveListener(OnCurrentLobbyRefreshed);
    }

    public static UnityEvent LobbyLeft = new();

    public void OnLeaveLobby() {
        LobbyLeft?.Invoke();
    }
    Coroutine starting;
    private void NetworkLobbyPlayersUpdated(Dictionary<ulong, LobbyOrchestrator.PlayerData> players) {
        var allActivePlayerIds = players.Keys;

        // Remove all inactive panels
        var toDestroy = _playerPanels.Where(p => !allActivePlayerIds.Contains(p.PlayerId)).ToList();
        foreach (var panel in toDestroy) {
            _playerPanels.Remove(panel);
            Destroy(panel.gameObject);
        }

        foreach (var player in players) {
            var currentPanel = _playerPanels.FirstOrDefault(p => p.PlayerId == player.Key);
            if (currentPanel != null) {
                currentPanel.SetReady(player.Value.IsReady);
            }
            else {
                var panel = Instantiate(_playerPanelPrefab, _playerPanelParent);
                panel.Init(player.Key, player.Value);
                _playerPanels.Add(panel);
            }
        }
        allReady = players.All(p => p.Value.IsReady);

        if (!NetworkManager.Singleton.IsHost) return;
        if (allReady && players.Count == 2)
            starting = (starting == default) ? StartCoroutine(timer()) : default;
        else if (starting != default) StopCoroutine(starting);
    }

    private void OnCurrentLobbyRefreshed(Lobby lobby) {
        lobbyId = lobby.Id;
        _waitingText.text = MenuController.GetLocalizedString(
            (lobby.Players.Count < 2) ? "WaitingSecond" : (allReady) ? "WaitingStart" : "WaitingReady");
        string password = lobby.Data[Constants.PasswordKey].Value;
        _passwordField.gameObject.SetActive(password != "");
        _passwordField.text = $"{MenuController.GetLocalizedString("Code")}: {password}";
        _firstTurn.text = $"{(Constants.FirstTurn)int.Parse(lobby.Data[Constants.FirstTurnKey].Value)}";
    }
    public void OnReadyClicked()
    {
        SetChanger.interactable = !_readyButton.isOn;
    }
    IEnumerator timer()
    {
        yield return new WaitForSeconds(_startingTime);
        StartPressed?.Invoke(lobbyId);
    }
}