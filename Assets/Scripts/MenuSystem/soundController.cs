using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class soundController : MonoBehaviour
{
 public AudioClip[] sound;
    int count = 1;
    protected AudioSource audioSRC => GetComponent<AudioSource>();
    public void playSound(AudioClip clip,float Volume, bool destroed=false , float p1=0.85f,float p2=1.25f,int count=1)
    {
        for (int i = 0; i < count; i++)
        {
            audioSRC.pitch = Random.Range(p1, p2);
            if (destroed)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position);

            }
            audioSRC.PlayOneShot(clip);
        }
    }

}
