using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobiusCruiser : Cruiser
{
    public float majorAmp = 4f;
    public float minorAmp = 1.5f;
    public float period = 6f;

    protected override Vector3 GetPosition(float time)
    {
        float phase = 2f * Mathf.PI * time / period;
        Vector3 position = new Vector3()
        {
            x = majorAmp * Mathf.Sin(phase),
            z = minorAmp * Mathf.Sin(phase * 2f)
        };
        return position;
    }
}
