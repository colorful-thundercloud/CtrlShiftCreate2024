using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class settingController : MonoBehaviour
{
    public TMP_Dropdown screenExtension;
    public void screenExtensionMenu()
    {
        if (screenExtension.value == 0)
        {
            Screen.SetResolution(720, 1080, true);
            Debug.Log("Расширь свое расширение на 720*1080");
        }
        if (screenExtension.value == 1)
        {
            Screen.SetResolution(1920, 1080, true);
            Debug.Log("Расширь свое расширение на 1920*1080");
        }
        if (screenExtension.value == 2)
        {
            Screen.SetResolution(2560, 1440, true);
            Debug.Log("Расширь свое расширение на 2560*1440");
        }
       
    }
    
}
