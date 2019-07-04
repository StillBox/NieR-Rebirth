using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaEndMark : MonoBehaviour
{
    private const float FADE_SPEED = 10f;

    [SerializeField] private Spring spring;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isHidden = true;
    private float alpha = 0f;

    private void Start()
    {
        alpha = 0f;
        SetAlpha(alpha);
    }

    private void Update()
    {
        alpha += (isHidden ? -1f : 1f) * Time.deltaTime * FADE_SPEED;
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        SetAlpha(alpha);
    }

    public void Set(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, 0f);
        spring.origin = new Vector3(pos.x, pos.y, 0f);
        isHidden = false;
    }

    public void Set(Vector2 pos, Color color)
    {
        transform.position = new Vector3(pos.x, pos.y, 0f);
        spring.origin = new Vector3(pos.x, pos.y, 0f);
        spriteRenderer.color = color;
        isHidden = false;
    }

    public void Hide()
    {
        isHidden = true;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    private void SetAlpha(float value)
    {
        Color color = spriteRenderer.color;
        color.a = value;
        spriteRenderer.color = color;
    }

    /*
    IEnumerator Fade(float beg, float end, bool hide = false)
    {
        float time = 0f;
        while (time < 0.1f)
        {
            float alpha = Mathf.Lerp(beg, end, time / 0.1f);
            SetAlpha(alpha);
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(end);
    }
    */
}