using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject hidenSran;
    private void OnMouseDown()
    {
        hidenSran.SetActive(true);
    }
    private void OnMouseUp()
    {
        hidenSran.SetActive(false);
    }
}
