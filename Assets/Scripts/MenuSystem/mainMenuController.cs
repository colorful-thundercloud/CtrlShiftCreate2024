using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuController : MonoBehaviour
{
    public GameObject[] hidenSran;
    public void Exit()
    {
        Application.Quit();
    }
    public void sceneController(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
    public void OnMouseEnter()
    {
        for (int i = 0; i < hidenSran.Length; i++) 
            hidenSran[i].SetActive(true);
    }
    public void OnMouseExit()
    {
        for (int i = 0; i < hidenSran.Length; i++) 
            hidenSran[i].SetActive(false);
    }
}
