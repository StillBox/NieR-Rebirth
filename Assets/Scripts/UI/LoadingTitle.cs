using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTitle : MonoBehaviour
{
    private const float GAP = 0.3f;
    private const int MAX_COUNT = 3;

    private Text loading;
    float time = 0f;
    int counter = 0;

    private void Awake()
    {
        loading = transform.Find("Loading").GetComponent<Text>();
    }

    private void Update()
    {
        time += Time.unscaledDeltaTime;
        if (time >= GAP)
        {
            counter++;
            if (counter > MAX_COUNT)
            {
                counter = 0;
                time -= 1f - MAX_COUNT * GAP;
            }
            else
            {
                time -= GAP;
            }
            string text = " - 系统启动中";
            for (int i = 0; i < counter; i++) text += " .";
            loading.text = text;
        }
    }
}
