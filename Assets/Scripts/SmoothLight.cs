using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SmoothLight
{
    public static IEnumerator smoothLight(Light2D light, float smoothTime, bool toLight = true)
    {
        float t = 0;
        while (t < 1f)
        {
            light.falloffIntensity = (toLight) ? Mathf.Lerp(1f, 0f, t / smoothTime) : Mathf.Lerp(0f, 1f, t / smoothTime);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
