using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ch1Scene : SceneController
{
    private const float BGM_LOOP_POINT = 17.18433f;
    private const float BGM_LOOP_LENGTH = 148.96545f;
    
#if OPEN_DEBUG_MODE
    private static int restartCount = 1;
    private static float restartTime = 42f;
#else
    private static int restartCount = 0;
    private static float restartTime = 42f;
#endif

    override public void ResetProgress()
    {
        base.ResetProgress();
        restartCount = 0;
    }
    
    [SerializeField] private AudioClip bgm;
    [SerializeField] private TextAsset startMessage;
    [SerializeField] private TextAsset restartMessage;

    [SerializeField] Light playerLight;
    [SerializeField] CubeGroup startDoor;
    [SerializeField] CubeGroup blackDoor;
    [SerializeField] InteractivePoint finalCheck;
    [SerializeField] private Transform hardCubes;
    [SerializeField] private Transform bombTraps;
    [SerializeField] private RedCube redCubePrefab;

    void Start()
    {
        IsSceneOver = false;
        GameManager.gameScene = GameScene.Ch1;
        GameManager.SetLastPlayProgress(GameProgress.HackCh1);
        HUDManager.instance.SetCamera(Camera.main);
        PauseMenu.instance.SetCamera(GameCamera.UI);

        Player.IsMovable = false;
        Player.IsArmed = false;
        playerLight.spotAngle = 45f;
        playerLight.intensity = 24f;

        blackDoor.SetActive(false);
        bombTraps.gameObject.SetActive(false);

        HUDManager.instance.HideTimer();
        Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear);

        if (!SoundManager.instance.IsPlaying)
        {
            SoundManager.instance.PlayBgm(bgm, 2f);
            SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT, BGM_LOOP_LENGTH);

        }

        //Preparation for difficulty level

        restartTime = GameManager.IsGameLevel(GameLevel.Easy) ? 120f : 42f;

        if (GameManager.IsGameLevelHigher(GameLevel.Hard))
        {
            int count = hardCubes.childCount;
            int trapCount = GameManager.IsGameLevel(GameLevel.VeryHard) ? 20 : 12;
            for (int i = 0; i < trapCount; i++)
            {
                Transform cube = hardCubes.GetChild(Random.Range(0, count));
                Instantiate(redCubePrefab, cube.position, Quaternion.identity, hardCubes);
                DestroyImmediate(cube.gameObject);
                count--;
            }
        }
        
        //Preparation with restart count

        if (restartCount == 0)
            StartCoroutine(StartPhase());
        else
            StartCoroutine(RestartPhase());
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

        IsSceneOver = true;
        Curtain.instance.ChangeColor(3f, Curtain.black_clear, Curtain.black, ReloadScene);
    }

    public void ReloadScene()
    {
        HUDManager.instance.ResetAll();
        GameManager.SetScene(GameScene.Ch1);
        restartCount++;
    }

    public void OnFinalChecked()
    {
        if (IsSceneOver) return;

        SoundManager.instance.PlayUiEfx(UiEfx.SCENE_CHANGE);
        HUDManager.instance.HideTimer();
        SoundManager.instance.StopBgm(1f);

        IsSceneOver = true;
        Curtain.instance.ChangeColor(2f, Curtain.white_clear, Curtain.white, CallNextScene);
    }

    public void CallNextScene()
    {
        ResetProgress();
        GameManager.storyChapter = StoryChapter.Ch1;
        GameManager.SetUnlockedProgress(GameProgress.StroyCh1);
        GameManager.SetScene(GameScene.Story);
    }

    //Coroutines for scene phases
    
    IEnumerator StartPhase()
    {
        Player.IsArmed = true;
        Player.IsMovable = true;

        Message[] messages = Message.GetMessages(startMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        startDoor.BlinkOut();
        blackDoor.BlinkIn();
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(PlayPhase());
    }

    IEnumerator RestartPhase()
    {
        Player.IsArmed = true;
        Player.IsMovable = true;
        
        Message[] messages = Message.GetMessages(restartMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        startDoor.BlinkOut();
        blackDoor.BlinkIn();
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(PlayPhase());
    }

    IEnumerator PlayPhase()
    {
        HUDManager.instance.SetTimer(restartTime, new HUDTimer.TimeOverHandler(OnStageFailed), Player.instance.transform);
        if (GameManager.IsGameLevel(GameLevel.VeryHard)) bombTraps.gameObject.SetActive(true);
        float time = 0f;
        while (!HUDManager.instance.IsTimeOver())
        {
            float angle = Mathf.Lerp(45f, 15f, time / restartTime);
            float intensity = Mathf.Lerp(24f, 8f, time / restartTime);
            playerLight.spotAngle = angle;
            playerLight.intensity = intensity;
            yield return null;
            time += Time.deltaTime;
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
                GameManager.SetScene(GameScene.Ch1);
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