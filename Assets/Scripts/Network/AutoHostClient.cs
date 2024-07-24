using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    private void Start()
    {
        if (!Application.isBatchMode)
        {
            Debug.Log("Client conected");
            networkManager.StartClient();

        }
        else
        {
            Debug.Log("Server has bin axyel");
        }
    }
    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }
}
