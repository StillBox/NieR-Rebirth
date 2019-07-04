using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private const float BGM_LOOP_POINT = 23.2101f;
    private const float BGM_LOOP_LENGTH = 120f;

    public static MenuController instance;
    public static Menu currentMenu;
    public static bool IsActivated
    {
        get { return currentMenu != null && currentMenu.IsActivated; }
    }

    [Header("Menu Scene BGM")]

    [SerializeField] private AudioClip bgm;

    [Header("Menus")]

    [SerializeField] private Menu titleMenu;
    [SerializeField] private Menu mainMenu;

    [SerializeField] private Menu chapterMenu;
    [SerializeField] private Menu optionsMenu;
    [SerializeField] private Menu aboutMenu;

    [SerializeField] private Menu modeOptions;
    [SerializeField] private Menu soundOptions;

    [Header("Menu Elements")]

    [SerializeField] private OptionsBox optionsBox;
    [SerializeField] private StaticControl background;
    [SerializeField] private StaticControl backgroundMark;

    [Header("Menu Scene Effects")]

    [SerializeField] private StaticImage menuCurtain;
    [SerializeField] private RGBChannelForMenu rgbChannel;
    [SerializeField] private ShockText shockText;

    private int selectedChapterId;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        instance = this;
    }

    private void Start()
    {
        GameManager.gameScene = GameScene.Menu;

        titleMenu.SetActive(false);
        mainMenu.SetActive(false);
        chapterMenu.SetActive(false);
        optionsMenu.SetActive(false);
        aboutMenu.SetActive(false);
        modeOptions.SetActive(false);
        soundOptions.SetActive(false);
        optionsBox.SetActive(false);
        OptionsBox.current = optionsBox;

        StartCoroutine(InitMenuScene());
    }

    IEnumerator InitMenuScene()
    {
        SoundManager.instance.PlayUiEfx(UiEfx.FLASH);
        Curtain.instance.ChangeColor(0.5f, Curtain.black, Curtain.black_clear);
        menuCurtain.FadeOut(0.4f, 1.6f);
        yield return new WaitForSecondsRealtime(0.3f);

        shockText.MainShock(2f);
        rgbChannel.scale = 0.006f;        
        yield return new WaitForSecondsRealtime(1.5f);

        ShowTitleMenu();
        rgbChannel.Shake(0.008f, 0.4f);
        yield return new WaitForSecondsRealtime(0.6f);

        CheckBgm();
        shockText.AfterShock(0.3f);
        rgbChannel.Shake(0.01f, 0.3f);
    }
    
    //Show & hide functions

    public void ShowTitleMenu()
    {
        currentMenu = titleMenu;
        titleMenu.FadeIn(0.4f);
    }

    public void ShowMainMenu()
    {
        currentMenu = mainMenu;
        mainMenu.FadeIn(0.4f);
    }

    public void ShowChapterMenu()
    {
        currentMenu = chapterMenu;
        chapterMenu.FadeIn(0.6f);
    }

    public void ShowOptionsMenu()
    {
        currentMenu = optionsMenu;
        optionsMenu.FadeIn(0.6f);
    }

    public void ShowAboutMenu()
    {
        currentMenu = aboutMenu;
        aboutMenu.FadeIn(0.4f);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowBackground()
    {
        //background.gameObject.SetActive(true);
        background.FadeIn(0.2f);
        backgroundMark.FadeIn(0.2f, 0.2f);
    }

    public void HideBackground()
    {
        backgroundMark.FadeOut(0.2f);
        background.FadeOut(0.2f, 0.2f);
        //Invoke("DeactivateBackground", 0.2f);
    }

    void DeactivateBackground()
    {
        background.gameObject.SetActive(false);
    }

    void CheckBgm()
    {
        if (!SoundManager.instance.IsPlaying)
        {
            SoundManager.instance.PlayBgm(bgm);
            SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT, BGM_LOOP_LENGTH);
        }
    }

    //Title Menu Controls

    public void OnAnyButtonDown()
    {
        titleMenu.FadeOut(0.4f);
        Invoke("ShowMainMenu", 1.5f);
    }

    public void OnMainMenuCancel()
    {
        mainMenu.FadeOut(0.4f);
        Invoke("ShowTitleMenu", 0.6f);
    }

    //Main Menu Controls

    public void OnStartButtonDown()
    {
        if (!IsActivated) return;
        mainMenu.FadeOut(0.4f);
        if (GameManager.IsUnlocked(GameProgress.HackCh1))
        {
            Invoke("ShowBackground", 0.4f);
            Invoke("ShowChapterMenu", 0.6f);
        }
        else if (GameManager.IsUnlocked(GameProgress.StoryIntro))
        {
            GameManager.storyChapter = StoryChapter.Intro;
            SoundManager.instance.StopBgm(1f);
            Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, LoadStoryScene);
        }
        else
        {
            OnChapterSelected(0);
        }
    }

    public void OnOptionsButtonDown()
    {
        if (!IsActivated) return;
        mainMenu.FadeOut(0.4f);
        Invoke("ShowBackground", 0.4f);
        Invoke("ShowOptionsMenu", 0.6f);
    }

    public void OnAboutButtonDown()
    {
        if (!IsActivated) return;
        mainMenu.FadeOut(0.4f);
        Invoke("ShowAboutMenu", 0.4f);
    }

    public void OnExitButtonDown()
    {
        if (!IsActivated) return;
        mainMenu.FadeOut(0.4f);
        SoundManager.instance.StopBgm(2f);
        Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, ExitGame);
    }

    public void BackToMainMenu(bool isBackgroundOn)
    {
        currentMenu.FadeOut(0.4f);
        if (isBackgroundOn)
        {
            Invoke("HideBackground", 0.2f);
            Invoke("ShowMainMenu", 0.6f);
        }
        else
        {
            Invoke("ShowMainMenu", 0.4f);
        }
    }

    //Chapter Select Menu Controls

    public void OnContinueSelected()
    {
        switch (GameManager.lastPlayProgress)
        {
            case GameProgress.Prologue:
            case GameProgress.StoryIntro:
                OnStorySelected(0);
                break;
            case GameProgress.StroyCh1:
                OnStorySelected(1);
                break;
            case GameProgress.StoryCh2:
                OnStorySelected(2);
                break;
            case GameProgress.StoryCh3:
                OnStorySelected(3);
                break;
            case GameProgress.StoryCoda:
                OnStorySelected(4);
                break;
            case GameProgress.HackCh1:
                OnChapterSelected(1);
                break;
            case GameProgress.HackCh2:
                OnChapterSelected(2);
                break;
            case GameProgress.HackCh3:
                OnChapterSelected(3);
                break;
            case GameProgress.HackCh4:
                OnChapterSelected(4);
                break;
        }
    }

    public void OnChapterSelected(int chapterId)
    {
        currentMenu = null;
        selectedChapterId = chapterId;
        Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, LoadChapterScene);
        SoundManager.instance.StopBgm(0.5f);
    }

    public void OnStorySelected(int storyId)
    {
        currentMenu = null;
        switch (storyId)
        {
            case 0:
                GameManager.storyChapter = StoryChapter.Intro;
                break;
            case 1:
                GameManager.storyChapter = StoryChapter.Ch1;
                break;
            case 2:
                GameManager.storyChapter = StoryChapter.Ch2;
                break;
            case 3:
                GameManager.storyChapter = StoryChapter.Ch3;
                break;
            case 4:
                GameManager.storyChapter = StoryChapter.Coda;
                break;
            case 5:
                GameManager.storyChapter = StoryChapter.Coda_2;
                break;
        }

        Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, LoadStoryScene);
        /*
        if (storyId == 0)
            Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, LoadStoryScene);
        else
            Curtain.instance.ChangeColor(1f, Curtain.white_clear, Curtain.white, LoadStoryScene);
            */
        SoundManager.instance.StopBgm(0.5f);
    }

    public void LoadChapterScene()
    {
        GameManager.SetScene(GameScene.Loading);
        switch (selectedChapterId)
        {
            case 0:
                GameManager.SetScene(GameScene.Prologue, false);
                break;
            case 1:
                GameManager.SetScene(GameScene.Ch1, false);
                break;
            case 2:
                GameManager.SetScene(GameScene.Ch2, false);
                break;
            case 3:
                GameManager.SetScene(GameScene.Ch3, false);
                break;
            case 4:
                GameManager.SetScene(GameScene.Ch4, false);
                break;
            case 11:
                GameManager.SetScene(GameScene.Ex1, false);
                break;
            case 12:
                GameManager.SetScene(GameScene.Ex2, false);
                break;
            case 13:
                GameManager.SetScene(GameScene.Ex3, false);
                break;
            default:
                GameManager.SetScene(GameScene.Ch1, false);
                break;
        }
    }

    public void LoadStoryScene()
    {
        GameManager.SetScene(GameScene.Loading);
        GameManager.SetScene(GameScene.Story, false);
    }

    //Options Menu Controls

    public void OnModeOptionsDown()
    {
        currentMenu = modeOptions;
        modeOptions.FadeIn(0.3f);
    }

    public void OnSoundOptionsDown()
    {
        currentMenu = soundOptions;
        soundOptions.FadeIn(0.3f);
    }

    public void BackToOptionsMenu()
    {
        currentMenu.FadeOut(0.2f);
        optionsMenu.LastPressed.SetPressed(false);
        currentMenu = optionsMenu;
    }
}