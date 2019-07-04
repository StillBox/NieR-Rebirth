using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DropDownButton : SubButton
{
    //Values of properties

    public string[] options;
    public int value;
    public UnityEvent onValueChanged;

    //Values of components

    [SerializeField] private Text current;

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
                mark.SetColor(lightColor);
                textName.SetColor(lightColor);
                current.SetColor(lightColor);
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
                mark.SetColor(darkColor);
                textName.SetColor(darkColor);
                current.SetColor(darkColor);
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
                mark.SetColor(lightColor);
                textName.SetColor(lightColor);
                pointer.SetPress();
                StartCoroutine(FadeSelection(1f, 0f, 0.1f));
                StartCoroutine(ChangeWidth(normalWidth, pressWidth));

                OptionsBox.Open(this);
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
                StartCoroutine(FadeSelection(0f, 1f, 0.1f));
                StartCoroutine(ChangeWidth(pressWidth, normalWidth));
            }
        }
    }

    public void SetValue(int value)
    {
        if (this.value != value)
        {
            this.value = value;
            onValueChanged.Invoke();
            current.text = options[this.value];
        }
    }

    //Methods for displaying

    public override void Init()
    {
        base.Init();

        current = transform.Find("Current").GetComponent<Text>();
        current.text = options[value];
        current.SetColor(darkColor);
    }

    protected override void SetAlpha(float alpha)
    {
        base.SetAlpha(alpha);
        current.SetAlpha(alpha);
    }

    IEnumerator FadeSelection(float beg, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(beg, end, time / duration);
            current.SetAlpha(alpha);
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        current.SetAlpha(end);
    }
}