using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryPara
{
    private static readonly string pageEnd = "<PageEnd>";
    private static readonly string bgmPlay = "<BGMPlay>";
    private static readonly string bgmStop = "<BGMStop>";
    private static readonly string clipEnd = "<ClipEnd>";

    public float x;
    public float y;
    public string text;

    public StoryPara(float x, float y, string text)
    {
        this.x = x;
        this.y = y;
        this.text = text;
    }

    public bool IsTag { get { return text.StartsWith("<") && text.EndsWith(">"); } }
    public bool IsPageEnd { get { return text.Equals(pageEnd); } }
    public bool IsBgmPlay { get { return text.Equals(bgmPlay); } }
    public bool IsBgmStop { get { return text.Equals(bgmStop); } }
    public bool IsClipEnd { get { return text.Equals(clipEnd); } }

    public int Length { get { return text.Length; } }
    public bool IsEmpty { get { return text.Equals(string.Empty); } }
    
    public char GetChar(int index) { return text[index]; }
    public string Substring(int length) { return text.Substring(0, length); }

    public static StoryPara[] GetStoryParas(TextAsset textAsset)
    {
        string strStory = textAsset.text;

        List<StoryPara> list = new List<StoryPara>();
        while (!strStory.Equals(string.Empty))
        {
            string line = string.Empty;
            int index = Mathf.Min(strStory.IndexOf('\r'), strStory.IndexOf('\n'));
            if (index < 0)
            {
                line = strStory;
                strStory = string.Empty;
            }
            else
            {
                line = strStory.Substring(0, index);
                strStory = strStory.Substring(index + 1);
            }
            if (line != string.Empty)
            {
                string[] contents = line.Split('|');
                float x = float.Parse(contents[0]);
                float y = float.Parse(contents[1]);
                string text = contents[2];
                for (int i = 3; i < contents.Length; i++) text += '\n' + contents[i];
                StoryPara para = new StoryPara(x, y, text);
                list.Add(para);
            }
        }
        return list.ToArray();
    }
}