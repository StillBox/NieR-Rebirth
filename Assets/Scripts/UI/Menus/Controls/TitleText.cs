using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TitleText : MonoBehaviour
{
    private const float TEXT_RATE = 0.018f;

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
            GetComponent<Text>().text = value + (showInputSymbol ? "_" : "");
        }
    }
    public bool showInputSymbol = false;
    
    private Text text;

    void Start()
    {
        Init();
    }

    void Init()
    {
        text = GetComponent<Text>();
        text.text = string.Empty;
    }

    public void FadeIn(float duration, float wait = 0f)
    {
        Init();
        StartCoroutine(Fade(0f, 1f, duration, wait));
        StartCoroutine(ShowText(wait));
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

    IEnumerator ShowText(float wait = 0f)
    {
        SetText("");

        float time = 0f;
        int count = 0;
        int randomCount = 0;
        int totalCount = title.Length;

        while (time < wait)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        time -= wait;
        while (count < totalCount)
        {
            if (time > TEXT_RATE)
            {
                StringBuilder currentText = new StringBuilder();
                currentText.Append(title.Substring(0, count));
                currentText.Append(UIControl.GetRandomChar(false, true));
                if (showInputSymbol) currentText.Append("_");
                SetText(currentText.ToString());

                time -= TEXT_RATE;
                randomCount++;
                if (randomCount >= 2)
                {
                    randomCount = 0;
                    count++;
                }
            }
            yield return null;
            time += Time.unscaledDeltaTime;
        }

        SetText(title);
    }

    private void SetAlpha(float value)
    {
        text.SetAlpha(value);
    }

    private void SetText(string value)
    {
        text.text = value;
    }
}