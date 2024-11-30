using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class Mathmaking : NetworkBehaviour
{
    NetworkManager manager;
    [SerializeField] TMP_InputField field; // ������ �� ��� InputField - ��������� � ����������
    
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    public void HostStarting()
    {
        manager.networkAddress = field.text;
        if (manager.networkAddress == null)
        {
            manager.StartHost();
        }

        
    }
    public void ClientStarting()
    {
        manager.networkAddress = field.text;
        if (manager.networkAddress == null)
        {
            manager.StartClient();
        }
        
    }
}
