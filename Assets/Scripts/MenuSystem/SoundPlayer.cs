using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : soundController
{
    private float musicVolue = 1f;
    public void Play(int index=0)
    {
        playSound(sound[index],musicVolue,true,1f,1f,1);
    }
    public void setVolume(float vol)
    {
        musicVolue = vol;
    }
    private void Update()
    {
        audioSRC.volume = musicVolue;
    }
}
