using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenuController : MonoBehaviour
{
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
}
