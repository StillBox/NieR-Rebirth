using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class STBInputModule : StandaloneInputModule
{
    protected override void Start()
    {
        inputOverride = STBBaseInput.instance;
    }
}