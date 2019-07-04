using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainButton : BaseButton
{
    private const float TEXT_RATE = 0.018f;

    //Values of properties

    public float height = 16f;
    public float widthNormal = 180f;
    public float widthHighlight = 480f;
    public float widthPressed = 1280f;
    public float speed = 2000f;

    //Values of components

    private Distort distort;
    private Image underline;

    //Values of states

    private float currentWidth;

    //Methods for focus and press events

    private void Update()
    {
        if (!isShown) return;

        if (isPressed)
        {
            if (currentWidth < widthPressed)
            {
                currentWidth += Time.unscaledDeltaTime * speed * 2f;
                if (currentWidth >= widthPressed)
                {
                    currentWidth = widthPressed;
                    SetPressed(false);
                    return;
                }
                underline.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth, height);
            }
            return;
        }

        if (isFocused)
        {
            if (currentWidth < widthHighlight)
            {
                currentWidth += Time.unscaledDeltaTime * speed;
                if (currentWidth >= widthHighlight) currentWidth = widthHighlight;
                underline.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth, height);
            }
        }
        else
        {
            if (currentWidth >= widthNormal)
            {
                currentWidth -= Time.unscaledDeltaTime * speed;
                if (currentWidth <= widthNormal)
                {
                    currentWidth = widthNormal;
                    underline.gameObject.SetActive(false);
                    return;
                }
                underline.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth, height);
            }
        }
    }

    public override void SetFocused(bool value)
    {
        if (value)
        {
            if (!isFocused)
            {
                if (ParentMenu != null)
                {
                    if (ParentMenu.LastFocused != this)
                    {
                        ParentMenu.LastFocused = this;
                        SoundManager.instance.PlayUiEfx(focusSound);
                    }
                }
                else
                {
                    SoundManager.instance.PlayUiEfx(focusSound);
                }

                if (controlGroup != null)
                {
                    if (currentFocused.ContainsKey(controlGroup))
                    {
                        currentFocused[controlGroup].SetFocused(false);
                    }
                    currentFocused.Add(controlGroup, this);
                }

                isFocused = true;
                distort.TurnOn();
                underline.gameObject.SetActive(true);
            }
        }
        else
        {
            if (isFocused)
            {
                if (IsControlFocusedInGroup())
                {
                    currentFocused.Remove(controlGroup);
                }

                isFocused = false;
                distort.TurnOff();
            }
        }
    }

    public override void SetPressed(bool value)
    {
        if (value)
        {
            if (!isPressed)
            {
                if (ParentMenu != null)
                {
                    ParentMenu.LastFocused = this;
                    ParentMenu.LastPressed = this;
                }
                SoundManager.instance.PlayUiEfx(pressSound);

                if (IsControlFocusedInGroup()) currentFocused[controlGroup].SetFocused(false);
                if (controlGroup != null)
                {
                    currentPressed.Add(controlGroup, this);
                }
                isPressed = true;
            }
        }
        else
        {
            if (isPressed)
            {
                if (ParentMenu != null && ParentMenu.LastPressed == this)
                    ParentMenu.LastPressed = null;

                if (IsControlPressedInGroup())
                {
                    currentPressed.Remove(controlGroup);
                    SetFocused(true);
                }
                isPressed = false;
            }
        }
    }

    //Methods for displaying

    public override void Init()
    {
        base.Init();

        distort = transform.Find("Distort").GetComponent<Distort>();
        underline = transform.Find("Underline").GetComponent<Image>();

        distort.TurnOff();
        currentWidth = widthNormal;
        underline.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth, height);
        underline.gameObject.SetActive(false);
        textName.text = string.Empty;
    }

    public override void FadeIn(float duration, float wait = 0f)
    {
        Init();
        StartCoroutine(Fade(0f, 1f, duration, wait));
        StartCoroutine(ShowText(wait));
    }
    
    IEnumerator ShowText(float wait = 0f)
    {
        float time = 0f;
        int count = 0;
        int randomCount = 0;
        int totalCount = controlName.Length;
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
                currentText.Append(controlName.Substring(0, count));
                currentText.Append(UIControl.GetRandomChar(true, true));
                textName.text = currentText.ToString();

                time -= TEXT_RATE;
                randomCount++;
                if (randomCount >= 3)
                {
                    randomCount = 0;
                    count++;
                }
            }
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        textName.text = controlName;
    }

    protected override void SetAlpha(float value)
    {
        base.SetAlpha(value);
        underline.SetAlpha(value);
    }
}