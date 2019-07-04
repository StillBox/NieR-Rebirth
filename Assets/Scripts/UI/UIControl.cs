using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIControl
{
    //For getting random character

    private static char[] controlCharacters =
        {
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
    };

    public static char GetRandomChar(bool lowerCase, bool upperCase)
    {
        int beg = lowerCase ? 0 : 26;
        int end = upperCase ? 52 : 26;
        return controlCharacters[Random.Range(beg, end)];
    }

    //Methods for setting color and alpha of UI components

    public static void SetAlpha(this Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    public static void SetAlpha(this Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public static void SetColor(this Text text, Color color)
    {
        Color newColor = color;
        newColor.a = text.color.a;
        text.color = newColor;
    }

    public static void SetColor(this Image image, Color color)
    {
        Color newColor = color;
        newColor.a = image.color.a;
        image.color = newColor;
    }
}