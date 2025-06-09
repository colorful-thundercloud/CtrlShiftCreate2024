using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject TutorialWindow;
    [SerializeField] Animator cardAnim;
    [SerializeField] TMP_InputField playerName;
    [SerializeField] TMP_Dropdown localeDropdown;
    Camera cam;
    Coroutine coroutine;
    private void Start()
    {
        cam = Camera.main;
        int key = PlayerPrefs.HasKey("Locale") ? PlayerPrefs.GetInt("Locale") : 0;
        ChangeLocale(key);
        StartCoroutine(LocaleDropdown());
    }
    public void Exit()
    {
        Application.Quit();
    }
    public async void MainMenu()
    {
        await SceneManager.LoadSceneAsync(0);
        NetworkManager.Singleton.Shutdown();
        await MatchmakingService.LeaveLobby();
    }
    public void MoveCamera(float x)
    {
        if(coroutine!= null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Mover.MoveCard(cam.transform, new Vector3(x, 0, -10), 0.6f));
    }
    public void ToggleWindow(Transform content)
    {
        bool enabled = !content.gameObject.activeSelf;
        content.gameObject.SetActive(enabled);
    }
    public void OnChangeNick(string value) => PlayerPrefs.SetString("Nick", value);
    public void Restart() => PlayerPrefs.DeleteAll();
    public void CheckTutorial()
    {
        if (PlayerPrefs.HasKey("Tutorial")) cardAnim.SetTrigger("Play");
        else TutorialWindow.SetActive(true);
    }
    public void Tutorial()
    {
        GetComponent<LobbyOrchestrator>().Single(new("Tutorial", true, 0));
        SceneManager.LoadSceneAsync("Tutorial");
    }
    public void Single()
    {
        GetComponent<LobbyOrchestrator>().Single(new("Bot", true, 0));
        GameManager.startGame(Random.Range(0, 2) == 0);
        SceneManager.LoadSceneAsync("Single");
    }
    bool localing = false;
    public void ChangeLocale(int id) { if (!localing) StartCoroutine(SetLocale(id)); }
    IEnumerator SetLocale(int id)
    {
        localing = true;
        localeDropdown.value = id;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        PlayerPrefs.SetInt("Locale", id);
        localing = false;
    }
    IEnumerator LocaleDropdown()
    {
        yield return LocalizationSettings.InitializationOperation;
        List<string> locales = LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.name).ToList();
        localeDropdown.options = locales.Select(locale => new TMP_Dropdown.OptionData { text = locale }).ToList();
    }
    public static string GetLocalizedString(string key) =>
        new LocalizedString { TableReference = "LocalTable", TableEntryReference = key }.GetLocalizedString();
}
