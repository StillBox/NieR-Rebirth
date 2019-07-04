using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoScene : SceneController
{
    public Canvas canvas;
    public GameObject meteor;
    public GameObject blockPrefab;
    public LogoChar[] logoChars;

    private float updateTime;
    private int blockCount_x = 160;
    private int blockCount_y = 90;
    private float spacing = 12f;
    private bool isOver = false;

    private List<BlockInfo> blocks;

    private void Awake()
    {
        blocks = new List<BlockInfo>();
    }
    
    void Start()
    {
        GameManager.gameScene = GameScene.Logo;

        updateTime = 0f;
        for (int i = 0; i < logoChars.Length; i++)
        {
            LogoChar logoChar = logoChars[i];
            Vector2Int anchor = logoChar.anchor;
            for (int j = 0; j < logoChar.offsets.Length; j++)
            {
                Vector2Int offset = logoChar.offsets[j];
                AddBlock(anchor.x + offset.x, anchor.y + offset.y);
            }
        }

        Curtain.instance.ChangeColor(1f, Curtain.logoScene, Curtain.black_clear);
    }
    
    void Update()
    {
        updateTime += Time.deltaTime;

        float x = -960f - spacing / 2, y = 540f;

        if (updateTime < 1.5f)
        {
            x += spacing * (blockCount_x / 2 + 29) * updateTime / 1.5f;
            y -= spacing * (blockCount_y / 2 + 4);
        }
        else if (updateTime < 1.75f)
        {
            x += spacing * (blockCount_x / 2 + 29) + 10f * spacing * (float)Mathf.Sin(Mathf.PI * 0.5f * (updateTime - 1.5f) / 0.25f);
            y -= spacing * (blockCount_y / 2 - 6) + 10f * spacing * (float)Mathf.Cos(Mathf.PI * 0.5f * (updateTime - 1.5f) / 0.25f);
        }
        else if (updateTime < 2.375f)
        {
            x += spacing * (blockCount_x / 2 + 14) + 25f * spacing * (float)Mathf.Sin(Mathf.PI * 0.5f * ((updateTime - 1.75f) / 0.625f + 1f));
            y -= spacing * (blockCount_y / 2 - 6) + 25f * spacing * (float)Mathf.Cos(Mathf.PI * 0.5f * ((updateTime - 1.75f) / 0.625f + 1f));
        }
        else if (updateTime < 3.000f)
        {
            x += spacing * (blockCount_x / 2 + 14) + 25f * spacing * (float)Mathf.Sin(Mathf.PI * 0.5f * ((updateTime - 2.375f) / 0.625f + 2f));
            y -= spacing * (blockCount_y / 2 - 6) + 25f * spacing * (float)Mathf.Cos(Mathf.PI * 0.5f * ((updateTime - 2.375f) / 0.625f + 2f));
        }
        else
        {
            x += spacing * (blockCount_x / 2 - 11);
            y -= spacing * (blockCount_y / 2 - 6);
        }

        meteor.GetComponent<RectTransform>().position = new Vector3(x, y, 0f);

        if (updateTime >= 4f && updateTime < 6f)
        {
            float scale = 1f + (float)Mathf.Pow((updateTime - 4f) / 0.1f, 2);
            meteor.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 0f);
        }

        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            BlockInfo block = blocks[i];
            if (block.x + 2 * block.y <= x + 2 * y)
            {
                block.bOn = true;
                block.timer = Random.Range(0f, 0.08f);
            }
            if (block.bOn)
            {
                block.timer -= Time.deltaTime;
                if (block.timer <= 0f)
                {
                    GameObject instance = Instantiate(blockPrefab, new Vector3(block.x, block.y, 0f), Quaternion.identity);
                    instance.transform.SetParent(canvas.transform);
                    blocks.RemoveAt(i);
                }
            }
        }

        if (!isOver)
        {
            if (updateTime >= 4f || (updateTime >= 1.5f && Input.anyKeyDown))
            {
                isOver = true;
                if (GameManager.IsUnlocked(GameProgress.StoryIntro))
                    Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, LoadMenuScene);
                else
                    Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, LoadPrologueScene);
            }
        }
    }

    private void LoadMenuScene()
    {
        GameManager.SetScene(GameScene.Menu);
    }

    private void LoadPrologueScene()
    {
        GameManager.SetScene(GameScene.Prologue);
    }

    private 
    
    class BlockInfo
    {
        public float x;
        public float y;
        public bool bOn;
        public float timer;
    }

    void AddBlock(int x, int y)
    {
        BlockInfo block = new BlockInfo
        {
            x = spacing * x,
            y = spacing * y,
            bOn = false,
            timer = 0f
        };
        blocks.Add(block);
    }

    [System.Serializable]
    public class LogoChar
    {
        public Vector2Int anchor;
        public Vector2Int[] offsets;
    }
}