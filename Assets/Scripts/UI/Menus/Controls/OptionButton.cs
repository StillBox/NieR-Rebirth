using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : BaseButton
{
    //Values of properties

    [HideInInspector] public int value;
    public Color darkColor = new Color();
    public Color lightColor = new Color();
    public Color pressColor = new Color();

    //Values of components
    
    protected Image front;
    protected ButtonPointer pointer;

    //Methods for focus and press events

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
                front.gameObject.SetActive(true);
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
                front.gameObject.SetActive(false);
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
                front.gameObject.SetActive(true);
                front.SetColor(pressColor);
                textName.SetColor(lightColor);
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

        front = transform.Find("Front").GetComponent<Image>();
        pointer = transform.Find("Pointer").GetComponent<ButtonPointer>();

        front.gameObject.SetActive(false);
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
    
    protected override void SetAlpha(float value)
    {
        base.SetAlpha(value);

        front.SetAlpha(value);
        pointer.SetAlpha(value);
    }

    public void SetWidth(float width)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        size.x = width;
        rectTransform.sizeDelta = size;
    }

    public void SetPosition(float x, float y)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(x, y);
    }
}