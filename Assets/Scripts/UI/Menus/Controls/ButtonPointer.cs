using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPointer : MonoBehaviour
{
    public Sprite focus;
    public Sprite press;
    private Image image;
    
    public void SetNormal()
    {
        gameObject.SetActive(false);
    }

    public void SetFocus()
    {
        gameObject.SetActive(true);
        GetComponent<Image>().sprite = focus;
    }

    public void SetPress()
    {
        gameObject.SetActive(true);
        GetComponent<Image>().sprite = press;
    }

    public void SetAlpha(float alpha)
    {
        GetComponent<Image>().SetAlpha(alpha);
    }
}