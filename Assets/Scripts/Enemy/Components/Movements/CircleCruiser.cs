using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCruiser : Cruiser
{
    public float radius = 3f;
    public float period = 6f;

    protected override Vector3 GetPosition(float time)
    {
        float rate = time / period;
        float phase = 2f * Mathf.PI * rate;
        Vector3 position = new Vector3()
        {
            x = (rate < 0.25f ? 0.5f - 0.5f * Mathf.Cos(2f * phase) : Mathf.Sin(phase)) * radius,
            z = (rate < 0.25f ? 0.5f * Mathf.Sin(2f * phase): Mathf.Cos(phase)) * radius
        };
        return position;
    }
}
