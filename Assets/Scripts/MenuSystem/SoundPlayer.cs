using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup Mixer;
    public static UnityEvent<AudioClip> Play = new();
    private AudioSource source;
    [SerializeField] Slider soundSlider, musicSlider;
    void play(AudioClip clip) => source.PlayOneShot(clip);
    void Start()
    {
        source = GetComponent<AudioSource>();
        Play.AddListener(play);
        setCurentVolume(soundSlider);
        setCurentVolume(musicSlider);
    }
    private string mixerGroup;
    public string MixerGroup { set { mixerGroup = value; } }
    void setCurentVolume(Slider slider)
    {
        float t;
        Mixer.audioMixer.GetFloat(slider.name, out t);
        t = Mathf.InverseLerp(-80f, 0f, t);
        slider.value = t;
    }
    public void ChangeVolume(float volume)
    {
        Mixer.audioMixer.SetFloat(mixerGroup, Mathf.Lerp(-80, 0, volume));
    }
}
