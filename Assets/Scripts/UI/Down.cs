using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Down : MonoBehaviour,IPointerDownHandler
{
    public UnityEvent OnDown = new();
    [SerializeField] private AudioClip sound;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (sound != null) SoundPlayer.Play.Invoke(sound);
        OnDown.Invoke();
    }
}
