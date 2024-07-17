using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] Users botUser;
    [SerializeField] SpriteRenderer botIcon;
    [SerializeField] List<BasicBot> bots;
    [SerializeField] DeckController botDeck;
    [SerializeField] AudioSource MusicSource;
    public static UnityEvent onNextLevel = new();
    static int currentLevel;
    private void Start()
    {
        currentLevel = -1;
        onNextLevel.AddListener(nextLevel);
    }
    void nextLevel()
    {
        currentLevel++;
        if (currentLevel >= bots.Count) return;
        BasicBot bot = bots[currentLevel];
        botIcon.sprite = bot.icon;
        botUser.NewStats(bot.hp);
        botDeck.SetSet(bot.cardSet);
        MusicSource.clip = bot.music;
        MusicSource.Play();
    }
}
