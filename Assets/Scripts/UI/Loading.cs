using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Loading : MonoBehaviour
{
    [SerializeField] float FadingTime, FadedAlpha;
    [SerializeField] GameObject Fading;
    [SerializeField] Material mat;
    [SerializeField] TMP_Text textField;
    public static UnityEvent<string> OnStart = new();
    Coroutine coroutine;

    private void Awake()
    {
        OnStart.AddListener(Invoke);
        mat.color = new(1, 1, 1, 0);
    }
    public void FromLocale(string key) =>
        Invoke(MenuController.GetLocalizedString(key));
    private void Invoke(string text = "")
    {
        bool enabled = text != "";
        if (enabled && Fading != default) Fading.SetActive(true);
        if (coroutine!=default) StopCoroutine(coroutine);
        coroutine = StartCoroutine(fader(FadingTime, enabled));
        textField.text = text;
    }
    IEnumerator fader(float time, bool enabled)
    {
        float t = 0;
        Color c = mat.color;
        while (t < time)
        {
            c.a = enabled ? Mathf.Lerp(0, FadedAlpha, t / time) : Mathf.Lerp(FadedAlpha, 0, t / time);
            mat.color = c;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (!enabled && Fading != default) Fading.SetActive(false);
    }
    public void SwapText(TMP_Text text) => textField = text;
}
