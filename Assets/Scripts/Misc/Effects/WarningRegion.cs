using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningRegion : MonoBehaviour
{
    [SerializeField, SetProperty("BeginPoint")]
    private Vector3 beginPoint = Vector3.zero;
    public Vector3 BeginPoint
    {
        get { return beginPoint; }
        set
        {
            beginPoint = value;
            ApplyPoints();
        }
    }

    [SerializeField, SetProperty("EndPoint")]
    private Vector3 endPoint;
    public Vector3 EndPoint
    {
        get { return endPoint; }
        set
        {
            endPoint = value;
            ApplyPoints();
        }
    }

    private void ApplyPoints()
    {
        Vector3 direction = endPoint - beginPoint;
        float length = direction.magnitude + 1f;
        transform.SetLocalScaleY(length * 0.5f);
        transform.position = 0.5f * (beginPoint + endPoint);
        transform.rotation = direction.magnitude <= Mathf.Epsilon ?
            Quaternion.identity : Quaternion.LookRotation(direction);
        transform.Rotate(90f, 0f, 0f);
    }

    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    public void Warn(float duration)
    {
        StartCoroutine(ChangeScale(0f, 1f, 1f));
        StartCoroutine(Twinkle(duration));
    }

    IEnumerator ChangeScale(float beg, float end, float duration)
    {
        transform.SetLocalScaleX(beg);
        transform.SetLocalScaleZ(beg);
        float time = 0f;
        while (time <duration)
        {
            float rate = time / duration;
            float scale = Mathf.Lerp(beg, end, rate);
            transform.SetLocalScaleX(scale);
            transform.SetLocalScaleZ(scale);
            yield return null;
            time += Time.deltaTime;
        }
        transform.SetLocalScaleX(end);
        transform.SetLocalScaleZ(end);
    }

    IEnumerator Twinkle(float duration)
    {
        SetAlpha(0f);
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float alpha = 0f;
            if (rate < 0.25f)
            {
                alpha = Mathf.Sin(2f * Mathf.PI * rate);
            }
            else if (rate < 0.75f)
            {
                rate -= 0.25f;
                alpha = 0.3f * Mathf.Cos(4f * Mathf.PI * rate) + 0.7f;
            }
            else
            {
                rate -= 0.75f;
                alpha = Mathf.Cos(2f * Mathf.PI * rate);
            }
            SetAlpha(alpha);
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(0f);
        Destroy(gameObject);
    }

    private void SetAlpha(float value)
    {
        Color color = material.GetColor("_Color");
        color.a = value;
        material.SetColor("_Color", color);
    }
}