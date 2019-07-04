using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private Image caption;
    [SerializeField] private Image window;
    [SerializeField] private Text header;
    [SerializeField] private Text text;
    [SerializeField] private Font font;

    private const float BORDER = 20f;
    private RectTransform thisTransform;

    private void Awake()
    {
        thisTransform = GetComponent<RectTransform>();
    }
    
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetPosition(Vector2 pos)
    {
        thisTransform.anchoredPosition = pos;
    }

    public void MoveBy(Vector2 delta)
    {
        Vector2 pos = thisTransform.anchoredPosition;
        pos += delta;
        thisTransform.anchoredPosition = pos;
    }

    public void SetHeader(string str)
    {
        header.text = str;   
    }

    public void SetMessage(string message)
    {
        thisTransform.sizeDelta = GetMessageSize(message);
        text.text = message;
    }

    public void SetAlpha(float alpha)
    {
        caption.SetAlpha(alpha);
        window.SetAlpha(alpha);
        header.SetAlpha(alpha);
        text.SetAlpha(alpha);
    }

    Vector2 GetMessageSize(string message)
    {
        int fontSize = text.fontSize;
        font.RequestCharactersInTexture(message, fontSize, FontStyle.Normal);
        CharacterInfo characterInfo;
        float lineCount = 0;
        float maxWidth = 0;
        float height = 0;
        float width = 0;
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i].Equals('\n'))
            {
                lineCount++;
                width = 0;
            }
            else
            {
                font.GetCharacterInfo(message[i], out characterInfo, fontSize);
                width += characterInfo.advance;
                if (width > maxWidth)
                    maxWidth = width;
                if (characterInfo.glyphHeight > height)
                    height = characterInfo.glyphHeight;
            }
        }
        return new Vector2(maxWidth + 2 * BORDER, lineCount * height * text.lineSpacing + 4f * BORDER);
    }
}

[System.Serializable]
public class Message
{
    public float wait;
    public float duration;
    public Vector2 position;
    public string message;
    public string header;

    public Message(float wait, float duration, Vector2 position, string message, string header = "MESSAGE")
    {
        this.wait = wait;
        this.duration = duration;
        this.message = message;
        this.position = position;
        this.header = header;
    }

    public Message() : this(0f, 3f, Vector2.zero, "UNDEFINED", "UNDEFINED") { }

    public static Message[] GetMessages(TextAsset asset)
    {
        string strAsset = asset.text;

        List<Message> list = new List<Message>();
        while (!strAsset.Equals(string.Empty))
        {
            string line = string.Empty;
            int index = Mathf.Min(strAsset.IndexOf('\r'), strAsset.IndexOf('\n'));
            if (index < 0)
            {
                line = strAsset;
                strAsset = string.Empty;
            }
            else
            {
                line = strAsset.Substring(0, index);
                strAsset = strAsset.Substring(index + 1);
            }
            if (line != string.Empty)
            {
                string[] contents = line.Split('|');
                float wait = float.Parse(contents[0]);
                float duration = float.Parse(contents[1]);
                float x = float.Parse(contents[2]);
                float y = float.Parse(contents[3]);
                string header = contents[4];
                string text = contents[5];
                for (int i = 6; i < contents.Length; i++) text += '\n' + contents[i];
                Message message = new Message(wait, duration, new Vector2(x, y), text, header);
                list.Add(message);
            }
        }
        return list.ToArray();
    }

    public static Message GetMessage(TextAsset asset, int index)
    {
        return GetMessages(asset)[index];
    }
}