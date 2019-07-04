using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default closed on start, if not set opened
/// </summary>
/// 
public class Bridge : MonoBehaviour
{
    //Values and functions for global control

    static List<Bridge> bridges = new List<Bridge>();

    public static void ClearAll()
    {
        foreach (Bridge bridge in bridges)
        {
            if (bridge != null)
                Destroy(bridge.gameObject);
        }
        bridges.Clear();
    }

    public static void SetAccessible(bool value)
    {
        foreach (Bridge bridge in bridges)
        {
            if (bridge != null && bridge.isOpened)
            {
                bridge.wall.SetActive(!value);
            }
        }
    }

    //For single object

    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject halo;

    private bool isOpened;
    private Material matHalo;

    private void Awake()
    {
        bridges.Add(this);
        matHalo = halo.GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        if (!isOpened)
        {
            wall.SetActive(true);
            floor.SetActive(false);
        }
    }

    public void Open(bool blink = true)
    {
        if (blink)
        {
            StartCoroutine(BlinkIn());
        }
        else
        {
            wall.SetActive(false);
            floor.SetActive(true);
            isOpened = true;
        }
    }

    public void Close(bool blink = false)
    {
        if (blink)
        {
            StartCoroutine(BlinkOut());
        }
        else
        {
            wall.SetActive(true);
            floor.SetActive(false);
            isOpened = false;
        }
    }

    IEnumerator BlinkIn()
    {
        SoundManager.instance.PlayEfx(Efx.SHOW_UP, transform.position);
        float time = 0f;
        while (time < 0.2f)
        {
            SetHaloAlpha(5f * time);
            yield return null;
            time += Time.deltaTime;
        }
        SetHaloAlpha(1f);
        while (time < 0.4f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        wall.SetActive(false);
        floor.SetActive(true);
        while (time < 0.5f)
        {
            SetHaloAlpha(10f * (0.5f - time));
            yield return null;
            time += Time.deltaTime;
        }
        SetHaloAlpha(0f);
        isOpened = true;
    }

    IEnumerator BlinkOut()
    {
        SoundManager.instance.PlayEfx(Efx.SHOW_UP, transform.position);
        isOpened = false;
        wall.SetActive(true);
        float time = 0f;
        while (time < 0.2f)
        {
            SetHaloAlpha(5f * time);
            yield return null;
            time += Time.deltaTime;
        }
        SetHaloAlpha(1f);
        while (time < 0.4f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        floor.SetActive(false);
        while (time < 0.5f)
        {
            SetHaloAlpha(10f * (0.5f - time));
            yield return null;
            time += Time.deltaTime;
        }
        SetHaloAlpha(0f);
    }

    void SetHaloAlpha(float alpha)
    {
        matHalo.SetFloat("_Alpha", alpha);
    }
}