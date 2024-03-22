using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuController : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }
    public void sceneController(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
