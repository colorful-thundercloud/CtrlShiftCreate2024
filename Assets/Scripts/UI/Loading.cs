using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] float FadingTime, FadedAlpha;
    [SerializeField] Image fade;
    [SerializeField] TMP_Text textField;
    public static UnityEvent<string> OnStart = new();
    Coroutine coroutine;

    private void Awake()
    {
        OnStart.AddListener(Invoke);
    }
    public void Invoke(string text = "")
    {
        bool enabled = text != "";
        if (enabled) fade.gameObject.SetActive(true);
        if (coroutine!=default) StopCoroutine(coroutine);
        coroutine = StartCoroutine(fader(FadingTime, enabled));
        textField.text = text;
    }
    IEnumerator fader(float time, bool enabled)
    {
        float t = 0;
        Color c = fade.color;
        while (t < time)
        {
            c.a = enabled ? Mathf.Lerp(0, FadedAlpha, t / time) : Mathf.Lerp(FadedAlpha, 0, t / time);
            fade.color = c;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (!enabled) fade.gameObject.SetActive(false);
    }
}
