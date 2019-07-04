using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlInfo : MonoBehaviour
{
    private static readonly string[] hints =
    {
        "Enter 确定   Esc 取消",
        "Ⓞ 确定   Ⓧ 取消",
        "Ⓐ 确定   Ⓑ 取消"
    };

    private Text info;

    private void Awake()
    {
        info = GetComponent<Text>();
    }

    void Update ()
    {
		switch (STBInput.GetPlayerDeviceType(0))
        {
            case InputDeviceType.KeyboardAndMouse:
                info.text = hints[0];
                break;
            case InputDeviceType.PS4Controller:
                info.text = hints[1];
                break;
            case InputDeviceType.XboxOneController:
                info.text = hints[2];
                break;
            default:
                info.text = hints[2];
                break;
        }
    }

    /*
    public void FadeIn(float duration, float wait = 0f)
    {
        StartCoroutine(Fade(0f, 1f, duration, wait));
    }

    public void FadeOut(float duration, float wait = 0f)
    {
        StartCoroutine(Fade(1f, 0f, duration, wait));
    }

    IEnumerator Fade(float beg, float end, float duration, float wait = 0f)
    {
        SetAlpha(beg);
        yield return new WaitForSecondsRealtime(wait);

        float time = 0f;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(beg, end, time / duration);
            SetAlpha(alpha);
            yield return null;
            time += Time.unscaledDeltaTime;
        }

        SetAlpha(end);
    }

    void SetAlpha(float alpha)
    {
        info.SetAlpha(alpha);
    }
    */
}
