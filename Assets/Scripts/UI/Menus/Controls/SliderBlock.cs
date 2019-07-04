using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderBlock : Button
{
    private const float MIN_HEIGHT = 5f;
    private const float MAX_HEIGHT = 20f;
    private const float SPEED = 1500f;

    //Values of base properties

    public SliderButton Parent { set; get; }
    public SliderBar ParentBar { set; get; }
    public float value;

    //Values of base states

    public bool IsVisible { set; private get; }
    private float currentHeight = MIN_HEIGHT;

    //Values of components

    private Image block;

    //Methods for input events

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!ParentBar.IsActivated) return;
        
        base.OnPointerEnter(eventData);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (!ParentBar.IsActivated) return;

        if (value != Parent.value)
        {
            SoundManager.instance.PlayUiEfx(Parent.focusSound);
            Parent.SetValue(value);
        }
        base.OnSelect(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (ParentBar.IsActivated)
            {
                SoundManager.instance.PlayUiEfx(Parent.pressSound);
                Parent.SetPressed(false);
                EventSystem.current.SetSelectedGameObject(Parent.gameObject);
            }
            else
            {
                Parent.SetPressed(true);
            }
        }
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        if (ParentBar.IsActivated)
        {
            SoundManager.instance.PlayUiEfx(Parent.pressSound);
            Parent.SetPressed(false);
            EventSystem.current.SetSelectedGameObject(Parent.gameObject);
        }
        else
        {
            Parent.SetPressed(true);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        return;
    }
    
    //Methods for displaying and controlling

    public virtual void Init()
    {
        block = transform.Find("Image").GetComponent<Image>();

        currentHeight = MIN_HEIGHT;
        SetHeight(currentHeight);
    }

    public bool Maximize()
    {
        currentHeight += Time.unscaledDeltaTime * SPEED;
        bool isMax = currentHeight >= MAX_HEIGHT;
        if (isMax) currentHeight = MAX_HEIGHT;
        SetHeight(currentHeight);
        return isMax;
    }

    public bool Minimize()
    {
        currentHeight -= Time.unscaledDeltaTime * SPEED;
        bool isMin = currentHeight <= MIN_HEIGHT;
        if (isMin) currentHeight = MIN_HEIGHT;
        SetHeight(currentHeight);
        return isMin;
    }
    
    public void SetAlpha(float value)
    {
        block.SetAlpha(IsVisible ? value : 0f);
    }

    public void SetHeight(float value)
    {
        block.GetComponent<RectTransform>().sizeDelta = new Vector2(MIN_HEIGHT, Mathf.Clamp(value, MIN_HEIGHT, MAX_HEIGHT));
    }
}