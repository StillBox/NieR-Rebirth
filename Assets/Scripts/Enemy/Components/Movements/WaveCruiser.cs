using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCruiser : Cruiser
{
    public float majorAmp = 3f;
    public float minorAmp = 2f;
    public float period = 10f;

    protected override Vector3 GetPosition(float time)
    {
        float phase = 2f * Mathf.PI * time / period;
        Vector3 position = new Vector3()
        {
            x = majorAmp * Mathf.Sin(phase),
            z = minorAmp * Mathf.Sin(phase * 4f)
        };
        return position;
    }
}
