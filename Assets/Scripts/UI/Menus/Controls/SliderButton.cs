using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SliderButton : SubButton
{
    //Values of properties

    public float maxValue = 10f;
    public float value;
    public UnityEvent onValueChanged;

    //Values of components

    private SliderBar sliderBar;

    //Methods for input events

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!MenuController.IsActivated) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isPressed)
            {
                SoundManager.instance.PlayUiEfx(pressSound);
                SetPressed(false);
            }
            else
            {
                if (IsControlPressedInGroup()) return;
                base.OnPointerClick(eventData);
                SetPressed(true);
            }
        }
    }
    
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
                sliderBar.Activate();
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
                sliderBar.IsActivated = false;
                StartCoroutine(ChangeWidth(pressWidth, normalWidth));
            }
        }
    }

    public void SetValue(float value)
    {
        if (this.value != value)
        {
            this.value = value;
            onValueChanged.Invoke();
        }
    }

    //Methods for displaying

    public override void Init()
    {
        base.Init();

        sliderBar = transform.Find("SliderBar").GetComponent<SliderBar>();
        sliderBar.Init();
    }

    protected override void SetAlpha(float value)
    {
        base.SetAlpha(value);
        sliderBar.SetAlpha(value);
    }
}
