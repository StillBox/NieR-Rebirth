using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryClip : MonoBehaviour
{
    public TextAsset textAsset;
    public AudioClip bgm;
    public Color textColor = Color.white;
    public Color backgroundColor = Color.black;

    [HideInInspector] public StoryPara[] story;

    private void Awake()
    {
        story = StoryPara.GetStoryParas(textAsset);
    }
}