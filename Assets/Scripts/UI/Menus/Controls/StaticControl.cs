using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticControl : MonoBehaviour
{
    public float maxAlpha = 1f;
    public bool autoHidden = true;

    protected void Start()
    {
        Init();
    }

    protected void Init()
    {
        SetAlpha(autoHidden ? 0f : 1f);
    }

    public void FadeIn(float duration, float wait = 0f)
    {
        Init();
        StartCoroutine(Fade(0f, maxAlpha, duration, wait));
    }

    public void FadeOut(float duration, float wait = 0f)
    {
        StartCoroutine(Fade(maxAlpha, 0f, duration, wait));
    }

    IEnumerator Fade(float beg, float end, float duration, float wait = 0f)
    {
        yield return new WaitForSecondsRealtime(wait);

        float time = 0f;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(beg, end, time / duration);
            SetAlpha(alpha);
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        SetAlpha(end);
    }

    public virtual void SetAlpha(float value) { }
}
