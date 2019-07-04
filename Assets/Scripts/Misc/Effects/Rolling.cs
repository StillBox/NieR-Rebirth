using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolling : MonoBehaviour
{
    public enum RotateAxis
    {
        X, Y, Z
    }
    public RotateAxis axis = RotateAxis.Z;

    public float speed = 360f;

    private RectTransform thisTransform;

    private void Awake()
    {
        thisTransform = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        float deltaAngle = speed * Time.unscaledDeltaTime;
        Vector3 eulers = new Vector3()
        {
            x = axis == RotateAxis.X ? deltaAngle : 0f,
            y = axis == RotateAxis.Y ? deltaAngle : 0f,
            z = axis == RotateAxis.Z ? deltaAngle : 0f
        };
        if (thisTransform != null)
            thisTransform.Rotate(eulers);
        else
            transform.Rotate(eulers);
    }
}