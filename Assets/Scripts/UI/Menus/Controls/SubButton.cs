using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubButton : BaseButton
{
    //Values of properties

    public Color darkColor = new Color();
    public Color lightColor = new Color();
    public Color pressColor = new Color();
    public Color normalColor = new Color();
    public float normalWidth = 320f;
    public float pressWidth = 320f;
    public float speed = 1500f;

    //Values of components
    
    protected Image back;
    protected Image front;
    protected Image mark;
    protected ButtonPointer pointer;

    private float frontWidth;

    //Methods for focus and press events
    
    private void Update()
    {
        if (!isShown) return;
        
        frontWidth += (isFocused || isPressed ? 1f : -1f) * Time.unscaledDeltaTime * speed;
        frontWidth = Mathf.Clamp(frontWidth, 0f, isPressed ? pressWidth : normalWidth);
        SetFrontWidth(frontWidth);
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
                mark.SetColor(lightColor);
                textName.SetColor(lightColor);
                pointer.SetFocus();
                StartCoroutine(Blink(1f, 0.5f));
                InfoBox.SetInfo(description);
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
                mark.SetColor(darkColor);
                textName.SetColor(darkColor);
                pointer.SetNormal();
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
                front.SetColor(pressColor);
                mark.SetColor(lightColor);
                textName.SetColor(lightColor);
                pointer.SetPress();
                StartCoroutine(ChangeWidth(normalWidth, pressWidth));
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
                StartCoroutine(ChangeWidth(pressWidth, normalWidth));
            }
        }
    }

    //Methods for displaying

    public override void Init()
    {
        base.Init();
        
        back = transform.Find("Back").GetComponent<Image>();
        front = transform.Find("Front").GetComponent<Image>();
        mark = transform.Find("Mark").GetComponent<Image>();
        pointer = transform.Find("Pointer").GetComponent<ButtonPointer>();

        frontWidth = 0f;
        SetFrontWidth(frontWidth);
        mark.SetColor(darkColor);
        textName.SetColor(darkColor);
        pointer.SetNormal();
    }

    protected IEnumerator Blink(float period, float gap, float node = 0.7f)
    {
        float time = 0f;
        while (isFocused)
        {
            time += Time.unscaledDeltaTime;
            if (time >= period) time -= period + gap;
            float rate = Mathf.Clamp(time / period, 0f, 1f);
            if (rate < node)
            {
                front.SetColor(Color.Lerp(darkColor, pressColor, rate / node));
            }
            else if (rate > node)
            {
                front.SetColor(Color.Lerp(pressColor, darkColor, (rate - node) / (1f - node)));
            }
            yield return null;
        }
    }

    protected IEnumerator ChangeWidth(float beg, float end)
    {
        float duration = Mathf.Abs(end - beg) / speed;
        float time = 0f;
        while (time < duration)
        {
            SetWidth(Mathf.Lerp(beg, end, time / duration));
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        SetWidth(end);
    }

    protected override void SetAlpha(float alpha)
    {
        back.SetAlpha(alpha);
        front.SetAlpha(alpha);
        mark.SetAlpha(alpha);
        textName.SetAlpha(alpha);
        pointer.SetAlpha(alpha);
    }

    protected virtual void SetWidth(float width)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        size.x = width;
        rectTransform.sizeDelta = size;
    }

    protected virtual void SetFrontWidth(float width)
    {
        RectTransform rectTransform = front.GetComponent<RectTransform>();
        Vector2 size = rectTransform.offsetMax;
        size.x = width;
        rectTransform.offsetMax = size;
    }
}