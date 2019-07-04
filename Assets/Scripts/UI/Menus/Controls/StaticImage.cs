using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticImage : StaticControl
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public override void SetAlpha(float value)
    {
        image.SetAlpha(value);
    }
}
