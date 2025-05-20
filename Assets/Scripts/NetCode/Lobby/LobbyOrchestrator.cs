using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#pragma warning disable CS4014

/// <summary>
///     Lobby orchestrator. I put as much UI logic within the three sub screens,
///     but the transport and RPC logic remains here. It's possible we could pull
/// </summary>
public class LobbyOrchestrator : NetworkBehaviour {
    [SerializeField] private MainLobbyScreen _mainLobbyScreen;
    [SerializeField] private CreateLobbyScreen _createScreen;
    [SerializeField] private RoomScreen _roomScreen;
    [SerializeField] private TMP_InputField _playerName;

    private void Start() {
        if (PlayerPrefs.HasKey("Nick")) _playerName.text = PlayerPrefs.GetString("Nick");
        _createScreen.gameObject.SetActive(false);
        _roomScreen.gameObject.SetActive(false);

        CreateLobbyScreen.LobbyCreated.AddListener(CreateLobby);
        LobbyRoomPanel.LobbySelected.AddListener(OnLobbySelected);
        RoomScreen.LobbyLeft.AddListener(OnLobbyLeft);
        RoomScreen.StartPressed.AddListener(startPressed);
        
        NetworkObject.DestroyWithScene = true;
    }

    #region Main Lobby
    private async void OnLobbySelected(Lobby lobby) {

        try {
            await MatchmakingService.JoinLobbyWithAllocation(lobby.Id);

            _mainLobbyScreen.gameObject.SetActive(false);
            _roomScreen.gameObject.SetActive(true);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e) {
            Debug.LogError(e);
        }
    }

 

    #endregion

    #region Create

    private async void CreateLobby(LobbyData data) {
        try {
            data.Name = _playerName.text;
            await MatchmakingService.CreateLobbyWithAllocation(data);

            _createScreen.gameObject.SetActive(false);
            _roomScreen.gameObject.SetActive(true);

            // Starting the host immediately will keep the relay server alive
            NetworkManager.Singleton.StartHost();
        }
        catch (Exception e) {
            Debug.LogError(e);
        }
    }

    #endregion

    #region Room

    private readonly Dictionary<ulong, PlayerData> _playersInLobby = new();

    public struct PlayerData:INetworkSerializable
    {
        public string Name;
        public bool IsReady;
        public int SetId;
        public PlayerData(string name, bool isReady, int setId) { Name = name; IsReady = isReady; SetId = setId; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref SetId);
        }
    }

    public static UnityEvent<Dictionary<ulong, PlayerData>> LobbyPlayersUpdated = new();

    public override void OnNetworkSpawn() {
        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            var player = new PlayerData(_playerName.text, false, 0);
            _playersInLobby.Add(NetworkManager.Singleton.LocalClientId, player);
            UpdateInterface();
        }

        // Client uses this in case host destroys the lobby
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

 
    }

    //spawn in host
    private void OnClientConnectedCallback(ulong playerId) {
        Debug.Log($"player connectec: {playerId}");
        if (!IsServer) return;
        Debug.Log("spawn in host");
        // Add locally

        if (!_playersInLobby.ContainsKey(playerId))
            GetDataClientRpc(playerId);
    }
    [Rpc(SendTo.NotMe)]
    private void GetDataClientRpc(ulong playerId)
    {
        SendServerPlayerDataServerRpc(playerId, new PlayerData(_playerName.text, false, 0));
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendServerPlayerDataServerRpc(ulong playerId, PlayerData data)
    {
        _playersInLobby.Add(playerId, data);
        PropagateToClients();
        UpdateInterface();
    }

    private void PropagateToClients()
    {
        foreach (var player in _playersInLobby)
            UpdatePlayerClientRpc(player.Key, player.Value);
    }
    //spawn in client
    [ClientRpc]
    private void UpdatePlayerClientRpc(ulong clientId, PlayerData data)
    {
        if (IsServer) return;
        if (!_playersInLobby.ContainsKey(clientId))
            _playersInLobby.Add(clientId, data);
        else _playersInLobby[clientId] = data;
        UpdateInterface();
    }

    private void OnClientDisconnectCallback(ulong playerId) {
        if (IsServer) {
            // Handle locally
            if (_playersInLobby.ContainsKey(playerId)) _playersInLobby.Remove(playerId);

            // Propagate all clients
            RemovePlayerClientRpc(playerId);

            UpdateInterface();
        }
        else {
            // This happens when the host disconnects the lobby
            _roomScreen.gameObject.SetActive(false);
            _mainLobbyScreen.gameObject.SetActive(true);
            OnLobbyLeft();
        }
    }

    [ClientRpc]
    private void RemovePlayerClientRpc(ulong clientId) {
        if (IsServer) return;

        if (_playersInLobby.ContainsKey(clientId)) _playersInLobby.Remove(clientId);
        UpdateInterface();
    }

    public void OnReadyClicked() {
        SetReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyServerRpc(ulong playerId) {
        var data = _playersInLobby[playerId];
        data.IsReady = !data.IsReady;
        _playersInLobby[playerId] = data;
        PropagateToClients();
        UpdateInterface();
    }
    public void OnSetCardSetClicked(int id) => SetCardSetServerRpc(NetworkManager.Singleton.LocalClientId, id);
    [ServerRpc(RequireOwnership = false)]
    private void SetCardSetServerRpc(ulong playerId, int id) 
    {
        var data = _playersInLobby[playerId];
        data.SetId = id;
        _playersInLobby[playerId] = data;
        PropagateToClients();
        UpdateInterface();
    }

    private void UpdateInterface() {
        LobbyPlayersUpdated?.Invoke(_playersInLobby);
    }

    private async void OnLobbyLeft() {
        _playersInLobby.Clear();
        NetworkManager.Singleton.Shutdown();
        await MatchmakingService.LeaveLobby();
    }
    
    public override void OnDestroy() {
     
        base.OnDestroy();
        CreateLobbyScreen.LobbyCreated.RemoveListener(CreateLobby);
        LobbyRoomPanel.LobbySelected.RemoveListener(OnLobbySelected);
        RoomScreen.LobbyLeft.RemoveListener(OnLobbyLeft);
        RoomScreen.StartPressed.RemoveListener(startPressed);
        
        // We only care about this during lobby
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
      
    }

    private async void startPressed(string lobbyid) => await OnGameStart(lobbyid);
    private async Task OnGameStart(string lobbyId)
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        Lobby currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);

        Constants.FirstTurn turnType = (Constants.FirstTurn)int.Parse(currentLobby.Data[Constants.FirstTurnKey].Value);
        bool turn = (turnType == Constants.FirstTurn.Random) ? UnityEngine.Random.Range(0, 2) == 0 : turnType == Constants.FirstTurn.Host;
        bool timer = bool.Parse(currentLobby.Data[Constants.TimerKey].Value);

        StartGameClientRpc(_playersInLobby.Values.ToArray(), lobbyId, turn, timer);

        await Task.Delay((int)(1f * 1000));

        await MatchmakingService.LockLobby();
        NetworkManager.Singleton.SceneManager.LoadScene("ONLINE", LoadSceneMode.Single);
    }
    [ClientRpc]
    private void StartGameClientRpc(PlayerData[] players, string currentLobbyId, bool turn, bool timer)
    {
        _playersInLobby.Clear();
        Loading.OnStart.Invoke("Запускаем игру");
        PlayersInCurrentLobby = players.ToList();
        Timer.LobbySettings = timer;
        GameManager.startGame((IsServer) ? turn : !turn);
    }
    public void Single(PlayerData enemy) => PlayersInCurrentLobby = new() { enemy, new(_playerName.text, true, 0) };
    public static List<PlayerData> PlayersInCurrentLobby { get; private set; }
    #endregion
}