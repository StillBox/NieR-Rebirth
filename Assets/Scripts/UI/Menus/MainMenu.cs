using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    protected override void Update()
    {
        base.Update();

        if (IsActivated && STBInput.GetButtonDown("Cancel"))
        {
            SoundManager.instance.PlayUiEfx(UiEfx.CANCEL);
            MenuController.instance.OnMainMenuCancel();
        }
    }

    protected override void OnFadeIn(float duration)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].FadeIn(duration * 0.6f, duration * (0.6f - 0.2f * i));
        }
    }    
}