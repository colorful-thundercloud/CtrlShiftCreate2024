using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Client : NetworkBehaviour
{

    public static Client localPlayer;


    private void Start()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
    }
   /* public void HostGame()
    {
        //string MathId = MathMaker.GetID();
        //CmdHostGame(MathId);
    }*/
    //запуск на сервере 
    [Command]
     void CmdHostGame(string mathId)
     {
         /*if (MathMaker.instance.HostGame(mathId,gameObject))
         {
             Debug.Log("Хост запущен ");
         }
         else
         {

             Debug.Log($"<color=red>Хост попущен</color>") ;

         }*/
     }
}

