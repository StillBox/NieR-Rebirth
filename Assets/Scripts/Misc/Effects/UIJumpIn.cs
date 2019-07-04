using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public class UIJumpIn : MonoBehaviour
{
    public float fadeTime = 1f;
    public float jumpTime = 2f;
    public float iniSpeed = 4f;
    public float gravity = -3;

    private float updateTime;
    private RectTransform rectTransform;
    private float iniHeight;

    private float Alpha
    {
        get
        {
            return GetComponent<Image>().color.a;
        }
        set
        {
            Color color = new Color(0f, 0f, 0f, value);
            GetComponent<Image>().color = color;
        }
    }

    // Use this for initialization
    void Start()
    {
        updateTime = 0f;
        rectTransform = GetComponent<RectTransform>();
        iniHeight = rectTransform.position.y - (iniSpeed * jumpTime + gravity * jumpTime * jumpTime / 2);
        Alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (updateTime < jumpTime)
        {
            updateTime += Time.deltaTime;
            Alpha = Mathf.Clamp(updateTime / fadeTime, 0f, 1f);
            if (updateTime >= jumpTime)
            {
                updateTime = jumpTime;
            }
            float height = iniSpeed * updateTime + gravity * updateTime * updateTime / 2;
            SetHeight(height);
        }
    }

    void SetHeight(float height)
    {
        Vector3 position = rectTransform.position;
        position.y = iniHeight + height;
        rectTransform.position = position;
    }
}
