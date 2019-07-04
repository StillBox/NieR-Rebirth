using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : Menu
{
    protected override void Update()
    {
        base.Update();

        if (IsActivated && Input.anyKeyDown)
        {
            buttons[0].SetPressed(true);
            MenuController.instance.OnAnyButtonDown();
        }
    }    
}
