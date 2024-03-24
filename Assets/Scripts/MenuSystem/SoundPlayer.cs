using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup Mixer;
    private static AudioSource source;
    public static void Play(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    private string mixerGroup;
    public string MixerGroup { set { mixerGroup = value; } }
    public void ChangeVolume(float volume)
    {
        Mixer.audioMixer.SetFloat(mixerGroup, Mathf.Lerp(-80, 0, volume));
    }
}
