using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuController : MonoBehaviour
{
    [SerializeField] Transform Cards;
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
    public void sceneController(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
    public void ToggleWindow(Transform content)
    {
        if(coroutine!= null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(activator(content));
    }
    IEnumerator activator(Transform content)
    {
        bool enabled = !content.gameObject.activeSelf;
        Cards.gameObject.SetActive(!enabled);
        content.gameObject.SetActive(enabled);
        if(!enabled) Cards.GetComponent<Animator>().enabled = true;
        Vector3 pos = (enabled) ? cam.WorldToScreenPoint(Vector3.zero) : cam.WorldToScreenPoint(Vector3.down * 10);
        yield return StartCoroutine(Mover.MoveCard(content, pos, 0.5f));
        if(!enabled) content.gameObject.SetActive(false);
    }
    public void OnChangeNick(string value) => PlayerPrefs.SetString("Nick", value);
}
