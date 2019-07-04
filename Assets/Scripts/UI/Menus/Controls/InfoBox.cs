using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour
{
    public static InfoBox current = null;
    public static void SetInfo(string value)
    {
        if (current != null)
            current.info.text = value;
    }

    [SerializeField] private Image[] images;
    [SerializeField] private Text info;
    [SerializeField] private Text controlInfo;
    
    void Init()
    {
        current = this;
    }

    public void FadeIn(float duration, float wait = 0f)
    {
        Init();
        StartCoroutine(Fade(0f, 1f, duration, wait));
    }

    public void FadeOut(float duration, float wait = 0f)
    {
        StartCoroutine(Fade(1f, 0f, duration, wait));
    }

    IEnumerator Fade(float beg, float end, float duration, float wait = 0f)
    {
        SetAlpha(beg);

        float time = 0f;
        while (time < wait)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        time -= wait;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(beg, end, time / duration);
            SetAlpha(alpha);
            yield return null;
            time += Time.unscaledDeltaTime;
        }

        SetAlpha(end);
    }

    void SetAlpha(float alpha)
    {
        foreach (Image image in images)
        {
            image.SetAlpha(alpha);
        }
        info.SetAlpha(alpha);
        controlInfo.SetAlpha(alpha);
    }
}