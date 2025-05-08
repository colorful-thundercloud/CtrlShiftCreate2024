using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Image timerImage;
    [SerializeField] AudioSource source;
    [SerializeField] float timerDelay = 30f;
    [SerializeField] UnityEvent OnEnd = new();
    Coroutine timerCoroutine;
    public static bool LobbySettings = true;
    private void Start()
    {
        timerImage.transform.parent.gameObject.SetActive(LobbySettings);
    }
    public void Set(bool active)
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (active && LobbySettings) timerCoroutine = StartCoroutine(timer());
        else
        {
            timerImage.fillAmount = 0;
            source.Stop();
        }
    }
    private IEnumerator timer()
    {
        float t = 0;
        bool lowTime = false;
        do
        {
            t += Time.deltaTime;
            timerImage.fillAmount = 1 - (t / timerDelay);
            if (t >= timerDelay * 0.75f && !lowTime)
            {
                source.Play();
                lowTime = true;
            }
            yield return new WaitForEndOfFrame();
        }
        while (t < timerDelay);
        source.Stop();
        OnEnd.Invoke();
    }
}
