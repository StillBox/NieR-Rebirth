using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PositionAttribute
{
    public static Vector2 XZ(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public static float GetPositionX(this Transform transform)
    {
        return transform.position.x;
    }

    public static float GetPositionY(this Transform transform)
    {
        return transform.position.y;
    }

    public static float GetPositionZ(this Transform transform)
    {
        return transform.position.z;
    }

    public static float GetLocalPositionX(this Transform transform)
    {
        return transform.localPosition.x;
    }

    public static float GetLocalPositionY(this Transform transform)
    {
        return transform.localPosition.y;
    }

    public static float GetLocalPositionZ(this Transform transform)
    {
        return transform.localPosition.z;
    }

    public static float GetScaleX(this Transform transform)
    {
        return transform.lossyScale.x;
    }

    public static float GetScaleY(this Transform transform)
    {
        return transform.lossyScale.y;
    }

    public static float GetScaleZ(this Transform transform)
    {
        return transform.lossyScale.z;
    }

    public static float GetLocalScaleX(this Transform transform)
    {
        return transform.localScale.x;
    }

    public static float GetLocalScaleY(this Transform transform)
    {
        return transform.localScale.y;
    }

    public static float GetLocalScaleZ(this Transform transform)
    {
        return transform.localScale.z;
    }

    public static void SetPositionX(this Transform transform, float value)
    {
        Vector3 position = transform.position;
        position.x = value;
        transform.position = position;
    }

    public static void SetPositionY(this Transform transform, float value)
    {
        Vector3 position = transform.position;
        position.y = value;
        transform.position = position;
    }

    public static void SetPositionZ(this Transform transform, float value)
    {
        Vector3 position = transform.position;
        position.z = value;
        transform.position = position;
    }

    public static void SetLocalPositionX(this Transform transform, float value)
    {
        Vector3 position = transform.localPosition;
        position.x = value;
        transform.localPosition = position;
    }

    public static void SetLocalPositionY(this Transform transform, float value)
    {
        Vector3 position = transform.localPosition;
        position.y = value;
        transform.localPosition = position;
    }

    public static void SetLocalPositionZ(this Transform transform, float value)
    {
        Vector3 position = transform.localPosition;
        position.z = value;
        transform.localPosition = position;
    }
    
    public static void SetLocalScaleX(this Transform transform, float value)
    {
        Vector3 scale = transform.localScale;
        scale.x = value;
        transform.localScale = scale;
    }

    public static void SetLocalScaleY(this Transform transform, float value)
    {
        Vector3 scale = transform.localScale;
        scale.y = value;
        transform.localScale = scale;
    }

    public static void SetLocalScaleZ(this Transform transform, float value)
    {
        Vector3 scale = transform.localScale;
        scale.z = value;
        transform.localScale = scale;
    }
}

//Extensions for camera

public static class CameraExtension
{
    public static float AdjustAngle(this Camera camera, Vector3 position, Vector3 dir)
    {
        Vector3 worldDir = Matrix4x4.Rotate(Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f)) * dir;
        Vector3 worldEnd = position + worldDir;
        Vector3 screenEnd = Camera.main.WorldToScreenPoint(worldEnd);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);

        if (dir.z <= Mathf.Epsilon)
            return 0f;
        else
        {
            Vector3 adjustEnd = new Vector3()
            {
                x = screenPosition.x + dir.x / dir.z * (screenEnd.y - screenPosition.y),
                y = screenEnd.y,
                z = screenEnd.z
            };
            Vector3 adjustWorldEnd = Camera.main.ScreenToWorldPoint(adjustEnd);
            return Vector3.SignedAngle(dir, adjustWorldEnd - position, Vector3.up) - camera.transform.eulerAngles.y;
        }
    }
}

/// <summary>
/// Class for angel ranged from -180 to 180
/// </summary>
public class Angle
{
    private float value;
    public Angle(float angle)
    {
        value = angle;
        while (value < -180f) value += 360f;
        while (value > 180f) value -= 360f;
    }

    public static implicit operator float(Angle angle)
    {
        return angle.value;
    }

    public static implicit operator Angle(float angle)
    {
        return new Angle(angle);
    }

    public static Angle operator +(Angle lhs, Angle rhs)
    {
        return new Angle(lhs.value + rhs.value);
    }

    public static Angle operator -(Angle lhs, Angle rhs)
    {
        return new Angle(lhs.value - rhs.value);
    }
}

//Class for angel ranged from 0 to 180

public class UAngle
{
    private float value;
    public UAngle(float angle)
    {
        value = angle;
        while (value < 0f) value += 180f;
        while (value > 180f) value -= 180f;
    }

    public static implicit operator float(UAngle angle)
    {
        return angle.value;
    }

    public static implicit operator UAngle(float angle)
    {
        return new UAngle(angle);
    }

    public static UAngle operator +(UAngle lhs, UAngle rhs)
    {
        return new UAngle(lhs.value + rhs.value);
    }

    public static UAngle operator -(UAngle lhs, UAngle rhs)
    {
        return new UAngle(lhs.value - rhs.value);
    }
}

[System.Serializable]
public class Ladder
{
    public float halfWidth = 0f;
    public float topLeft = 0f;
    public float topRight = 0f;
    public float bottomLeft = 0f;
    public float bottomRight = 0f;

    public Ladder() { }

    public Ladder(float halfWidth, float topLeft, float topRight, float bottomLeft, float bottomRight)
    {
        this.halfWidth = halfWidth;
        this.topLeft = topLeft;
        this.topRight = topRight;
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
    }

    public float Top(float x)
    {
        return Mathf.Lerp(topLeft, topRight, (x / halfWidth + 1f) / 2f);
    }

    public float Bottom(float x)
    {
        return Mathf.Lerp(bottomLeft, bottomRight, (x / halfWidth + 1f) / 2f);
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        Vector2 closest = point;
        closest.x = Mathf.Clamp(closest.x, -halfWidth, halfWidth);
        closest.y = Mathf.Max(Bottom(closest.x), Mathf.Min(Top(closest.x), closest.y));
        return closest;
    }
}

/*
[System.Serializable]
public class Quad
{
    Vector2 topLeft = Vector2.zero;
    Vector2 topRight = Vector2.zero;
    Vector2 bottomLeft = Vector2.zero;
    Vector2 bottomRight = Vector2.zero;

    public Quad(float x1, float y1, float x2, float y2,
        float x3, float y3, float x4, float y4)
    {
        topLeft = new Vector2(x1, y1);
        topRight = new Vector2(x2, y2);
        bottomLeft = new Vector2(x3, y3);
        bottomRight = new Vector2(x4, y4);
    }

    public Quad(Vector2 node1, Vector2 node2, Vector2 node3, Vector2 node4)
    {
        topLeft = node1;
        topRight = node2;
        bottomLeft = node3;
        bottomRight = node4;
    }
}
*/