using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageEndMark : MonoBehaviour
{
    private const float FADE_SPEED = 5f;

    [SerializeField] private Spring spring;
    private Material material;

    private bool isHidden = true;
    private float alpha = 0f;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        alpha = 0f;
        SetAlpha(alpha);
        spring.origin = transform.position;
    }

    private void Update()
    {
        alpha += (isHidden ? -1f : 1f) * Time.deltaTime * FADE_SPEED;
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        SetAlpha(alpha);
    }

    public void Set()
    {
        isHidden = false;
    }

    public void Set(Color color)
    {
        material.SetColor("_Color", color);
        isHidden = false;
    }

    public void Hide()
    {
        isHidden = true;
    }

    public void SetColor(Color color)
    {
        material.SetColor("_Color", color);
    }

    private void SetAlpha(float value)
    {
        Color color = material.GetColor("_Color");
        color.a = value;
        material.SetColor("_Color", color);
    }

    /*
    IEnumerator Fade(float beg, float end, bool hide = false)
    {
        float time = 0f;
        while (time < 0.2f)
        {
            float alpha = Mathf.Lerp(beg, end, time / 0.2f);
            material.SetFloat("_Scale", alpha);
            yield return null;
            time += Time.deltaTime;
        }
        material.SetFloat("_Scale", end);
        if (hide)
            gameObject.SetActive(false);
    }
    */
}
