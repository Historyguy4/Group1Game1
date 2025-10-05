using System;
using System.Collections;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    [SerializeField] CanvasGroup cg;
    [SerializeField] bool StartFade;

    private float FadeTime = 2.5f;

    private void Awake()
    {
        if(StartFade)
        {
            cg.alpha = 1f;
            FadeOut();
        }
        else
        {
            cg.alpha = 0f;
        }
    }

    public void FadeIn(Action action = null)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 1f, FadeTime, action));
    }

    public void FadeOut(Action action = null)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0f, FadeTime, action));
    }

    private IEnumerator FadeRoutine(float start, float end, float duration, Action action = null)
    {
        float t = 0f;
        cg.alpha = start;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }

        cg.alpha = end;

        action?.Invoke();
    }
}
