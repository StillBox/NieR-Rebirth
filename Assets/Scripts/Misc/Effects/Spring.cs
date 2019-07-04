using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Vector3 origin;
    public float offset;
    public float period = 0.4f;
    public float gap = 0.4f;

    private float timeUpdate;

    void Start()
    {
        timeUpdate = 0f;
    }

    void Update()
    {
        timeUpdate += Time.deltaTime;
        if (timeUpdate > period)
        {
            timeUpdate -= period + gap;
        }
        if (timeUpdate > 0f)
        {
            float rate = 1f - 2f * Mathf.Abs(timeUpdate / period - 0.5f);
            float h = offset * rate;
            Vector3 position = origin;
            position.y -= h;
            transform.position = position;
        }
    }
}