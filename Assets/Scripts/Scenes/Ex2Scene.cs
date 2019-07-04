using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex2Scene : SceneController
{
    private const float BOSS_HP = 180f;
    private const int BOSS_ID_POPOLA = 0;
    private const int BOSS_ID_DEVOLA = 1;

    private const float BGM_LOOP_POINT = 23.33f;
    private const float BGM_LOOP_LENGTH = 270.274f;
    private const float SPB = 60f / 127.9f;
    
    [SerializeField] private AudioClip bgm;
    [SerializeField] private EnemyBossEx2 popola;
    [SerializeField] private EnemyBossEx2 devola;

    private static int restartCount = 0;

    override public void ResetProgress()
    {
        base.ResetProgress();
        restartCount = 0;
    }

    void Start ()
    {
        IsSceneOver = false;
        GameManager.gameScene = GameScene.Ex2;
        HUDManager.instance.SetCamera(Camera.main);
        PauseMenu.instance.SetCamera(GameCamera.UI);

        Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear);

        if (!SoundManager.instance.IsPlaying)
        {
            SoundManager.instance.PlayBgm(bgm, 2f);
            SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT, BGM_LOOP_LENGTH);
        }

        //Init of enemies

        HUDManager.instance.AddHPBar(BOSS_ID_POPOLA, "POPOLA", 0.21f, -0.06f, 0.3125f);
        HUDManager.instance.AddHPBar(BOSS_ID_DEVOLA, "DEVOLA", -0.21f, -0.06f, 0.3125f);

        popola.healthPoint = (int)BOSS_HP;
        devola.healthPoint = (int)BOSS_HP;

        StartCoroutine(PrePhase());
        StartCoroutine(CheckBossStates());
    }

    void Update ()
    {
        if (STBInput.GetButtonDown("Pause"))
        {
            PauseMenu.instance.CallPauseMenu();
        }

        if (!IsSceneOver && Player.IsDeath)
        {
            OnStageFailed();
        }

        HUDManager.instance.SetHP(BOSS_ID_POPOLA, popola.healthPoint / BOSS_HP);
        HUDManager.instance.SetHP(BOSS_ID_DEVOLA, devola.healthPoint / BOSS_HP);        
    }

    public void OnStageFailed()
    {
        Player.IsMovable = false;
        Player.IsArmed = false;

        IsSceneOver = true;
        Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, ReloadScene);
    }

    public void ReloadScene()
    {
        restartCount++;
        HUDManager.instance.ResetAll();
        GameManager.SetScene(GameScene.Ex2);
    }

    public void CallNextScene()
    {
        ResetProgress();
        GameManager.SetExtraClear(2);
        GameManager.SetScene(GameScene.Menu);
    }

    IEnumerator PrePhase()
    {
        if (restartCount == 0)
        {
            Player.IsMovable = false;
            Player.IsArmed = false;

            Player.instance.transform.position = new Vector3(0f, 0f, -11f);
            popola.transform.position = new Vector3(-2.5f, 0f, 16f);
            devola.transform.position = new Vector3(2.5f, 0f, 16f);

            Player.instance.MoveTo(new Vector3(0f, 0f, -3f), SPB * 8f);

            while (SoundManager.instance.Current <= 0.8f)
                yield return null;

            popola.StartPrePhase(SPB, 16f);
            devola.StartPrePhase(SPB, 18f);
            yield return new WaitForSeconds(SPB * 6f);

            SetTrack(false);
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.z = 3f;
            StartCoroutine(MoveCameraTo(cameraPos, SPB * 1.8f));
            popola.MoveTo(new Vector3(-2.5f, 0f, 3f), SPB * 7f);
            devola.MoveTo(new Vector3(2.5f, 0f, 3f), SPB * 7f);
            yield return new WaitForSeconds(SPB * 2f);

            cameraPos.z = -11f;
            StartCoroutine(MoveCameraTo(cameraPos, SPB * 5f));
            yield return new WaitForSeconds(SPB * 5f);
            
            HUDManager.instance.ShowHPBar(BOSS_ID_POPOLA);
            HUDManager.instance.ShowHPBar(BOSS_ID_DEVOLA);

            SetTrack(true);
            Player.IsMovable = true;
            Player.IsArmed = true;

            yield return new WaitForSeconds(SPB * 35f);
            StartCoroutine(MainLoop());
        }
        else
        {
            Player.instance.transform.position = new Vector3(0f, 0f, -3f);
            popola.transform.position = new Vector3(-2.5f, 0f, 3f);
            devola.transform.position = new Vector3(2.5f, 0f, 3f);

            Player.IsMovable = true;
            Player.IsArmed = true;

            float timePast = SoundManager.instance.Current - 0.8f;
            yield return new WaitForSeconds(timePast % SPB);

            HUDManager.instance.ShowHPBar(BOSS_ID_POPOLA);
            HUDManager.instance.ShowHPBar(BOSS_ID_DEVOLA);

            popola.StartPrePhase(SPB, 5f);
            devola.StartPrePhase(SPB, 7f);

            yield return new WaitForSeconds(SPB * 37f);
            StartCoroutine(MainLoop());
        }
    }

    IEnumerator MainLoop()
    {
        popola.StartOrbitClosePhase(SPB, 40f, 5f, 5f);
        devola.StartOrbitClosePhase(SPB, 40f, 5f, 5f);
        yield return new WaitForSeconds(SPB * 50f);

        popola.StartSentryPhase(SPB, 42f, 3f, 3f);
        devola.StartShieldPhase(SPB, 42f, 3f, 3f);
        yield return new WaitForSeconds(SPB * 48f);
        
        popola.StartOrbitApartPhase(SPB, 72f, 18f, 18f);
        devola.StartOrbitApartPhase(SPB, 72f, 18f, 18f);
        yield return new WaitForSeconds(SPB * 108f);
        
        popola.StartDashPhase(SPB, 18f, 4, 6f, 6f);
        devola.StartDashPhase(SPB, 18f, 4, 6f, 6f);
        yield return new WaitForSeconds(SPB * 84f);

        popola.StartOrbitClosePhase(SPB, 40f, 5f, 5f);
        devola.StartOrbitClosePhase(SPB, 40f, 5f, 5f);
        yield return new WaitForSeconds(SPB * 50f);

        popola.StartShieldPhase(SPB, 42f, 3f, 3f);
        devola.StartSentryPhase(SPB, 42f, 3f, 3f);
        yield return new WaitForSeconds(SPB * 48f);

        popola.StartOrbitApartPhase(SPB, 72f, 18f, 18f);
        devola.StartOrbitApartPhase(SPB, 72f, 18f, 18f);
        yield return new WaitForSeconds(SPB * 108f);

        popola.StartDashPhase(SPB, 18f, 4, 6f, 6f);
        devola.StartDashPhase(SPB, 18f, 4, 6f, 6f);
        yield return new WaitForSeconds(SPB * 84f);

        StartCoroutine(MainLoop());
    }

    IEnumerator BinaryOrbitPhase()
    {
        popola.StartOrbitClosePhase(SPB, 30f, 5f, 5f);
        devola.StartOrbitClosePhase(SPB, 30f, 5f, 5f);
        yield return new WaitForSeconds(SPB * 40f);
    }

    IEnumerator SentryPhase(int type)
    {

        yield return new WaitForSeconds(0f);
    }

    IEnumerator CheckBossStates()
    {
        while (popola.healthPoint != 0 || devola.healthPoint != 0)
        {
            if (devola.healthPoint == 0 || popola.healthPoint == 0)
            {
                popola.BurstLevel = 3;
            }
            else if (popola.healthPoint - devola.healthPoint > 0.5f * BOSS_HP)
            {
                popola.BurstLevel = 3;
            }
            else if (popola.healthPoint - devola.healthPoint > 0.2f * BOSS_HP)
            {
                popola.BurstLevel = 2;
            }
            else if (popola.healthPoint - devola.healthPoint > 0.1f * BOSS_HP)
            {
                popola.BurstLevel = 1;
            }
            else
            {
                popola.BurstLevel = 0;
            }

            if (devola.healthPoint == 0 || popola.healthPoint == 0)
            {
                devola.BurstLevel = 3;
            }
            else if (devola.healthPoint - popola.healthPoint > 0.5f * BOSS_HP)
            {
                devola.BurstLevel = 3;
            }
            else if (devola.healthPoint - popola.healthPoint > 0.2f * BOSS_HP)
            {
                devola.BurstLevel = 2;
            }
            else if (devola.healthPoint - popola.healthPoint > 0.1f * BOSS_HP)
            {
                devola.BurstLevel = 1;
            }
            else
            {
                devola.BurstLevel = 0;
            }

            yield return new WaitForSeconds(0.1f);
        }

        popola.Damage(999);
        devola.Damage(999);

        if (IsSceneOver) yield break;

        EnemyManager.instance.DestroyAll();
        EnemyBulletManager.instance.DestroyAll();

        SlowDown(0.2f, 5f);
        SoundManager.instance.StopBgm(4f);

        IsSceneOver = true;
        Curtain.instance.ChangeColor(5f, Curtain.white_clear, Curtain.white, CallNextScene);
    }

    //Functions for view / camera

    private void SetTrack(bool value)
    {
        TrackedCamera trackedCamera = Camera.main.GetComponent<TrackedCamera>();
        trackedCamera.xTrack = value;
        trackedCamera.yTrack = value;
        trackedCamera.zTrack = value;
    }

    IEnumerator MoveCameraBy(Vector3 offset, float duration)
    {
        Vector3 origin = Camera.main.transform.position;
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            Vector3 current = origin + offset * rate;
            Camera.main.transform.position = current;
            yield return null;
            time += Time.deltaTime;
        }
        Camera.main.transform.position = origin + offset;
    }

    IEnumerator MoveCameraTo(Vector3 target, float duration)
    {
        Vector3 origin = Camera.main.transform.position;
        Vector3 offset = target - origin;
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            Vector3 current = origin + offset * rate;
            Camera.main.transform.position = current;
            yield return null;
            time += Time.deltaTime;
        }
        Camera.main.transform.position = target;
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
            Rect rect = new Rect(Screen.width - 160f, Screen.height - 60f, 40f, 20f);

            GUI.Label(rect, "Player Invincible", style);
            rect.x += 120f;
            if (GUI.Button(rect, Player.IsSuper ? "On" : "Off"))
            {
                Player.IsSuper = !Player.IsSuper;
            }
        }
    }
}
