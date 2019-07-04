using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private const float TEXT_RATE = 0.018f;

    [SerializeField] private Text textName;
    [SerializeField] private Image underline;
    [SerializeField] private Image back;
    [SerializeField] private Image front;

    public Transform anchor;
    public string character;
    public Vector2 offset;
    public float width;
    public float hp;

    private bool isReady;
    private float currentHp;

    //For Fade & Auto Fade

    public bool autoFade = false;
    public float fadeTime = 5f;

    private float timer = 0f;
    private float alpha = 0f;
    private bool isShown = false;
    private Coroutine fadeCoroutine = null;

    public bool IsAutoFaded
    {
        get { return timer <= 0f; }
    }

    void Start()
    {
        SetAlpha(0f);
        timer = fadeTime;
    }

    void Update()
    {
        if (isReady)
        {
            if (currentHp != hp)
            {
                float delta = hp - currentHp;
                float step = Time.deltaTime / 0.5f;
                if (Mathf.Abs(delta) > step)
                    delta = Mathf.Sign(delta) * step;
                currentHp += delta;
                SetHPBar(currentHp);
            }

            if (autoFade && timer > 0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    FadeOut(0.2f);
                }
            }
        }
    }

    public bool IsActive
    {
        get { return gameObject.activeSelf; }
        set { SetActive(value); }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetSize(float value)
    {
        width = Screen.width * value;
    }

    public void SetPosition(Vector2 offset)
    {
        SetPosition(offset.x, offset.y);
    }

    public void SetPosition(float x, float y)
    {
        Vector2 position = Vector2.zero;
        position.x = x > 0f ? (x - 0.5f) * Screen.width : (x + 0.5f) * Screen.width;
        position.y = y > 0f ? (y - 0.5f) * Screen.height : (y + 0.5f) * Screen.height;
        GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void Show()
    {
        SetActive(true);
        currentHp = 0f;
        SetHPBar(currentHp);
        FadeIn(0.2f);
        StartCoroutine(ShowText(0.2f));
        StartCoroutine(ChangeWidth(16f, width, 0.5f, 0.2f));
    }

    public void Hide()
    {
        SetActive(true);
        StartCoroutine(ChangeWidth(width, 16f, 0.5f));
        FadeOut(0.2f, 0.2f);
    }

    public void FadeIn(float duration, float wait = 0f)
    {
        if (isShown) return;
        isShown = true;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(1f, duration, wait));
    }

    public void FadeOut(float duration, float wait = 0f)
    {
        if (!isShown) return;
        isShown = false;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(0f, duration, wait));
    }

    public void SetHP(float value)
    {
        if (hp != value)
        {
            hp = value;
            timer = fadeTime;
            FadeIn(0.2f);
        }
    }

    IEnumerator Fade(float target, float duration, float wait = 0f)
    {
        float time = 0f;
        float origin = alpha;
        while (time < wait)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        time -= wait;
        while (time < duration)
        {
            alpha = Mathf.Lerp(origin, target, time / duration);
            SetAlpha(alpha);
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        SetAlpha(target);
    }

    IEnumerator ChangeWidth(float beg, float end, float duration, float wait = 0f)
    {
        SetWidth(beg);
        
        float time = 0f;
        while (time < wait)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        time -= wait;
        while (time < duration)
        {
            SetWidth(Mathf.Lerp(beg, end, time / duration));
            yield return null;
            time += Time.unscaledDeltaTime;
        }

        SetWidth(end);
        isReady = beg < end;
    }

    IEnumerator ShowText(float wait = 0f)
    {
        SetText("");

        float time = 0f;
        int count = 0;
        int randomCount = 0;
        int totalCount = character.Length;

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
                currentText.Append(character.Substring(0, count));
                currentText.Append(UIControl.GetRandomChar(false, true));
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

        SetText(character);
    }

    private void SetHPBar(float value)
    {
        front.fillAmount = value;
    }

    private void SetWidth(float value)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        size.x = value;
        rectTransform.sizeDelta = size;
    }

    private void SetAlpha(float value)
    {
        textName.SetAlpha(value);
        underline.SetAlpha(value);
        back.SetAlpha(value);
        front.SetAlpha(value);
    }

    private void SetText(string value)
    {
        textName.text = value;
    }
}