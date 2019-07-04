using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShockText : MonoBehaviour
{
    [SerializeField, SetProperty("Title")]
    private string title = "TITLE";
    public string Title
    {
        get
        {
            return title;
        }
        set
        {
            title = value;
            transform.Find("Major").GetComponent<Text>().text = value;
            transform.Find("Minor").GetComponent<Text>().text = value;
        }
    }

    private Text major;
    private Text minor;

    private const float TEXT_RATE = 0.018f;

    void Start()
    {
        Init();
    }

    void Init()
    {
        major = transform.Find("Major").GetComponent<Text>();
        minor = transform.Find("Minor").GetComponent<Text>();
        major.text = title;
        minor.text = title;
        SetAlpha(0f);
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

    public void MainShock(float duration)
    {
        FadeIn(0.1f);
        StartCoroutine(RandomMove(major, 360f, 0f, duration));
        StartCoroutine(RandomMove(minor, 360f, 0f, duration));
        StartCoroutine(RandomSize(major, 40f, 0.2f, duration));
        StartCoroutine(RandomSize(minor, 40f, 0.1f, duration));
        FadeOut(0.2f, duration - 0.2f);
    }

    public void AfterShock(float duration)
    {
        FadeOut(0.1f, 0.2f);
        StartCoroutine(RandomMove(major, 32f, 0f, duration));
        StartCoroutine(RandomMove(minor, 16f, 0f, duration));
        StartCoroutine(RandomSize(major, 0f, 0.3f, duration));
        StartCoroutine(RandomSize(minor, 0f, 0.3f, duration));
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
    
    IEnumerator RandomMove(Text text, float amplitude, float gap, float duration)
    {
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        float time = 0f;
        while (time < duration)
        {
            float rate = 1f - time / duration;
            Vector2 position = new Vector2()
            {
                x = Random.Range(-1f, 1f) * amplitude * rate,
                y = Random.Range(-1f, 1f) * amplitude * rate * 0.5f
            };
            rectTransform.anchoredPosition = position;
            yield return null;
            time += Time.deltaTime;            
        }
    }

    IEnumerator RandomSize(Text text, float amplitude, float gap, float duration)
    {
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        float time = 0f;
        Vector3 scale = Vector3.one;
        while (time < duration)
        {
            float rate = 1f - time / duration;
            text.fontSize = (int)(160f + Random.Range(-2f, 1f) * amplitude * rate);
            rectTransform.localScale = scale;
            yield return new WaitForSeconds(gap);
            time += gap;
            scale.y = -scale.y;
        }
    }

    private void SetAlpha(float value)
    {
        major.SetAlpha(value * 0.8f);
        minor.SetAlpha(value * 0.5f);
    }
}