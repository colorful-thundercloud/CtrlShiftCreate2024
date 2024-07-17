using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mover
{
    public static IEnumerator SmoothSizeChange(Vector3 targetScale, Transform transform, float smoothTime)
    {
        float t = 0;
        float scale;
        float start = transform.localScale.x;
        while (transform.localScale != targetScale)
        {
            scale = Mathf.Lerp(start, targetScale.x, t / smoothTime);
            transform.localScale = new Vector3(scale, scale, targetScale.z);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            if (transform == null) yield break;
        }
    }
    public static IEnumerator MoveCard(CardController card, Vector3 targetPosition, float time)
    {
        float t = 0;
        Vector3 start = card.transform.position;
        while (card.transform.position != targetPosition)
        {
            card.transform.position = Vector3.Lerp(start, targetPosition, t / time);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
