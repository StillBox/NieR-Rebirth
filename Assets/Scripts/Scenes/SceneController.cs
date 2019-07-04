using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance = null;

    public bool IsSceneOver { get; set; }

    virtual protected void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        instance = this;
    }

    virtual public void ResetProgress()
    {
        HUDManager.instance.ResetAll();
    }

    protected void SlowDown(float min, float duration)
    {
        StartCoroutine(ChangeTimeScale(min, duration));
    }

    IEnumerator ChangeTimeScale(float min, float duration)
    {
        float time = 0f;
        while (time < duration - 0.01f)
        {
            float rate = Mathf.Sin(time / duration * Mathf.PI / 2f);
            float scale = Mathf.Lerp(1f, min, rate);
            Time.timeScale = scale;
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        Time.timeScale = 1f;
    }
}