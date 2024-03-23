using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : soundController
{
    public void Play(int index)
    {
        playSound(sound[index],100f,true,1f,1f,1);
    }
}
