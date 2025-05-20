using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject TutorialWindow;
    [SerializeField] Animator cardAnim;
    [SerializeField] TMP_InputField playerName;
    Camera cam;
    Coroutine coroutine;
    private void Start()
    {
        cam = Camera.main;
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
}
