using System;
using System.Collections;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

public class LobbyRoomPanel : MonoBehaviour {
    [SerializeField] private TMP_Text _nameText, _firstTurn, _text;
    [SerializeField] private Transform _lock;
    [SerializeField] private TMP_InputField _passwordInput;
    private bool isPrivate = false;
    public Lobby Lobby { get; private set; }

    public static UnityEvent<Lobby> LobbySelected = new();

    public void Init(Lobby lobby) {
        UpdateDetails(lobby);
    }

    public void UpdateDetails(Lobby lobby) {
        Lobby = lobby;
        _nameText.text = lobby.Name;
        _firstTurn.text = $"Первый ход: {(Constants.FirstTurn)GetValue(Constants.FirstTurnKey)}";
        isPrivate = lobby.Data[Constants.PasswordKey].Value != "";
        _lock.gameObject.SetActive(isPrivate);

        int GetValue(string key) {
            return int.Parse(lobby.Data[key].Value);
        }
    }

    public void Clicked() {
        if (!isPrivate) join();
        else
        {
            _nameText.gameObject.SetActive(false);
            _firstTurn.gameObject.SetActive(false);
            _passwordInput.gameObject.SetActive(true);
        }
    }
    public void OnPasswordChange(string value)
    {
        if (Lobby.Data[Constants.PasswordKey].Value == value)
        {
            StartCoroutine(SetColor(Color.green, 0.5f));
            join();
        }
        else if (value.Length == 4) StartCoroutine(SetColor(Color.red, 0.5f));
    }
    private void join()
    {
        LobbySelected?.Invoke(Lobby);

    }
    private IEnumerator SetColor(Color color, float time)
    {
        _text.color = color;
        yield return new WaitForSeconds(time);
        _text.color = Color.white;
    }
}