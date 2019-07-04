using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryBoard : MonoBehaviour
{
    public Text textPrefab;

    private List<Text> texts;
    private Text current;
    private Vector2 currentPos;
    private Color textColor;

    private void Awake()
    {
        texts = new List<Text>();
    }

    public void AddText(float x, float y)
    {
        Text newText = Instantiate(textPrefab, transform);
        newText.GetComponent<RectTransform>().offsetMin = new Vector2(x, 0);
        newText.GetComponent<RectTransform>().offsetMax = new Vector2(0, -y);
        newText.color = textColor;
        texts.Add(newText);
        current = newText;
        currentPos = new Vector2(x, y);
    }

    public void SetText(string text)
    {
        current.text = text;
    }

    public void ClearAll()
    {
        foreach (Text text in texts)
        {
            Destroy(text.gameObject);
        }
        texts.Clear();
    }

    public void SetColor(Color color)
    {
        textColor = color;
        foreach (Text text in texts)
        {
            text.color = color;
        }
    }
    
    public Vector2 GetEndPosition()
    {
        int fontSize = current.fontSize;
        Font font = current.font;
        string str = current.text;
        font.RequestCharactersInTexture(str, fontSize, FontStyle.Normal);
        CharacterInfo characterInfo;
        float lineCount = 0;
        float height = 0;
        float width = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i].Equals('\n'))
            {
                lineCount++;
                width = 0;
            }
            else
            {
                font.GetCharacterInfo(str[i], out characterInfo, fontSize);
                width += characterInfo.advance;
                if (characterInfo.glyphHeight > height)
                    height = characterInfo.glyphHeight;
            }
        }
        Vector2 boardPos = new Vector2(width + fontSize, (lineCount * current.lineSpacing + 0.5f) * height);
        boardPos += currentPos;
        Vector2 resolution = GetComponent<CanvasScaler>().referenceResolution;
        boardPos -= 0.5f * resolution;
        boardPos.y = -boardPos.y;
        return 2f * boardPos / resolution.y;
    }
}