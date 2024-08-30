using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] TMP_InputField JoinMathInput;
    [SerializeField] Button HostBtn;
    [SerializeField] Button ClientBtn;   
public void Host()
    {
        JoinMathInput.interactable = false;
        HostBtn.interactable = false;
        ClientBtn.interactable = false;

        //Client.localPlayer.HostGame();
    }

   public void Join()
    {
        JoinMathInput.interactable = false;
        HostBtn.interactable = false;
        ClientBtn.interactable = false;
    }
}
