using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilShield : MonoBehaviour
{
    private const float SPEED = 0.3f;
    private float offset;
    private Material material;

    private bool isCharged = false;
    public bool IsCharged
    {
        get { return isCharged; }
    }

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        offset = 0f;
        SetScale(0f);
    }

    void Update()
    {
        offset += SPEED * Time.deltaTime;
        while (offset >= 1f)
        {
            offset -= 1f;
        }
        material.SetFloat("_Offset", offset);
    }

    public void Expand(float duration)
    {
        StartCoroutine(ChangeScale(0.5f, 2f, duration));
        StartCoroutine(ChangeAlpha(0f, 0.5f, duration * 0.6f));
    }

    public void Shrink(float duration)
    {
        StartCoroutine(ChangeScale(2f, 0.5f, duration));
        StartCoroutine(ChangeAlpha(0.5f, 0f, duration * 0.6f, duration * 0.4f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.Damage();
            Player.instance.PushAway(transform.position, 15f);
        }
    }

    IEnumerator ChangeScale(float beg, float end, float duration)
    {
        SetScale(beg);
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float scale = Mathf.Lerp(beg, end, rate);
            SetScale(scale);
            yield return null;
            time += Time.deltaTime;
        }
        SetScale(end);
    }

    IEnumerator ChangeAlpha(float beg, float end, float duration, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);
        SetAlpha(beg);
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float alpha = Mathf.Lerp(beg, end, rate);
            SetAlpha(alpha);
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(end);
    }

    private void SetScale(float value)
    {
        isCharged = value >= 1f;
        transform.localScale = Vector3.one * value;
    }

    private void SetAlpha(float value)
    {
        material.SetFloat("_Alpha", value);
    }
}