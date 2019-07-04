using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutMenu : Menu
{
    protected override void Update()
    {
        base.Update();

        if (!IsActivated) return;

        if (Input.anyKeyDown)
        {
            SoundManager.instance.PlayUiEfx(UiEfx.CANCEL);
            MenuController.instance.BackToMainMenu(false);
        }
    }
}