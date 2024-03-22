using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSustem : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private float musicVolue = 1f;
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        m_AudioSource.volume = musicVolue;
    }

    public void setVolume(float vol)
    {
        musicVolue = vol;
    }
}
