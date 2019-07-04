using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class STBBaseInput : BaseInput
{
    public static STBBaseInput instance = null;

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public override float GetAxisRaw(string axisName)
    {
        return STBInput.GetAxisRaw(axisName);
    }
    
    public override bool GetButtonDown(string buttonName)
    {
        return STBInput.GetButtonDown(buttonName);
    }
}
