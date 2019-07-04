using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderBar : MonoBehaviour
{
    public SliderButton Parent
    {
        set; get;
    }
    public Image[] images;
    public SliderBlock blockPrefab;
    private SliderBlock[] blocks;

    private int filledCount = 0;
    public bool IsActivated { set; get; }

    private void Update()
    {
        if (filledCount < Parent.value)
        {
            if (filledCount > 0 && !blocks[filledCount].Maximize()) return;
            if (blocks[filledCount + 1].Maximize()) filledCount++;
        }

        if (filledCount > Parent.value)
        {
            if (filledCount < 10 && !blocks[filledCount + 1].Minimize()) return;
            if (blocks[filledCount].Minimize()) filledCount--;
        }
    }

    public void Init()
    {
        Parent = GetComponentInParent<SliderButton>();
        filledCount = 0;

        if (blocks == null)
        {
            blocks = new SliderBlock[11];
            for (int i = 0; i < 11; i++)
                blocks[i] = CreateBlock(i, i != 0, -102f + 10f * i);
            for (int i = 0; i < 11; i++)
            {
                blocks[i].navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = i == 0 ? null : blocks[i - 1],
                    selectOnRight = i == 10 ? null : blocks[i + 1]
                };
            }
        }

        foreach (SliderBlock block in blocks) block.Init();
    }

    public void Activate()
    {
        IsActivated = true;

        foreach (SliderBlock block in blocks)
        {
            if (Parent.value == block.value)
            {
                EventSystem.current.SetSelectedGameObject(block.gameObject);
                break;
            }
        }
    }

    public void SetAlpha(float value)
    {
        foreach (SliderBlock block in blocks)
        {
            block.SetAlpha(value);
        }
        foreach (Image image in images)
        {
            image.SetAlpha(value);
        }
    }

    private SliderBlock CreateBlock(float value, bool isVisible, float xPosition)
    {
        SliderBlock block = Instantiate(blockPrefab, transform);
        block.Parent = Parent;
        block.ParentBar = this;
        block.value = value;
        block.IsVisible = isVisible;
        block.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, 0f);
        return block;
    }
}