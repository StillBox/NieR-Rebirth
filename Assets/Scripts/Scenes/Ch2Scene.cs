using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ch2Scene : SceneController
{
    private const float BGM_LOOP_POINT = 24.6418f;
    private const float BGM_LOOP_LENGTH = 168f;
    
#if OPEN_DEBUG_MODE
    private static int restartCount = 1;
    //private static bool[] isClear = { false, false, false, false, false, false };
    private static bool[] isClear = { true, true, true, true, true, true };
#else

    private static int restartCount = 0;
    private static bool[] isClear = { false, false, false, false, false, false };
#endif

    override public void ResetProgress()
    {
        base.ResetProgress();
        restartCount = 0;
        for (int i = 0; i < isClear.Length; i++)
        {
            isClear[i] = false;
        }
    }

    [SerializeField] private AudioClip bgm;
    [SerializeField] private TextAsset startMessage;
    [SerializeField] private TextAsset hintMessage;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private EnemyGroup[] enemyGroups;
    [SerializeField] private Boss enemyBoss;

    [SerializeField] private CubeGroup startDoor;
    [SerializeField] private CubeGroup[] majorDoors;
    [SerializeField] private EventTrigger[] majorTriggers;
    [SerializeField] private EventTrigger hintTrigger;
    [SerializeField] private CubeGroup[] platDoors;
    [SerializeField] private EventTrigger[] platTriggers;
    [SerializeField] private InteractivePoint[] checkPoints;
    [SerializeField] private CubeGroup[] finalCubes;

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
            isInBattle = value;
        }
    }
    private int checkedCount;
    private int currentPlatform;

    protected override void Awake()
    {
        base.Awake();        
        for (int i = 0; i < checkPoints.Length; i++)
        {
            checkPoints[i].SetActive(false);
        }
    }

    void Start()
    {
        IsSceneOver = false;
        GameManager.gameScene = GameScene.Ch2;
        GameManager.SetLastPlayProgress(GameProgress.HackCh2);
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
        checkedCount = 0;
        for (int i = 0; i < 6; i++)
        {
            if (isClear[i])
            {
                checkedCount++;
            }
        }

        //Preparation for difficulty level

        if (GameManager.IsGameLevel(GameLevel.Hard))
        {
            if (checkedCount < 2)
            {
                for (int i = 0; i < 2; i++) isClear[i] = false;
                checkedCount = 0;
            }
            else if (checkedCount < 6)
            {
                for (int i = 2; i < 6; i++) isClear[i] = false;
                checkedCount = 2;
            }
        }
        else if (GameManager.IsGameLevel(GameLevel.VeryHard))
        {
            for (int i = 0; i < 6; i++) isClear[i] = false;
            checkedCount = 0;
        }

        //Preparation with restart count

        if (restartCount == 0)
            StartCoroutine(StartPhase());
        else
        {
            for (int i = 0; i < 6; i++)
            {
                if (isClear[i])
                {
                    Destroy(platDoors[i].gameObject);
                    Destroy(platTriggers[i].gameObject);
                    Destroy(checkPoints[i].gameObject);
                    Destroy(enemyGroups[i].gameObject);
                }
            }

            Player.instance.transform.position = majorTriggers[0].transform.position;
            Destroy(startDoor.gameObject);
            Destroy(hintTrigger.gameObject);

            if (checkedCount >= 2)
            {
                Player.instance.transform.position = majorTriggers[1].transform.position;
                Destroy(majorDoors[0].gameObject);
                Destroy(majorTriggers[0].gameObject);
            }
            if (checkedCount >= 6)
            {
                Destroy(majorDoors[1].gameObject);
                Destroy(majorTriggers[1].gameObject);
            }
            Camera.main.ImmediateTrack();
        }
    }

    void Update()
    {
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

    private void ResetPlayer()
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        if (currentPlatform == 6)
        {
            position = platDoors[6].transform.position;
            position.z += 2f;
        }
        else
        {
            position = platDoors[currentPlatform].transform.position;
            if (currentPlatform % 2 == 0)
            {
                position.x -= 2f;
                rotation = Quaternion.LookRotation(Vector3.left);
            }
            else
            {
                position.x += 2f;
                rotation = Quaternion.LookRotation(Vector3.right);
            }
        }
        EffectManager.instance.SetSearchLight(position, false);
        Destroy(Player.instance.gameObject);
        Instantiate(playerPrefab, position, rotation);
        Camera.main.ImmediateTrack();
        EnemyBulletManager.instance.DestroyAll();
        Player.IsMovable = true;
        Player.IsArmed = true;
    }

    public void ReloadScene()
    {
        HUDManager.instance.ResetAll();
        GameManager.SetScene(GameScene.Ch2);
        restartCount++;
    }

    public void OnPlatformTriggerEntered(int index)
    {
        IsInBattle = true;
        currentPlatform = index;

        platDoors[index].BlinkIn();
        enemyGroups[index].Activate();

        if (index == 6)
        {
            if (GameManager.IsGameLevelHigher(GameLevel.Hard))
            {
                finalCubes[2].BlinkIn();
            }
            else
            {
                finalCubes[0].BlinkIn();
                finalCubes[1].BlinkIn();
            }
            enemyBoss.SetActive(true);
        }
    }

    public void OnMajorPlarformTriggerEntered(int index)
    {
        majorDoors[index].BlinkOut();
    }

    public void ShowHintMessage()
    {
        Message hint = Message.GetMessage(hintMessage, 0);
        HUDManager.instance.ShowMessage(hint);
    }

    public void OnEnemyCleared(int index)
    {
        if (index < 6)
        {
            IsInBattle = false;
            checkPoints[index].SetActive(true);
        }
        else
        {
            switch (index)
            {
                case 6:
                case 7:
                    enemyBoss.WeaponLevel = index - 5;
                    enemyGroups[index + 1].Activate();
                    break;
                case 8:
                    enemyBoss.IsInvincible = false;
                    break;
            }
        }
    }

    public void OnBossDestroyed()
    {
        IsInBattle = false;
        checkPoints[6].SetActive(true);
    }

    public void OnPlatformChecked(int index)
    {
        platDoors[index].BlinkOut();
        checkedCount++;
        isClear[index] = true;

        if (checkedCount == 2)
            majorTriggers[0].SetActive(true);
        if (checkedCount == 6)
            majorTriggers[1].SetActive(true);
    }

    public void OnFinalChecked()
    {
        if (IsSceneOver) return;

        SoundManager.instance.PlayUiEfx(UiEfx.SCENE_CHANGE);
        SoundManager.instance.StopBgm(1f);

        IsSceneOver = true;
        Curtain.instance.ChangeColor(2f, Curtain.white_clear, Curtain.white, CallNextScene);
    }

    public void CallNextScene()
    {
        ResetProgress();
        GameManager.storyChapter = StoryChapter.Ch2;
        GameManager.SetUnlockedProgress(GameProgress.StoryCh2);
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

        startDoor.BlinkOut();
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
                GameManager.SetScene(GameScene.Ch2);
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