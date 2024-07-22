using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disconect : MonoBehaviour
{
    public void StopServer()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
    }
}
