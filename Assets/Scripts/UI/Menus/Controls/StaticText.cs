using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticText : StaticControl
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public override void SetAlpha(float value)
    {
        text.SetAlpha(value);
    }
}