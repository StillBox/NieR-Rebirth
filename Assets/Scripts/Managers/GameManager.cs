using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Dictionary<int, GameProgress> GameProgresses = new Dictionary<int, GameProgress>()
    {
        { (int)GameProgress.Prologue, GameProgress.Prologue },
        { (int)GameProgress.StoryIntro, GameProgress.StoryIntro },
        { (int)GameProgress.HackCh1, GameProgress.HackCh1 },
        { (int)GameProgress.StroyCh1, GameProgress.StroyCh1 },
        { (int)GameProgress.HackCh2, GameProgress.HackCh2 },
        { (int)GameProgress.StoryCh2, GameProgress.StoryCh2 },
        { (int)GameProgress.HackCh3, GameProgress.HackCh3 },
        { (int)GameProgress.StoryCh3, GameProgress.StoryCh3 },
        { (int)GameProgress.HackCh4, GameProgress.HackCh4 },
        { (int)GameProgress.StoryCoda, GameProgress.StoryCoda },
        { (int)GameProgress.MainClear, GameProgress.MainClear },
        { (int)GameProgress.AllClear, GameProgress.AllClear }
    };
    /*
    public static Dictionary<int, GameLevel> GameLevels = new Dictionary<int, GameLevel>()
    {
        { (int)GameLevel.EASY, GameLevel.EASY },
        { (int)GameLevel.NORMAL, GameLevel.NORMAL },
        { (int)GameLevel.HARD, GameLevel.HARD },
        { (int)GameLevel.VERY_HARD, GameLevel.VERY_HARD }
    };
    */
    public static GameManager instance = null;

    //Debug Mode Switch

    public static bool isDebugModeOn = false;
    public static bool isUIHidden = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            isDebugModeOn = !isDebugModeOn;
        }
        /*
        if (Input.GetKeyDown(KeyCode.F10))
        {
            isUIHidden = !isUIHidden;
            GameCamera.UI.cullingMask = isUIHidden ? 0 : LayerMask.GetMask("UI");
        }
        */
    }

    //For Scenes

    public static GameScene gameScene;
    public static StoryChapter storyChapter = StoryChapter.Coda;
    public static AsyncOperation operation = null;
    public static float LoadProgress { get { return operation.progress; } }
    
    public static void SetScene(GameScene scene, bool autoLoad = true, bool autoActivate = true)
    {
        gameScene = scene;
        if (autoLoad) LoadScene(autoActivate);    
    }

    public static void LoadScene(bool autoActivate = true)
    {
        string sceneName = GetSceneName();

        if (sceneName.Equals(SceneManager.GetActiveScene().name))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            if (autoActivate) ActivateScene();
        }
    }

    public static void ActivateScene()
    {
        instance.StartCoroutine(CheckProgress());
    }

    static IEnumerator CheckProgress()
    {
        while (LoadProgress < 0.9f) yield return null;
        operation.allowSceneActivation = true;
    }

    public static string GetSceneName()
    {
        switch (gameScene)
        {
            case GameScene.Logo:
                return "Logo";
            case GameScene.Loading:
                return "Loading";
            case GameScene.Prologue:
                return "Prologue";
            case GameScene.Menu:
                return "Menu";
            case GameScene.Story:
                return "Story";
            case GameScene.Ch1:
                return "Ch1";
            case GameScene.Ch2:
                return "Ch2";
            case GameScene.Ch3:
                return "Ch3";
            case GameScene.Ch4:
                return "Ch4";
            case GameScene.Ex1:
                return "Ex1";
            case GameScene.Ex2:
                return "Ex2";
            case GameScene.Ex3:
                return "Ex3";
            default:
                return null;
        }
    }

    //For Save Data

    public static GameProgress lastPlayProgress;
    public static GameProgress unlockedProgress;

    public static int ex1Clear;
    public static int ex2Clear;
    public static int ex3Clear;
    public static int infiniteLevel;

    //Game Settings Values and Keys

    public static int gameLevel;
    public static int screenGridOn;
    public static int rgbChannelOn;
    public static int fadeBorderOn;
    public static int miscEffectLevel;
    public static int directionAdjustOn;

    private static readonly string keyGameLevel = "GameLevel";

    //Volume Values and Keys

    public static float mainVolume;
    public static float bgmVolume;
    public static float efxVolume;

    private static readonly string keyBgmVolume = "BgmVolume";
    private static readonly string keyEfxVolume = "EfxVolume";

    public void LoadData()
    {
        //Sound volume

        mainVolume = 1f;
        bgmVolume = PlayerPrefs.GetFloat(keyBgmVolume, 0.6f);
        efxVolume = PlayerPrefs.GetFloat(keyEfxVolume, 0.8f);
        SoundManager.instance.MainVolume = mainVolume;
        SoundManager.instance.BgmVolume = bgmVolume;
        SoundManager.instance.EfxVolume = efxVolume;

        //Mode options

        gameLevel = PlayerPrefs.GetInt("GameLevel", 1);
        screenGridOn = PlayerPrefs.GetInt("ScreenGridOn", 1);
        rgbChannelOn = PlayerPrefs.GetInt("RGBChannelOn", 1);
        fadeBorderOn = PlayerPrefs.GetInt("FadeBorderOn", 1);
        miscEffectLevel = PlayerPrefs.GetInt("MiscEffectLevel", 2);
        directionAdjustOn = PlayerPrefs.GetInt("DirectionAdjustOn", 1);

        //Game progress

        lastPlayProgress = GameProgresses[PlayerPrefs.GetInt("LastPlay", 0)];
        unlockedProgress = GameProgresses[PlayerPrefs.GetInt("Unlocked", 0)];
    }

    public void SaveData()
    {
        PlayerPrefs.Save();
    }

    //Game progress

    public static void SetLastPlayProgress(GameProgress progress)
    {
        lastPlayProgress = progress;
        PlayerPrefs.SetInt("LastPlay", (int)lastPlayProgress);
    }

    public static void SetUnlockedProgress(GameProgress progress)
    {
        if (unlockedProgress.CompareTo(progress) <= 0)
            unlockedProgress = progress;
        PlayerPrefs.SetInt("Unlocked", (int)unlockedProgress);
    }

    public static bool IsUnlocked(GameProgress progress)
    {
        return unlockedProgress.CompareTo(progress) >= 0;
    }

    public static void SetExtraClear(int stage)
    {
        switch (stage)
        {
            case 1:
                ex1Clear = 1;
                PlayerPrefs.SetInt("Ex1Clear", 1);
                break;
            case 2:
                ex2Clear = 1;
                PlayerPrefs.SetInt("Ex2Clear", 1);
                break;
            case 3:
                ex3Clear = 1;
                PlayerPrefs.SetInt("Ex3Clear", 1);
                break;
        }
        if (ex1Clear == 1 && ex2Clear == 1 && ex3Clear == 1)
            SetUnlockedProgress(GameProgress.AllClear);
    }

    public static void SetInfiniteLevel(int level)
    {
        if (infiniteLevel < level) infiniteLevel = level;
        PlayerPrefs.SetInt("InfiniteLevel", infiniteLevel);
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey("LastPlay");
        PlayerPrefs.DeleteKey("Unlocked");
        PlayerPrefs.DeleteKey("Ex1Clear");
        PlayerPrefs.DeleteKey("Ex2Clear");
        PlayerPrefs.DeleteKey("Ex3Clear");
        PlayerPrefs.DeleteKey("InfiniteLevel");
        lastPlayProgress = GameProgress.Prologue;
        unlockedProgress = GameProgress.Prologue;
        ex1Clear = 0;
        ex2Clear = 0;
        ex3Clear = 0;
        infiniteLevel = 0;
    }

    //Mode options

    public static void SetLevel(int level)
    {
        gameLevel = level;
        PlayerPrefs.SetInt("GameLevel", gameLevel);
    }

    public static void SetScreenGridOn(int value)
    {
        screenGridOn = value;
        PlayerPrefs.SetInt("ScreenGridOn", screenGridOn);
    }

    public static void SetRGBChannelOn(int value)
    {
        rgbChannelOn = value;
        PlayerPrefs.SetInt("RGBChannelOn", screenGridOn);
    }

    public static void SetFadeBorderOn(int value)
    {
        fadeBorderOn = value;
        PlayerPrefs.SetInt("FadeBorderOn", fadeBorderOn);
    }

    public static void SetMiscEffectLevel(int value)
    {
        miscEffectLevel = value;
        PlayerPrefs.SetInt("MiscEffectLevel", miscEffectLevel);
    }

    public static void SetDirectionAdjustOn(int value)
    {
        directionAdjustOn = value;
        PlayerPrefs.SetInt("DirectionAdjustOn", directionAdjustOn);
    }

    public static bool IsGameLevel(GameLevel level)
    {
        return gameLevel == (int)level;
    }

    public static bool IsGameLevelHigher(GameLevel level, bool inclusive = true)
    {
        if (inclusive)
            return gameLevel >= (int)level;
        else
            return gameLevel > (int)level;
    }

    public static bool IsGameLevelLower(GameLevel level, bool inclusive = true)
    {
        if (inclusive)
            return gameLevel <= (int)level;
        else
            return gameLevel < (int)level;
    }

    public static bool IsScreenGridOn
    {
        set { SetScreenGridOn(value ? 1 : 0); }
        get { return screenGridOn == 1; }
    }

    public static bool IsRGBChannelOn
    {
        set { SetRGBChannelOn(value ? 1 : 0); }
        get { return rgbChannelOn == 1; }
    }

    public static bool IsFadeBorderOn
    {
        set { SetFadeBorderOn(value ? 1 : 0); }
        get { return fadeBorderOn == 1; }
    }

    public static bool IsMiscEffectOn
    {
        set { SetMiscEffectLevel(value ? 1 : 0); }
        get { return miscEffectLevel >= 1; }
    }

    public static int MiscEffectLevel
    {
        set { SetMiscEffectLevel(value); }
        get { return miscEffectLevel; }
    }

    public static bool IsDirectionAdjustOn
    {
        set { SetDirectionAdjustOn(value ? 1 : 0); }
        get { return directionAdjustOn == 1; }
    }

    //Sound volume

    public static void SetBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp(value, 0f, 1f);
        SoundManager.instance.BgmVolume = bgmVolume;
        PlayerPrefs.SetFloat(keyBgmVolume, bgmVolume);
    }

    public static void SetEfxVolume(float value)
    {
        efxVolume = Mathf.Clamp(value, 0f, 1f);
        SoundManager.instance.EfxVolume = efxVolume;
        PlayerPrefs.SetFloat(keyEfxVolume, efxVolume);
    }

    //For Monitoring

    static bool isGameMonitorOn = false;

    private void OnGUI()
    {
        if (!isDebugModeOn) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleLeft;

        if (GUI.Button(new Rect(0f, Screen.height - 30f, 160f, 30f), "Game Monitor"))
            isGameMonitorOn = !isGameMonitorOn;

        if (isGameMonitorOn)
        {
            Rect rect = new Rect(20f, Screen.height - 60f, 30f, 20f);
            GUI.Label(rect, "ScreenGrid : ", style);
            rect.x += 90f;
            if (GUI.Button(rect, IsScreenGridOn ? "On" : "Off")) IsScreenGridOn = !IsScreenGridOn;

            rect.x += 40f;
            GUI.Label(rect, "RGBChannel : ", style);
            rect.x += 100f;
            if (GUI.Button(rect, IsRGBChannelOn ? "On" : "Off")) IsRGBChannelOn = !IsRGBChannelOn;

            rect.x += 40f;
            GUI.Label(rect, "FadeBorder : ", style);
            rect.x += 90f;
            if (GUI.Button(rect, IsFadeBorderOn ? "On" : "Off")) IsFadeBorderOn = !IsFadeBorderOn;

            rect.x += 40f;
            GUI.Label(rect, "directionAdjust : ", style);
            rect.x += 110f;
            if (GUI.Button(rect, IsDirectionAdjustOn ? "On" : "Off")) IsDirectionAdjustOn = !IsDirectionAdjustOn;

            rect.x = 20f;
            rect.y -= 30f;
            GUI.Label(rect, "Difficulty : ", style);
            rect.x += 100f;
            rect.width = 400f;
            SetLevel(GUI.SelectionGrid(rect, gameLevel, new string[] { "Easy", "Normal", "Hard", "Very Hard" }, 4));

            rect.x = 20f;
            rect.y -= 30f;
            GUI.Label(rect, "Ex1 : " + (ex1Clear == 1 ? "O" : "X"), style);
            rect.x += 100f;
            GUI.Label(rect, "Ex2 : " + (ex2Clear == 1 ? "O" : "X"), style);
            rect.x += 100f;
            GUI.Label(rect, "Ex3 : " + (ex3Clear == 1 ? "O" : "X"), style);
            rect.x += 100f;
            rect.width = 98f;
            if (GUI.Button(rect, "All Clear"))
                for (int i = 1; i <= 3; i++) SetExtraClear(i);

            rect.x = 20f;
            rect.y -= 30f;
            GUI.Label(rect, "Unlocked : " + unlockedProgress.ToString(), style);
            rect.x += 200f;
            rect.width = 98f;
            if (GUI.Button(rect, "Reset")) ResetProgress();
            rect.x += 100f;
            if (GUI.Button(rect, "Main Clear")) SetUnlockedProgress(GameProgress.MainClear);

            rect.x = 20f;
            rect.y -= 30f;
            GUI.Label(rect, "Last Played : " + lastPlayProgress.ToString(), style);
            rect.x += 200f;
            System.Text.StringBuilder content = new System.Text.StringBuilder();
            content.Append("Current : ");
            content.Append(gameScene.ToString());
            if (gameScene == GameScene.Story) content.Append(" - " + storyChapter.ToString());
            GUI.Label(rect, content.ToString(), style);
        }
    }
}

public enum GameLevel
{
    Easy = 0,
    Normal,
    Hard,
    VeryHard
}

public enum GameScene
{
    Logo = 0,
    Loading,
    Prologue,
    Menu,
    Story,
    Ch1,
    Ch2,
    Ch3,
    Ch4,
    Ex1,
    Ex2,
    Ex3
}

public enum StoryChapter
{
    Intro = 0,
    Intro_1,
    Ch1,
    Ch1_1,
    Ch1_2,
    Ch2,
    Ch2_1,
    Ch3,
    Ch3_1,
    Ch3_2,
    Ch3_3,
    Coda,
    Coda_1,
    Coda_2
}

public enum GameProgress
{
    Prologue = 0,
    StoryIntro,
    HackCh1,
    StroyCh1,
    HackCh2,
    StoryCh2,
    HackCh3,
    StoryCh3,
    HackCh4,
    StoryCoda,
    MainClear,
    AllClear
}