using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelController : NetworkBehaviour
{
    [SerializeField] TMP_Text playerNameField, enemyNameField;
    [SerializeField] Users botUser;
    [SerializeField] SpriteRenderer botIcon, BG, HandBG;
    [SerializeField] List<BasicBot> bots;
    [SerializeField] DeckController botDeck;
    [SerializeField] AudioSource MusicSource;
    public static UnityEvent onNextLevel = new();
    static int currentLevel;
    private void Start()
    {
        currentLevel = -1;
        //onNextLevel.AddListener(nextLevel);
        playerNameField.text = LobbyOrchestrator.PlayersInCurrentLobby[IsServer ? 0 : 1].Name;
        enemyNameField.text = LobbyOrchestrator.PlayersInCurrentLobby[IsServer ? 1 : 0].Name;
    }
    void nextLevel()
    {
        currentLevel++;
        if (currentLevel >= bots.Count) return;
        BasicBot bot = bots[currentLevel];
        botIcon.sprite = bot.icon;
        BG.sprite = bot.BG;
        HandBG.sprite = bot.HandBG;
        botUser.NewStats(bot.hp);
        botDeck.SetSet(bot.cardSet);
        MusicSource.clip = bot.music;
        MusicSource.Play();
    }
}
