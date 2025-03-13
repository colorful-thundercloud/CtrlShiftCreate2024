using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup Mixer;
    public static UnityEvent<AudioClip> Play = new();
    public static UnityEvent<float> ChangePitch = new();
    private AudioSource source;
    [SerializeField] Slider soundSlider, musicSlider;
    void play(AudioClip clip) => source.PlayOneShot(clip);
    void Awake()
    {
        source = GetComponent<AudioSource>();
        Play.AddListener(play);
        setCurentVolume(soundSlider);
        setCurentVolume(musicSlider);
        ChangePitch.AddListener(SetPitch);
    }
    private string mixerGroup;
    public string MixerGroup { set { mixerGroup = value; } }
    void setCurentVolume(Slider slider)
    {
        float t; 
        if(PlayerPrefs.HasKey(slider.name))
            t = PlayerPrefs.GetFloat(slider.name);
        else
        {
            Mixer.audioMixer.GetFloat(slider.name, out t);
            t = Mathf.InverseLerp(-80f, 0f, t);
        }
        slider.value = t;
    }
    public void ChangeVolume(float volume)
    {
        float t = Mathf.Lerp(-60, 0, volume);
        if (t == -60) t = -80;
        Mixer.audioMixer.SetFloat(mixerGroup, t);
        PlayerPrefs.SetFloat(mixerGroup, volume);
    }
    public void SetPitch(float pitch = 0)
    {
        source.pitch = pitch;
    }
    public void RandomPitch()
    {
        source.pitch = Random.Range(0.9f, 2f);
    }
}
