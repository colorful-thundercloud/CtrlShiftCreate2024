using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    public GameObject hidenSran;
    public GameObject[] shlack;
    int count = 0;
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)&&count==0)
        {
            hidenSran.SetActive(true);
            count++;
            for (int i = 0; i < shlack.Length; i++)
            {
                shlack[i].SetActive(false);
            }
        }
        else 
        { 
            hidenSran.SetActive(false); 
            count = 0; 
        }
        
    }




}
