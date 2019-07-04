using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    private const float TEXT_RATE = 0.002f;

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
            GetComponent<Text>().text = value;
        }
    }

    private Text text;
    private bool isComplete = false;
    public bool IsComplete { get { return isComplete; } }

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
        StartCoroutine("ShowText", wait);
    }

    public void FadeOut(float duration, float wait = 0f)
    {
        StartCoroutine(Fade(1f, 0f, duration, wait));
    }

    public void Finish()
    {
        StopCoroutine("ShowText");
        SetText(title);
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
        isComplete = false;
        SetText("");
        yield return new WaitForSecondsRealtime(wait);

        float time = 0f;
        int count = 0;
        int totalCount = title.Length;
        
        while (count < totalCount)
        {
            if (time > TEXT_RATE)
            {
                StringBuilder currentText = new StringBuilder();
                currentText.Append(title.Substring(0, count));
                currentText.Append(UIControl.GetRandomChar(false, true));
                SetText(currentText.ToString());

                time -= TEXT_RATE;
                count++;
            }
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        
        isComplete = true;
        while (isComplete)
        {
            if (time > 0.6f) time -= 1.2f;
            StringBuilder currentText = new StringBuilder();
            currentText.Append(title);
            if (time >= 0f) currentText.Append(" _");
            SetText(currentText.ToString());
            yield return null;
            time += Time.unscaledDeltaTime;
        }
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
