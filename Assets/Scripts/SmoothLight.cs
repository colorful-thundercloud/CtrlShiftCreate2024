using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SmoothLight
{
    public static IEnumerator smoothLight(Light2D light, float smoothTime, bool toLight = true)
    {
        float t = 0;
        while (t < smoothTime)
        {
            if (light == null) yield break;
            light.falloffIntensity = (toLight) ? Mathf.Lerp(1f, 0f, t / smoothTime) : Mathf.Lerp(0f, 1f, t / smoothTime);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    public static IEnumerator twinckle(Light2D light, float speed)
    {
        float t;
        bool toLight = true;
        while (true)
        {
            t = 0;
            while (t < speed)
            {
                if (light == null) yield break;
                light.falloffIntensity = (toLight) ? Mathf.Lerp(1f, 0f, t / speed) : Mathf.Lerp(0f, 1f, t / speed);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            toLight = !toLight;
        }
    }
}
