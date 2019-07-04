using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ch3Scene : SceneController
{
    private const float BGM_LOOP_POINT = 7.0325f;
    private const float BGM_LOOP_LENGTH = 125.10633f;
    
#if OPEN_DEBUG_MODE
    private static int restartCount = 2;
    private static bool[] isClear =
    {
        false, false, false, false, false,
        false, false, false, false, false,
        false, false, false, false, false,
        false, false, false, false, false
    };
    private static int lastClear = -1;
#else
    private static int restartCount = 0;
    private static bool[] isClear =
    {
        false, false, false, false, false,
        false, false, false, false, false,
        false, false, false, false, false,
        false, false, false, false, false
    };
    private static int lastClear = -1;
#endif

    override public void ResetProgress()
    {
        base.ResetProgress();
        restartCount = 0;
        for (int i = 0; i < isClear.Length; i++)
        {
            isClear[i] = false;
        }
        lastClear = -1;
    }

    [System.Serializable]
    public class BridgeGroup
    {
        public Bridge[] bridges;

        public int Count
        {
            get { return bridges.Length; }
        }

        public void Close(bool blink = false)
        {
            for (int i = 0; i < Count; i++)
            {
                bridges[i].Close(blink);
            }
        }

        public void Open(bool blink = true)
        {
            for (int i = 0; i < Count; i++)
            {
                bridges[i].Open(blink);
            }
        }
    }

    [SerializeField] AudioClip bgm;
    [SerializeField] TextAsset startMessage;
    [SerializeField] TextAsset finalMessage;
    [SerializeField] TextAsset bonusMessage;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;

    [SerializeField] private Bridge startBridge;
    [SerializeField] private Bridge endBridge;
    [SerializeField] private EventTrigger[] triggers;
    [SerializeField] private BridgeGroup[] bridgeGroups;
    [SerializeField] private Transform[] platforms;
    [SerializeField] private InteractivePoint finalCheck;
    [SerializeField] private InteractivePoint[] secrets;
    [SerializeField] private CubeGroup redCubes;
    
    private bool isInBattle;
    public bool IsInBattle
    {
        get
        {
            return isInBattle;
        }
        set
        {
            AirWall.isBulletBlocked = value;
            Bridge.SetAccessible(!value);
            isInBattle = value;
        }
    }
    private Enemy currentEnemy;
    private int currentPlatform;

    void Start()
    {
        IsSceneOver = false;
        GameManager.gameScene = GameScene.Ch3;
        GameManager.SetLastPlayProgress(GameProgress.HackCh3);
        HUDManager.instance.SetCamera(Camera.main);
        PauseMenu.instance.SetCamera(GameCamera.UI);

        Player.IsMovable = true;
        Player.IsArmed = true;

        Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear);
        if (!SoundManager.instance.IsPlaying)
        {
            SoundManager.instance.PlayBgm(bgm, 2f);
            SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT, BGM_LOOP_LENGTH);
        }
        
        IsInBattle = false;
        currentEnemy = null;
        currentPlatform = -1;

        //Preparation for difficulty level

        if (GameManager.IsGameLevel(GameLevel.VeryHard))
        {
            for (int i = 0; i < isClear.Length; i++)
            {
                isClear[i] = false;
            }
            lastClear = -1;
        }

        //Preparation with restart count

        if (restartCount == 0)
        {
            StartCoroutine(StartPhase());
        }
        else
        {
            if (lastClear == -1)
            {
                startBridge.Open(false);
                Player.instance.transform.position = Vector3.zero;
            }
            else
            {
                for (int i = 0; i < isClear.Length; i++)
                {
                    if (isClear[i])
                    {
                        Debug.Log(i + " " + isClear[i]);
                        Destroy(triggers[i].gameObject);
                        bridgeGroups[i].Open(false);
                    }
                }
                Player.instance.transform.position = platforms[lastClear].position;
            }
            Camera.main.ImmediateTrack();
        }
    }

    private void Update()
    {
        if (currentPlatform != -1 && currentEnemy == null)
        {
            EnemyBulletManager.instance.DestroyAll();
            isClear[currentPlatform] = true;
            lastClear = currentPlatform;
            IsInBattle = false;

            if (currentPlatform == 17)
                endBridge.Open();
            else
                bridgeGroups[currentPlatform].Open();

            if (GameManager.IsGameLevelHigher(GameLevel.Hard))
            {
                redCubes.BlinkOut();
            }

            currentPlatform = -1;
        }

        if (STBInput.GetButtonDown("Pause"))
        {
            PauseMenu.instance.CallPauseMenu();
        }

        if (!IsSceneOver && Player.IsDeath)
        {
            OnStageFailed();
        }
    }

    public void OnStageFailed()
    {
        Player.IsMovable = false;
        Player.IsArmed = false;
        if (GameManager.IsGameLevel(GameLevel.Easy))
        {
            ResetPlayer();
        }
        else
        {
            IsSceneOver = true;
            Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, ReloadScene);
        }
    }

    public void ReloadScene()
    {
        HUDManager.instance.ResetAll();
        GameManager.SetScene(GameScene.Ch3);
        restartCount++;
    }

    public void ResetPlayer()
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        if (IsInBattle)
        {
            position = platforms[currentPlatform].transform.position;
        }
        else
        {
            position = platforms[lastClear].transform.position;
        }
        EffectManager.instance.SetSearchLight(position, false);
        Destroy(Player.instance.gameObject);
        Instantiate(playerPrefab, position, rotation);
        Camera.main.ImmediateTrack();
        EnemyBulletManager.instance.DestroyAll();
        Player.IsMovable = true;
        Player.IsArmed = true;
    }

    public void OnPlatformTriggerEntered(int index)
    {
        IsInBattle = true;
        currentPlatform = index;
        Vector3 position = platforms[index].position;

        currentEnemy = EnemyManager.instance.SetEnemy(enemyPrefab, position);
        currentEnemy.healthPoint = GameManager.IsGameLevel(GameLevel.Easy) ? 4 :
            GameManager.IsGameLevel(GameLevel.Normal) ? 5 : 7;

        if (GameManager.IsGameLevelHigher(GameLevel.Hard))
        {
            redCubes.transform.position = position;
            redCubes.BlinkIn();
        }
    }

    public void OnFinalChecked()
    {
        if (IsSceneOver) return;

        StartCoroutine(FinalPhase());
    }

    public void OnSecretChecked(int index)
    {
        Message message = Message.GetMessage(bonusMessage, index);
        HUDManager.instance.ShowMessage(message);
    }

    public void CallNextScene()
    {
        ResetProgress();
        Bridge.ClearAll();
        GameManager.storyChapter = StoryChapter.Ch3;
        GameManager.SetUnlockedProgress(GameProgress.StoryCh3);
        GameManager.SetScene(GameScene.Story);
    }

    IEnumerator StartPhase()
    {
        Player.IsArmed = true;
        Player.IsMovable = true;

        Message[] messages = Message.GetMessages(startMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        startBridge.Open();
    }

    IEnumerator FinalPhase()
    {
        SoundManager.instance.StopBgm(2f);

        Message[] messages = Message.GetMessages(finalMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;

        if (IsSceneOver) yield break;
        SoundManager.instance.PlayUiEfx(UiEfx.SCENE_CHANGE);

        IsSceneOver = true;
        Curtain.instance.ChangeColor(2f, Curtain.white_clear, Curtain.white, CallNextScene);
    }

    //For Monitoring

    static bool isSceneMonitorOn = false;

    private void OnGUI()
    {
        if (!GameManager.isDebugModeOn) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleLeft;

        if (GUI.Button(new Rect(Screen.width - 160f, Screen.height - 30f, 160f, 30f), "Scene Monitor"))
            isSceneMonitorOn = !isSceneMonitorOn;

        if (isSceneMonitorOn)
        {
            Rect rect = new Rect(Screen.width - 160f, Screen.height - 60f, 80f, 20f);
            if (GUI.Button(rect, "Retry"))
            {
                ResetProgress();
                GameManager.SetScene(GameScene.Ch3);
            }
            rect.x += 80f;
            if (GUI.Button(rect, "Skip"))
            {
                OnFinalChecked();
            }

            rect.x = Screen.width - 160f;
            rect.y -= 30f;
            GUI.Label(rect, "Player Invincible", style);
            rect.x += 120f;
            rect.width = 40f;
            if (GUI.Button(rect, Player.IsSuper ? "On" : "Off"))
            {
                Player.IsSuper = !Player.IsSuper;
            }
        }
    }
}