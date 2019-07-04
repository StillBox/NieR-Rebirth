using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDProgress : MonoBehaviour
{
    [SerializeField] Image back;
    [SerializeField] Image front;

    public Transform anchor = null;

    public bool IsActive
    {
        get { return gameObject.activeSelf; }
        set { SetActive(value); }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetProgress(float value)
    {
        front.fillAmount = value;
    }
    
    public void SetPosition(Vector2 value)
    {
        GetComponent<RectTransform>().anchoredPosition = value;
    }
}