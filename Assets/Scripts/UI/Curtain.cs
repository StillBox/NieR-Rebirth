using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Curtain : MonoBehaviour
{
    public static Color logoScene = new Color(35f / 256f, 31f / 256f, 32f / 256f);

    public static Color white = new Color(1f, 1f, 1f, 1f);
    public static Color white_clear = new Color(1f, 1f, 1f, 0f);
    public static Color black = new Color(0f, 0f, 0f, 1f);
    public static Color black_clear = new Color(0f, 0f, 0f, 0f);

    public static Color gray = new Color(0.5f, 0.5f, 0.5f, 1f);
    public static Color gray_clear = new Color(0.5f, 0.5f, 0.5f, 0f);
    public static Color dark_gray = new Color(0.25f, 0.25f, 0.25f, 1f);
    public static Color dark_gray_clear = new Color(0.25f, 0.25f, 0.25f, 0f);
    public static Color light_gray = new Color(0.75f, 0.75f, 0.75f, 1f);
    public static Color light_gray_clear = new Color(0.75f, 0.75f, 0.75f, 0f);

    public static Color dark_brown = new Color(0.301f, 0.285f, 0.254f, 1f);
    public static Color dark_brown_clear = new Color(0.301f, 0.285f, 0.254f, 0f);
    public static Color light_brown = new Color(0.781f, 0.754f, 0.664f, 1f);
    public static Color light_brown_clear = new Color(0.781f, 0.754f, 0.664f, 0f);

    public static Curtain instance = null;

    [SerializeField] private Image image;

    private bool isChanging = false;
    public bool IsChanging
    {
        get { return isChanging; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Open()
    {
        Debug.Log("Curtain Opened.");
        gameObject.SetActive(true);
    }

    public void Close()
    {
        Debug.Log("Curtain Closed.");
        gameObject.SetActive(false);
    }

    public delegate void CurtainChangeHandler();

    public void SetColor(Color color)
    {
        if (!IsChanging)
        {
            image.color = color;
        }
    }

    public void ChangeColor(float duration, Color color0, Color color1, CurtainChangeHandler handler = null)
    {
        if (!isChanging)
        {
            isChanging = true;
            StartCoroutine(Change(duration, color0, color1, handler));
        }
    }

    IEnumerator Change(float duration, Color color0, Color color1, CurtainChangeHandler handler = null)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            image.color = Color.Lerp(color0, color1, t);

            time += Time.unscaledDeltaTime;
            yield return null;
        }
        image.color = color1;
        isChanging = false;
        if (handler != null) handler();
    }
}