using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModeMenu : Menu
{
    [SerializeField] private DropDownButton difficulty;
    [SerializeField] private DropDownButton screenGrid;
    [SerializeField] private DropDownButton rgbChannel;
    [SerializeField] private DropDownButton fadeBorder;
    [SerializeField] private DropDownButton directionAdjust;
    
    protected override void Update()
    {
        if (!IsCurrentActive()) return;

        base.Update();

        if (STBInput.GetButtonDown("Cancel"))
        {
            SoundManager.instance.PlayUiEfx(UiEfx.CANCEL);
            if (LastPressed == null)
            {
                MenuController.instance.BackToOptionsMenu();
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                LastPressed.SetPressed(false);
                EventSystem.current.SetSelectedGameObject(LastFocused.gameObject);
            }
        }
    }

    protected override void OnFadeIn(float duration)
    {
        difficulty.value = GameManager.gameLevel;
        screenGrid.value = GameManager.screenGridOn;
        rgbChannel.value = GameManager.rgbChannelOn;
        fadeBorder.value = GameManager.fadeBorderOn;
        directionAdjust.value = GameManager.directionAdjustOn;

        foreach (StaticImage staticImage in staticImages)
        {
            staticImage.FadeIn(duration * 0.6f);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].FadeIn(duration * 0.6f, duration * 0.15f * i);
        }
    }
    
    //Mode Options Menu Controls

    public void OnDifficultyChanged()
    {
        GameManager.SetLevel(difficulty.value);
    }

    public void OnScreenGridChanged()
    {
        GameManager.SetScreenGridOn(screenGrid.value);
    }

    public void OnRGBChannelChanged()
    {
        GameManager.SetRGBChannelOn(rgbChannel.value);
    }

    public void OnFadeBorderChanged()
    {
        GameManager.SetFadeBorderOn(fadeBorder.value);
    }

    public void OnDirectionAdjustChanged()
    {
        GameManager.SetDirectionAdjustOn(directionAdjust.value);
    }
}