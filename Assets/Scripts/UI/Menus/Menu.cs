using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    [SerializeField] protected TitleText[] titleTexts;
    [SerializeField] protected StaticText[] staticTexts;
    [SerializeField] protected StaticImage[] staticImages;
    [SerializeField] protected BaseButton[] buttons;

    [SerializeField] protected BaseButton defaultButton;

    public BaseButton LastFocused
    {
        set; get;
    }

    public BaseButton LastPressed
    {
        set; get;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
    
    public bool IsActivated
    {
        protected set;
        get;
    }

    public bool IsCurrentActive()
    {
        return IsActivated && MenuController.currentMenu == this;
    }

    virtual protected void Update()
    {
        if (!IsCurrentActive()) return;

        if (EventSystem.current.currentSelectedGameObject == null && LastFocused != null)
            EventSystem.current.SetSelectedGameObject(LastFocused.gameObject);
    }

    public void FadeIn(float duration)
    {
        SetActive(true);
        OnFadeIn(duration);
        SetDefault();
        StartCoroutine(Activate(duration));
    }

    public void FadeOut(float duration)
    {
        IsActivated = false;
        OnFadeOut(duration);
        StartCoroutine(Close(duration));
    }

    virtual protected void OnFadeIn(float duration)
    {
        foreach (TitleText titleText in titleTexts)
        {
            titleText.FadeIn(duration * 0.4f);
        }
        foreach (StaticText staticText in staticTexts)
        {
            staticText.FadeIn(duration * 0.4f);
        }
        foreach (StaticImage staticImage in staticImages)
        {
            staticImage.FadeIn(duration * 0.4f);
        }
        foreach (BaseButton button in buttons)
        {
            button.FadeIn(duration * 0.4f);
        }
    }

    virtual protected void OnFadeOut(float duration)
    {
        foreach (TitleText titleText in titleTexts)
        {
            titleText.FadeOut(duration * 0.6f);
        }
        foreach (StaticText staticText in staticTexts)
        {
            staticText.FadeOut(duration * 0.6f);
        }
        foreach (StaticImage staticImage in staticImages)
        {
            staticImage.FadeOut(duration * 0.6f);
        }
        foreach (BaseButton button in buttons)
        {
            button.FadeOut(duration * 0.6f);
        }
    }

    public void SetDefault()
    {
        if (defaultButton != null)
        {
            LastFocused = defaultButton;
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        }
    }

    virtual protected void OnActivated()
    {
        IsActivated = true;
    }

    virtual protected void OnClosed()
    {
        SetActive(false);
    }

    IEnumerator Activate(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        OnActivated();
    }

    IEnumerator Close(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        OnClosed();
    }
}