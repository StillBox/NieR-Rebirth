using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticControlGroup : StaticControl
{
    [SerializeField] private StaticControl[] controls;

    public override void SetAlpha(float value)
    {
        foreach (StaticControl control in controls)
            control.SetAlpha(value);
    }
}
