using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryScene : SceneController
{
    private const float TEXT_GAP = 0.08f;
    
    [SerializeField] private StoryBoard storyBoard;
    [SerializeField] private ParaEndMark paraEndMark;
    [SerializeField] private PageEndMark pageEndMark;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private StoryClip intro;
    [SerializeField] private StoryClip intro_1;
    [SerializeField] private StoryClip ch1;
    [SerializeField] private StoryClip ch1_1;
    [SerializeField] private StoryClip ch1_2;
    [SerializeField] private StoryClip ch2;
    [SerializeField] private StoryClip ch2_1;
    [SerializeField] private StoryClip ch3;
    [SerializeField] private StoryClip ch3_1;
    [SerializeField] private StoryClip ch3_2;
    [SerializeField] private StoryClip ch3_3;
    [SerializeField] private StoryClip coda;
    [SerializeField] private StoryClip coda_1;
    [SerializeField] private StoryClip coda_2;

    private StoryClip currentClip;
    private StoryPara[] currentStory;
    private AudioClip bgm;
    private GameScene nextScene;

    private int currentPara;
    private int currentChar;
    private bool isParaOver;
    private bool isPageOver;
    
    void Start()
    {
        GameManager.gameScene = GameScene.Story;

        currentPara = 0;
        currentChar = 0;
        isParaOver = false;
        isPageOver = false;

        switch (GameManager.storyChapter)
        {
            case StoryChapter.Intro:
                GameManager.SetLastPlayProgress(GameProgress.StoryIntro);
                LoadStory(intro);
                Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear, StoryStart);
                break;
            case StoryChapter.Intro_1:
                GameManager.SetLastPlayProgress(GameProgress.StoryIntro);
                LoadStory(intro_1);
                Curtain.instance.ChangeColor(1f, Curtain.black, Curtain.black_clear, StoryStart);
                break;
            case StoryChapter.Ch1:
                GameManager.SetLastPlayProgress(GameProgress.StroyCh1);
                LoadStory(ch1);
                Curtain.instance.ChangeColor(2f, Curtain.white, Curtain.white_clear, StoryStart);
                break;
            case StoryChapter.Ch1_1:
                LoadStory(ch1_1);
                Curtain.instance.ChangeColor(1f, Curtain.light_gray, Curtain.light_gray_clear, StoryStart);
                break;
            case StoryChapter.Ch1_2:
                LoadStory(ch1_2);
                Curtain.instance.ChangeColor(1f, Curtain.black, Curtain.black_clear, StoryStart);
                break;
            case StoryChapter.Ch2:
                GameManager.SetLastPlayProgress(GameProgress.StoryCh2);
                LoadStory(ch2);
                Curtain.instance.ChangeColor(2f, Curtain.white, Curtain.white_clear, StoryStart);
                break;
            case StoryChapter.Ch2_1:
                LoadStory(ch2_1);
                Curtain.instance.ChangeColor(1f, Curtain.light_gray, Curtain.light_gray_clear, StoryStart);
                break;
            case StoryChapter.Ch3:
                GameManager.SetLastPlayProgress(GameProgress.StoryCh3);
                LoadStory(ch3);
                Curtain.instance.ChangeColor(2f, Curtain.white, Curtain.white_clear, StoryStart);
                break;
            case StoryChapter.Ch3_1:
                LoadStory(ch3_1);
                Curtain.instance.ChangeColor(1f, Curtain.dark_gray, Curtain.dark_gray_clear, StoryStart);
                break;
            case StoryChapter.Ch3_2:
                LoadStory(ch3_2);
                Curtain.instance.ChangeColor(1f, Curtain.gray, Curtain.gray_clear, StoryStart);
                break;
            case StoryChapter.Ch3_3:
                LoadStory(ch3_3);
                Curtain.instance.ChangeColor(1f, Curtain.light_gray, Curtain.light_gray_clear, StoryStart);
                break;
            case StoryChapter.Coda:
                GameManager.SetLastPlayProgress(GameProgress.StoryCoda);
                LoadStory(coda);
                Curtain.instance.ChangeColor(2f, Curtain.white, Curtain.white_clear, StoryStart);
                break;
            case StoryChapter.Coda_1:
                LoadStory(coda_1);
                Curtain.instance.ChangeColor(1f, Curtain.dark_gray, Curtain.dark_gray_clear, StoryStart);
                break;
            case StoryChapter.Coda_2:
                LoadStory(coda_2);
                Curtain.instance.ChangeColor(2f, Curtain.white, Curtain.white_clear, StoryStart);
                break;
            default:
                LoadStory(intro);
                break;
        }
    }

    void Update()
    {
#if OPEN_DEBUG_MODE
        if (Input.GetKeyDown(KeyCode.PageDown))
            OnSceneEnd();
#endif

        if (isParaOver)
        {
            bool showNext = false;

            if (isPageOver)
            {
                if (STBInput.GetButtonDown("Next") || STBInput.GetButtonDown("Skip"))
                {
                    SoundManager.instance.PlayUiEfx(UiEfx.TEXT);
                    showNext = true;
                    isPageOver = false;
                    pageEndMark.Hide();
                    storyBoard.ClearAll();
                }
            }
            else
            {
                if (STBInput.GetButtonDown("Next") || STBInput.GetButton("Skip"))
                {
                    SoundManager.instance.PlayUiEfx(UiEfx.TEXT);
                    showNext = true;
                    paraEndMark.Hide();
                }
            }
            
            if (showNext)
            {
                isParaOver = false;
                if (currentPara < currentStory.Length)
                {
                    StartCoroutine(ShowPara());
                }
                else
                {
                    storyBoard.ClearAll();
                    OnSceneEnd();
                }
            }
        }
    }

    void OnSceneEnd()
    {
        switch (GameManager.storyChapter)
        {
            case StoryChapter.Intro:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Intro_1;
                Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, CallNextScene);
                break;
            case StoryChapter.Intro_1:
                nextScene = GameScene.Ch1;
                GameManager.SetUnlockedProgress(GameProgress.HackCh1);
                Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, CallNextScene);
                break;
            case StoryChapter.Ch1:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Ch1_1;
                Curtain.instance.ChangeColor(1f, Curtain.dark_gray, Curtain.light_gray, CallNextScene);
                break;
            case StoryChapter.Ch1_1:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Ch1_2;
                Curtain.instance.ChangeColor(1f, Curtain.light_gray, Curtain.black, CallNextScene);
                break;
            case StoryChapter.Ch1_2:
                nextScene = GameScene.Ch2;
                GameManager.SetUnlockedProgress(GameProgress.HackCh2);
                Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, CallNextScene);
                break;
            case StoryChapter.Ch2:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Ch2_1;
                Curtain.instance.ChangeColor(1f, Curtain.black, Curtain.light_gray, CallNextScene);
                break;
            case StoryChapter.Ch2_1:
                nextScene = GameScene.Ch3;
                GameManager.SetUnlockedProgress(GameProgress.HackCh3);
                Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, CallNextScene);
                break;
            case StoryChapter.Ch3:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Ch3_1;
                Curtain.instance.ChangeColor(1f, Curtain.black, Curtain.dark_gray, CallNextScene);
                break;
            case StoryChapter.Ch3_1:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Ch3_2;
                Curtain.instance.ChangeColor(1f, Curtain.dark_gray, Curtain.gray, CallNextScene);
                break;
            case StoryChapter.Ch3_2:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Ch3_3;
                Curtain.instance.ChangeColor(1f, Curtain.gray, Curtain.light_gray, CallNextScene);
                break;
            case StoryChapter.Ch3_3:
                nextScene = GameScene.Ch4;
                GameManager.SetUnlockedProgress(GameProgress.HackCh4);
                Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, CallNextScene);
                break;
            case StoryChapter.Coda:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Coda_1;
                Curtain.instance.ChangeColor(1f, Curtain.light_gray, Curtain.dark_gray, CallNextScene);
                break;
            case StoryChapter.Coda_1:
                nextScene = GameScene.Story;
                GameManager.storyChapter = StoryChapter.Coda_2;
                Curtain.instance.ChangeColor(2f, Curtain.light_gray, Curtain.white, CallNextScene);
                break;
            case StoryChapter.Coda_2:
                nextScene = GameScene.Menu;
                GameManager.SetUnlockedProgress(GameProgress.MainClear);
                Curtain.instance.ChangeColor(2f, Curtain.white, Curtain.black, CallNextScene);
                break;
            default:
                nextScene = GameScene.Menu;
                SoundManager.instance.PlayUiEfx(UiEfx.SCENE_CHANGE);
                Curtain.instance.ChangeColor(2f, Curtain.white_clear, Curtain.white, BackToDark);
                break;
        }
    }

    void LoadStory(StoryClip clip)
    {
        currentClip = Instantiate(clip);
        currentStory = currentClip.story;
        bgm = currentClip.bgm;
        SetTextColor(currentClip.textColor);
        SetBackColor(currentClip.backgroundColor);
    }

    void StoryStart()
    {
        StartCoroutine(ShowPara());
    }

    private void BackToDark()
    {
        Curtain.instance.ChangeColor(1f, Curtain.white, Curtain.black, CallNextScene);
    }

    void CallNextScene()
    {
        GameManager.SetScene(nextScene);
    }

    IEnumerator ShowPara()
    {
        StoryPara para = currentStory[currentPara];

        if (!para.IsTag)
        {
            storyBoard.AddText(para.x, para.y);
            currentChar = 0;
            float time = 0f;
            float efxTime = 0f;
            bool isSpeedUp = false;
            while (currentChar < para.Length)
            {
                yield return null;

                if (STBInput.GetButtonDown("Next")) isSpeedUp = true;
                if (STBInput.GetButtonUp("Next")) isSpeedUp = false;
                if (STBInput.GetButton("Skip")) currentChar = para.Length - 1;

                time += Time.deltaTime * (isSpeedUp ? 5f : 1f);
                efxTime += Time.deltaTime;

                if (time > TEXT_GAP)
                {
                    currentChar++;
                    if (efxTime >= TEXT_GAP)
                    {
                        SoundManager.instance.PlayUiEfx(UiEfx.TEXT);
                        while (efxTime >= TEXT_GAP) efxTime -= TEXT_GAP;
                    }
                    storyBoard.SetText(para.Substring(currentChar));
                    if (para.GetChar(currentChar - 1).Equals('\n'))
                        time -= TEXT_GAP * 5f;
                    else
                        time -= TEXT_GAP;
                }
            }
            isParaOver = true;
            paraEndMark.Set(GetActPos(storyBoard.GetEndPosition()));
            currentPara++;
            para = currentStory[currentPara];
        }

        if (para.IsTag)
        {
            currentPara++;
            if (para.IsBgmPlay) SoundManager.instance.PlayBgm(bgm, 2f);
            if (para.IsBgmStop) SoundManager.instance.StopBgm(1f);
            if (para.IsPageEnd)
            {
                paraEndMark.Hide();
                pageEndMark.Set();
                isPageOver = true;
                yield break;
            }
            if (para.IsClipEnd)
            {
                OnSceneEnd();
                yield break;
            }
            if (!isParaOver) StartCoroutine(ShowPara());
        }        
    }

    Vector2 GetActPos(Vector2 ratio)
    {
        float size = mainCamera.orthographicSize;
        return ratio * size;
    }

    void SetTextColor(Color color)
    {
        paraEndMark.SetColor(color);
        pageEndMark.SetColor(color);
        storyBoard.SetColor(color);
    }

    void SetBackColor(Color color)
    {
        mainCamera.backgroundColor = color;
    }
}