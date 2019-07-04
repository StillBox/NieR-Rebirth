using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookshelf : MonoBehaviour
{
    private const float BOTTOM_HEIGHT = 0.2f;
    private const float TOP_HEIGHT = 0.2f;
    private const float SIDE_WIDTH = 0.4f;
    private const float DECK_HEIGHT = 0.05f;

    [SerializeField] private Transform bottom;
    [SerializeField] private Transform top;
    [SerializeField] private Transform sideL;
    [SerializeField] private Transform sideR;
    [SerializeField] private Transform girdersHolder;
    [SerializeField] private Transform decksHolder;
    [SerializeField] private Transform girderPrefab;
    [SerializeField] private Transform deckPrefab;
    
    [SerializeField, SetProperty("ColumnCount")]
    private int columnCount = 2;
    public int ColumnCount
    {
        set
        {
            columnCount = Mathf.Max(value, 1);

            for (int i = girdersHolder.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(girdersHolder.GetChild(i).gameObject);
            }

            bottom.SetLocalScaleX(width * columnCount);
            top.SetLocalScaleX(width * columnCount - SIDE_WIDTH * 2f);
            sideL.SetLocalPositionX((SIDE_WIDTH - width * columnCount) / 2f);
            sideR.SetLocalPositionX((width * columnCount - SIDE_WIDTH) / 2f);
            for (int i = 0; i < columnCount - 1; i++)
            {
                Transform girder = Instantiate(girderPrefab, girdersHolder);
                girder.SetLocalScaleX(SIDE_WIDTH * 2f);
                girder.SetLocalScaleY(height - BOTTOM_HEIGHT - TOP_HEIGHT);
                girder.SetLocalScaleZ(depth);
                girder.SetLocalPositionX((i - columnCount / 2f + 1f) * width);
                girder.SetLocalPositionY((height + BOTTOM_HEIGHT - TOP_HEIGHT) / 2f);
            }

            SetDecks();
        }
    }

    [SerializeField, SetProperty("Width")]
    private float width = 6.4f;
    public float Width
    {
        set
        {
            width = value;

            bottom.SetLocalScaleX(width * columnCount);
            top.SetLocalScaleX(width * columnCount - SIDE_WIDTH * 2f);
            sideL.SetLocalPositionX((SIDE_WIDTH - width * columnCount) / 2f);
            sideR.SetLocalPositionX((width * columnCount - SIDE_WIDTH) / 2f);
            for (int i = girdersHolder.childCount - 1; i >= 0; i--)
            {
                girdersHolder.GetChild(i).SetLocalPositionX((i - columnCount / 2f + 1f) * width);
            }

            SetDecks();
        }
    }

    [SerializeField, SetProperty("Height")]
    private float height = 7.2f;
    public float Height
    {
        set
        {
            height = value;

            top.SetLocalPositionY(height - TOP_HEIGHT / 2f);
            sideL.SetLocalScaleY(height - BOTTOM_HEIGHT);
            sideR.SetLocalScaleY(height - BOTTOM_HEIGHT);
            sideL.SetLocalPositionY((height + BOTTOM_HEIGHT) / 2f);
            sideR.SetLocalPositionY((height + BOTTOM_HEIGHT) / 2f);
            for (int i = girdersHolder.childCount - 1; i >= 0; i--)
            {
                girdersHolder.GetChild(i).SetLocalScaleY(height - BOTTOM_HEIGHT - TOP_HEIGHT);
                girdersHolder.GetChild(i).SetLocalPositionY((height + BOTTOM_HEIGHT - TOP_HEIGHT) / 2f);
            }

            SetDecks();
        }
    }

    [SerializeField, SetProperty("Depth")]
    private float depth = 1f;
    public float Depth
    {
        set
        {
            depth = value;

            bottom.SetLocalScaleZ(depth);
            top.SetLocalScaleZ(depth);
            sideL.SetLocalScaleZ(depth);
            sideR.SetLocalScaleZ(depth);
            for (int i = girdersHolder.childCount - 1; i >= 0; i--)
            {
                girdersHolder.GetChild(i).SetLocalScaleZ(depth);
            }
            for (int i = decksHolder.childCount - 1; i >= 0; i--)
            {
                decksHolder.GetChild(i).SetLocalScaleZ(depth - 0.1f);
            }
        }
    }


    [SerializeField, SetProperty("Gap")]
    private float gap = 0.85f;
    public float Gap
    {
        set
        {
            gap = Mathf.Max(value, DECK_HEIGHT);

            SetDecks();
        }
    }

    private void SetDecks()
    {
        for (int i = decksHolder.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(decksHolder.GetChild(i).gameObject);
        }

        int count = (int)((height - BOTTOM_HEIGHT - TOP_HEIGHT) / gap) - 1;
        for (int i = 0; i < count; i++)
        {
            for (int c = 0; c < columnCount; c++)
            {
                Transform deck = Instantiate(deckPrefab, decksHolder);
                deck.SetLocalScaleX(width - SIDE_WIDTH * 2f);
                deck.SetLocalScaleY(DECK_HEIGHT);
                deck.SetLocalScaleZ(depth - 0.1f);
                deck.SetLocalPositionX((c - columnCount / 2f + 0.5f) * width);
                deck.SetLocalPositionY(BOTTOM_HEIGHT + gap * (i + 1) + DECK_HEIGHT / 2f);
            }
        }
    }
}