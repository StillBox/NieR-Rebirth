using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseButton : Button
{
    //Values and methods for parent menu or controls

    public Menu ParentMenu
    {
        set; get;
    }

    //Values of base properties

    [SerializeField, SetProperty("ControlName")]
    protected string controlName = "Name";
    public string ControlName
    {
        get
        {
            return controlName;
        }
        set
        {
            controlName = value;
            transform.Find("Name").GetComponent<Text>().text = value;
        }
    }

    public string controlGroup = null;
    public string description = "No Description";
    public UiEfx focusSound = UiEfx.SELECT;
    public UiEfx pressSound = UiEfx.DECIDE_1;

    //Values and methods for current focused and pressed button in each group

    protected static Dictionary<string, BaseButton> currentFocused = new Dictionary<string, BaseButton>();
    protected static Dictionary<string, BaseButton> currentPressed = new Dictionary<string, BaseButton>();

    protected bool IsControlFocusedInGroup()
    {
        if (controlGroup == null)
            return false;
        else
            return currentFocused.ContainsKey(controlGroup);
    }

    public void ResetAllFocused()
    {
        currentFocused = new Dictionary<string, BaseButton>();
    }

    protected bool IsControlPressedInGroup()
    {
        if (controlGroup == null)
            return false;
        else
            return currentPressed.ContainsKey(controlGroup);
    }

    public void ResetAllPressed()
    {
        currentPressed = new Dictionary<string, BaseButton>();
    }

    //Values of base components

    protected Text textName;

    //Values of base states

    protected bool isShown = false;
    protected bool isFocused = false;
    protected bool isPressed = false;

    //Methods for input events
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (IsControlPressedInGroup()) return;

        base.OnPointerEnter(eventData);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (IsControlPressedInGroup()) return;

        base.OnSelect(eventData);
        SetFocused(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!ParentMenu.IsActivated) return;
        if (IsControlPressedInGroup()) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerClick(eventData);
            SetPressed(true);
        }
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        if (!ParentMenu.IsActivated) return;
        if (IsControlPressedInGroup()) return;

        base.OnSubmit(eventData);
        SetPressed(true);
    }
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        return;
    }

    //Methods for focus and press events

    public virtual void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public virtual void SetFocused(bool value)
    {
        isFocused = value;
    }

    public virtual void SetPressed(bool value)
    {
        isPressed = value;
    }

    //Methods for displaying

    public virtual void Init()
    {
        ParentMenu = GetComponentInParent<Menu>();

        if (IsControlFocusedInGroup()) currentFocused.Remove(controlGroup);
        if (IsControlPressedInGroup()) currentPressed.Remove(controlGroup);

        textName = transform.Find("Name").GetComponent<Text>();
        textName.text = controlName;

        isShown = false;
        isFocused = false;
        isPressed = false;
    }

    public virtual void FadeIn(float duration, float wait = 0f)
    {
        Init();
        StartCoroutine(Fade(0f, 1f, duration, wait));
    }

    public virtual void FadeOut(float duration, float wait = 0f)
    {
        StartCoroutine(Fade(1f, 0f, duration, wait));
    }

    protected IEnumerator Fade(float beg, float end, float duration, float wait = 0f)
    {
        SetAlpha(beg);

        float time = 0f;
        while (time < wait)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        time -= wait;
        if (beg < end) isShown = true;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(beg, end, time / duration);
            SetAlpha(alpha);
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        if (beg > end) isShown = false;

        SetAlpha(end);
    }

    protected virtual void SetAlpha(float value)
    {
        textName.SetAlpha(value);
    }
}
