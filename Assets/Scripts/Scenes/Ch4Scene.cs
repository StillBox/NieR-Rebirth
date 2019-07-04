using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ch4Scene : SceneController
{
    private readonly float[] BGM_LOOP_POINT = { 9.865f, 15.730f, 113.37f };
    private readonly float[] BGM_LOOP_LENGTH = { 129.23075f, 107.02712f, 210.81f };
    private const float BGM_CHORUS_POINT = 71.4f;
    private const float BGM_OFFSET = 0.00623f;
    
#if OPEN_DEBUG_MODE
    private static int restartCount = 4;
    private static int wingCount = -2;
    private static bool isRecoverEntered = true;
    private static bool isFinalEntered = false;
    private static bool isChorusStarted = false;
    private static float lastProgress = 0f;
    private static float[] checkedProgresses = { 0f, 0f, 0f, 0f };
    private static int currentBgm = -1;
#else
    private static int restartCount = 0;
    private static int wingCount = -2;
    private static bool isRecoverEntered = false;
    private static bool isFinalEntered = false;
    private static bool isChorusStarted = false;
    private static float lastProgress = 0f;
    private static float[] checkedProgresses = { 0f, 0f, 0f, 0f };
    private static int currentBgm = -1;
#endif

    override public void ResetProgress()
    {
        base.ResetProgress();
        Bridge.ClearAll();
        restartCount = 0;
        wingCount = -2;
        isRecoverEntered = false;
        isFinalEntered = false;
        isChorusStarted = false;
        lastProgress = 0f;
        for (int i = 0; i < 4; i++)
        {
            checkedProgresses[i] = 0f;
        }
    }

    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private TextAsset hintMessage;
    [SerializeField] private TextAsset startMessage;
    [SerializeField] private TextAsset finalMessage;
    [SerializeField] private TextAsset supportMessage;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private EnemyGroup[] enemyGroups;
    [SerializeField] private Boss enemyBoss;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Enemy finalEnemyPrefab;

    [SerializeField] private Bridge bridge;
    [SerializeField] private EventTrigger enterTrigger;
    [SerializeField] private CubeGroup[] cubeGroups;
    [SerializeField] private InteractivePoint[] checkPoints;
    [SerializeField] private EventTrigger finalTrigger;
    [SerializeField] private InteractivePoint finalCheck;
    [SerializeField] private GameObject finalPlatform;
    
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

    private bool isInRecoverPhase;
    private bool isInFinalPhase;
    private int checkedCount;
    
    void Start()
    {
        IsSceneOver = false;
        GameManager.gameScene = GameScene.Ch4;
        GameManager.SetLastPlayProgress(GameProgress.HackCh4);
        HUDManager.instance.SetCamera(Camera.main);
        PauseMenu.instance.SetCamera(GameCamera.UI);

        Player.IsMovable = true;
        Player.IsArmed = true;
        
        Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear);
        if (!SoundManager.instance.IsPlaying)
        {
            currentBgm = isFinalEntered ? 1 : 0;
            SoundManager.instance.PlayBgm(bgms[currentBgm], 2f);
        }

        IsInBattle = false;
        isInRecoverPhase = false;
        isInFinalPhase = false;
        checkedCount = 0;
        enemyBoss.SetActive(false);

        if (restartCount == 0)
        {
            StartCoroutine(StartPhase());
        }
        else
        {
            if (isFinalEntered)
            {
                bridge.Close();
                Destroy(enterTrigger.gameObject);
                foreach (CubeGroup cubeGroup in cubeGroups) Destroy(cubeGroup.gameObject);
                foreach (EnemyGroup enemyGroup in enemyGroups) Destroy(enemyGroup.gameObject);
                foreach (InteractivePoint checkPoint in checkPoints) Destroy(checkPoint.gameObject);
                Destroy(enemyBoss.gameObject);

                finalTrigger.SetActive(true);
                finalCheck.SetActive(true);
                finalCheck.Progress = lastProgress;

                Vector3 position = finalCheck.transform.position;
                position.z -= 5f;
                Player.instance.transform.position = position;
                Camera.main.ImmediateTrack();

                if (wingCount >= 1)
                {
                    Message[] supports = Message.GetMessages(supportMessage);
                    Message message = wingCount <= 4 ? supports[wingCount - 1] :
                        supports[Random.Range(4, supports.Length)];
                    HUDManager.instance.ShowMessage(message);
                    Player.instance.SetWingmen(Mathf.Min(4, wingCount));
                }
            }
            else
            {
                bridge.Open(false);
                for (int i = 0; i < 4; i++)
                {
                    checkPoints[i].Progress = checkedProgresses[i];
                }
            }
        }
    }

    void Update()
    {
        //For BGM states

        float current = SoundManager.instance.Current;
        if (isFinalEntered)
        {
            if (wingCount <= 0)
            {
                if (current >= BGM_LOOP_POINT[1] + BGM_LOOP_LENGTH[1])
                {
                    SoundManager.instance.SeekTo(current - BGM_LOOP_LENGTH[1]);
                }
            }
            else
            {
                if (!isChorusStarted)
                {
                    if (current < BGM_CHORUS_POINT)
                    {
                        isChorusStarted = true;
                    }
                    else
                    {
                        if (current >= BGM_LOOP_POINT[1] + BGM_LOOP_LENGTH[1])
                        {
                            SoundManager.instance.SeekTo(current - BGM_LOOP_LENGTH[1]);
                        }
                    }
                }
                else
                {
                    if (currentBgm != 2)
                    {
                        if (current >= BGM_CHORUS_POINT)
                        {
                            currentBgm = 2;
                            SoundManager.instance.PlayBgm(bgms[2]);
                            SoundManager.instance.SeekTo(current + BGM_OFFSET);
                        }
                    }
                    else
                    {
                        if (current >= BGM_LOOP_POINT[2] + BGM_LOOP_LENGTH[2])
                        {
                            SoundManager.instance.PlayBgm(bgms[2], 3f);
                            SoundManager.instance.SeekTo(current - BGM_LOOP_LENGTH[2]);
                        }
                    }
                }
            }
        }
        else
        {
            if (current >= BGM_LOOP_POINT[0] + BGM_LOOP_LENGTH[0])
            {
                SoundManager.instance.SeekTo(current - BGM_LOOP_LENGTH[0]);
            }
        }

        //For Enemy States

        if (isInRecoverPhase)
        {
            if (checkedCount < 4)
            {
                for (int i = EnemyManager.instance.Count; i < 4 + checkedCount; i++)
                {
                    EnemyManager.instance.SetEnemy(enemyPrefab, GetRandomPosition());
                }
            }
            else
            {
                if (EnemyManager.instance.IsEmpty)
                {
                    isInRecoverPhase = false;
                    enemyBoss.IsInvincible = false;
                }
            }
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

        if (GameManager.IsGameLevel(GameLevel.Easy) && !isFinalEntered)
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
        restartCount++;

        if (isFinalEntered)
        {
            wingCount++;
            float progressCount = GameManager.IsGameLevel(GameLevel.Easy) ? 16f :
                GameManager.IsGameLevel(GameLevel.Normal) ? 8f :
                GameManager.IsGameLevel(GameLevel.Hard) ? 4f : 2f;
            lastProgress = (int)(finalCheck.Progress * progressCount) / progressCount;

            HUDManager.instance.ResetAll();
            GameManager.SetScene(GameScene.Ch4);
        }
        else
        {
            if (GameManager.IsGameLevel(GameLevel.VeryHard))
            {
                for (int i = 0; i < 4; i++)
                {
                    checkedProgresses[i] = 0f;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    checkedProgresses[i] = checkPoints[i].Progress;
                }
            }
            HUDManager.instance.ResetAll();
            GameManager.SetScene(GameScene.Ch4);            
        }
    }

    public void ResetPlayer()
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        position = finalPlatform.transform.position;
        position.z -= 10f;
        EffectManager.instance.SetSearchLight(position, false);
        Destroy(Player.instance.gameObject);
        Instantiate(playerPrefab, position, rotation);
        Camera.main.ImmediateTrack();
        EnemyBulletManager.instance.DestroyAll();
        Player.IsMovable = true;
        Player.IsArmed = true;
    }

    public void OnPlatformTriggerEntered()
    {
        if (restartCount == 0)
        {
            Message message = Message.GetMessage(hintMessage, 0);
            HUDManager.instance.ShowMessage(message);
        }

        IsInBattle = true;
        enemyBoss.SetActive(true);
        bridge.Close(true);
        if (GameManager.IsGameLevel(GameLevel.Normal) && isRecoverEntered)
        {
            foreach (CubeGroup cubeGroup in cubeGroups) Destroy(cubeGroup.gameObject);
            foreach (EnemyGroup enemyGroup in enemyGroups) Destroy(enemyGroup.gameObject);
            enemyBoss.WeaponLevel = 2;
            isInRecoverPhase = true;
        }
        else
        {
            cubeGroups[0].BlinkIn();
            enemyGroups[0].Activate();
        }
    }

    public void OnEnemyCleared(int index)
    {
        cubeGroups[index].BlinkOut();
        if (index < 2)
        {
            cubeGroups[index + 1].BlinkIn();
            enemyGroups[index + 1].Activate();
            enemyBoss.WeaponLevel = index + 1;
        }
        else
        {
            isRecoverEntered = true;
            isInRecoverPhase = true;
        }
    }

    public void OnPointChecked()
    {
        checkedCount++;
    }

    public void OnBossDestroyed()
    {
        if (!isFinalEntered)
        {
            IsInBattle = false;
            SoundManager.instance.StopBgm(5f);
            StartCoroutine(EnterFinalPhase());
            isFinalEntered = true;
        }
    }

    public void OnFinalTriggerEntered()
    {
        if (!SoundManager.instance.IsPlaying)
            SoundManager.instance.PlayBgm(bgms[1], 2f);
        Player.IsMovable = false;
        isInFinalPhase = true;
        StartCoroutine(MovePlayerToCenter());
        StartCoroutine(FinalPhase());
    }

    public void OnFinalChecked()
    {
        EnemyManager.instance.DestroyAll();
        EnemyBulletManager.instance.DestroyAll();

        if (IsSceneOver) return;

        SoundManager.instance.PlayUiEfx(UiEfx.SCENE_CHANGE);
        SoundManager.instance.StopBgm(4f);
        SlowDown(0.3f, 5f);

        IsSceneOver = true;
        Curtain.instance.ChangeColor(5f, new Color(1f, 1f, 1f, 0.5f), Curtain.white, CallNextScene);
    }

    public void CallNextScene()
    {
        ResetProgress();
        Bridge.ClearAll();
        GameManager.storyChapter = StoryChapter.Coda;
        GameManager.SetUnlockedProgress(GameProgress.StoryCoda);
        GameManager.SetScene(GameScene.Story);
    }
    
    Vector3 GetRandomPosition()
    {
        Vector3 center = finalPlatform.transform.position;
        center.y = 0f;

        float width = finalPlatform.transform.localScale.x;
        float height = finalPlatform.transform.localScale.z;
        Vector3 offset = new Vector3()
        {
            x = Random.Range(-width / 2f + 2f, width / 2f - 2f),
            y = 0f,
            z = Random.Range(-height / 2f + 2f, height / 2f - 2f)
        };
        if (offset.magnitude <= 3f)
            offset = offset.normalized * 3f;
        
        return center + offset;
    }

    void SetCameraOffset(float heightScale)
    {
        Camera.main.SetTrackOffset(0f, 16f * (1f + heightScale), -7f);
    }

    IEnumerator StartPhase()
    {
        Player.IsArmed = true;
        Player.IsMovable = true;

        Message[] messages = Message.GetMessages(startMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        bridge.Open();
    }

    IEnumerator EnterFinalPhase()
    {
        Player.IsSuper = false;

        Message[] messages = Message.GetMessages(finalMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(2f);

        finalCheck.SetActive(true);
        finalTrigger.SetActive(true);
    }

    IEnumerator MovePlayerToCenter()
    {
        Vector3 target = finalCheck.transform.position;
        Vector3 offset = target - Player.instance.transform.position;
        while (finalCheck.Progress < 1f)
        {
            Vector3 movement = offset;
            if (movement.magnitude >= 0.1f)
                movement *= 0.1f / movement.magnitude;
            offset -= movement;
            Player.instance.transform.position = target - offset;
            yield return null;
        }
    }

    IEnumerator FinalPhase()
    {
        float time = 0f;
        float progress = finalCheck.Progress;
        while (progress < 1f)
        {
            float gap = 2.5f;
            for (int i = 1; i < 4; i++)
            {
                if (progress > 0.2f * i) gap *= (1f - progress) / (1f - 0.2f * i);
            }
            gap = Mathf.Clamp(gap, 0.2f, 2.5f);
            //gap = Mathf.Max(0.2f, 2.5f * (1f - Mathf.Pow(finalCheck.Progress, 0.4f)));

            time += Time.deltaTime;
            if (time >= gap)
            {
                EnemyManager.instance.SetEnemy(finalEnemyPrefab, GetRandomPosition());
                time -= gap;
            }
            SetCameraOffset(progress);
            if (!Player.IsDeath && progress >= 0.95f)
            {
                Color color = Color.white;
                color.a = 10f * (progress - 0.95f);
                Curtain.instance.SetColor(color);
            }

            yield return null;
            progress = finalCheck.Progress;
        }
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
                GameManager.SetScene(GameScene.Ch4);
            }
            rect.x += 80f;
            if (GUI.Button(rect, "Skip"))
            {
                OnFinalChecked();
            }

            if (!(isFinalEntered && wingCount < 4))
            {
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
}