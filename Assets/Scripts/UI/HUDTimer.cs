using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDTimer : MonoBehaviour
{
    public delegate void TimeOverHandler();
    public Transform anchor = null;

    private Text text;
    private float time;
    private TimeOverHandler handler;

    public bool IsOver
    {
        get { return time <= 0f; }
    }

    private void Awake()
    {
        text = GetComponent<Text>();
    }
    
    void Update()
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;
            if (time <= 0f)
            {
                time = 0f;
                if (handler != null)
                    handler();
            }
        }
        
        if (time <= 10f)
        {
            text.color = new Color(1f, 0.25f, 0f, 1f);
        }

        int min = (int)time / 60;
        int sec = (int)time % 60;
        int mil = (int)((time - 60 * min - sec) * 100f);
        text.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, mil);
    }

    public bool IsActive
    {
        get { return gameObject.activeSelf; }
        set { SetActive(value); }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if (!value)
            CancelInvoke();
    }

    public void Set(float time, TimeOverHandler handler = null)
    {
        this.time = time;
        this.handler = handler;
        text.color = Color.white;
        float beepGap = Mathf.Max(0f, time - 10f);
        Invoke("Beep", beepGap);
    }

    public void SetPosition(Vector2 value)
    {
        GetComponent<RectTransform>().anchoredPosition = value;
    }

    void Beep()
    {
        SoundManager.instance.PlayUiEfx(UiEfx.TICK);
        if (time > 0.05f)
        {
            float gap = time <= 1.1f ? 0.1f :
                time <= 3.1f ? 0.25f :
                time <= 5.1f ? 0.5f : 1f;
            Invoke("Beep", gap);
        }
    }
}

