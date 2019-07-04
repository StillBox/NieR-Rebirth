using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex1Scene : SceneController
{
    private const float BOSS_HP = 180f;
    private const int BOSS_ID_KO_SHI = 0;
    private const int BOSS_ID_RO_SHI = 1;
    
    private const float SPEED_MIN = -12f;
    private const float SPEED_MAX = -30f;

    private const float BGM_LOOP_POINT = 137.5f;
    private const float BGM_LOOP_LENGTH = 145.822f;
    
    [SerializeField] private AudioClip bgm;

    [SerializeField] private MapEx1 map;
    [SerializeField] private FloatingDots dots;
    [SerializeField] private EnemyJunior baseEnmeyPrefab;
    [SerializeField] private EnemyBombEx1 bombEnemyPrefab;
    [SerializeField] private EnemyKoShi ko_Shi;
    [SerializeField] private EnemyRoShi ro_Shi;

    private static int restartCount = 0;
    private TrackedCamera trackedCamera = null;
    private float viewAngle = 0f;
    private int bossCount = 2;

    override public void ResetProgress()
    {
        base.ResetProgress();
        restartCount = 0;
    }

    void Start()
    {
        IsSceneOver = false;
        GameManager.gameScene = GameScene.Ex1;
        HUDManager.instance.SetCamera(Camera.main);
        PauseMenu.instance.SetCamera(GameCamera.UI);
        trackedCamera = Camera.main.GetComponent<TrackedCamera>();
        viewAngle = 0f;
        SetCameraAngle(viewAngle);

        Player.IsMovable = true;
        Player.IsArmed = true;

        Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear);

        if (!SoundManager.instance.IsPlaying)
        {
            SoundManager.instance.PlayBgm(bgm, 2f);
            SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT, BGM_LOOP_LENGTH);
        }

        TurnForward(1.5f);
        Player.instance.transform.position = new Vector3(0f, 0f, -10f);
        Player.instance.SmoothMoveTo(Vector3.zero, 1.5f);
        SetGlobalSpeed(SPEED_MIN);

        //Init of enemies

        HUDManager.instance.AddHPBar(BOSS_ID_KO_SHI, "KO-SHI", 0.21f, -0.06f, 0.3125f);
        HUDManager.instance.AddHPBar(BOSS_ID_RO_SHI, "RO-SHI", -0.21f, -0.06f, 0.3125f);

        ko_Shi.healthPoint = (int)BOSS_HP;
        ro_Shi.healthPoint = (int)BOSS_HP;
        ko_Shi.SetActive(false);
        ro_Shi.SetActive(false);

        StartCoroutine(NoramlBattle(1));
        StartCoroutine(CheckBossState());
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

    public void OnKoShiDestroyed()
    {
        HUDManager.instance.HideHPBar(BOSS_ID_KO_SHI);
        bossCount--;
    }

    public void OnRoShiDestroyed()
    {
        HUDManager.instance.HideHPBar(BOSS_ID_RO_SHI);
        bossCount--;
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
        GameManager.SetScene(GameScene.Ex1);
    }

    public void CallNextScene()
    {
        ResetProgress();
        GameManager.SetExtraClear(1);
        GameManager.SetScene(GameScene.Menu);
    }

    //=============
    //Normal Battle
    //=============

    IEnumerator NoramlBattle(int phase)
    {
#if SHOW_DEBUG_LOG
        Debug.Log("Start normal battle - phase " + phase);
#endif
        SetBossMode(false);

        if (phase == 1)
        {
            yield return new WaitForSeconds(1.5f);
            while (SoundManager.instance.Current <= 4.95f) yield return null;
        }
        else
        {
            float timeBeg = Time.timeSinceLevelLoad;

            SetBossMode(false);
            GhostManager.instance.MoveIn();
            while (!GhostManager.instance.IsMoveInEmpty)
            {
                if (EnemyManager.instance.IsEmpty)
                {
                    if (!GhostManager.instance.ExistGhostReadyToRebirth)
                        GhostManager.instance.MinimizeWaitTime();
                }
                yield return new WaitForSeconds(0.1f);
            }

            while (!EnemyManager.instance.IsEmpty)
                yield return new WaitForSeconds(0.1f);

            yield return new WaitForSeconds(1.5f);

            float delta = Time.timeSinceLevelLoad - timeBeg;
            if (delta <= 5f)
                yield return new WaitForSeconds(5f - delta);
        }

        if (phase == 5)
        {
            StartCoroutine(FinalBossBattle());
            yield break;
        }
        
        Vector3[] enemyPositions =
            phase == 1 ? new Vector3[4] { new Vector3(-4f, 0f, 8f), new Vector3(4f, 0f, 8f), new Vector3(5f, 0f, -5f), new Vector3(-5f, 0f, -5f) } :
            phase == 2 ? new Vector3[4] { new Vector3(5f, 0f, 1f), new Vector3(0f, 0f, 9f), new Vector3(-5f, 0f, 1f), new Vector3(0f, 0f, -6f) } :
            phase == 3 ? new Vector3[4] { new Vector3(5f, 0f, -5f), new Vector3(-5f, 0f, -5f), new Vector3(-4f, 0f, 8f), new Vector3(4f, 0f, 8f) } :
            new Vector3[4] { new Vector3(-5f, 0f, 1f), new Vector3(0f, 0f, -6f), new Vector3(5f, 0f, 1f), new Vector3(0f, 0f, 9f) };

        for (int i = 0; i < 4; i++)
        {
            EnemyManager.instance.SetEnemy(baseEnmeyPrefab, enemyPositions[i]);
            yield return new WaitForSeconds(0.45f);
        }

        while (!EnemyManager.instance.IsEmpty)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        if (phase % 2 == 1)
            StartCoroutine(BossBattleKoShi(phase));
        else
            StartCoroutine(BossBattleRoShi(phase));        
    }

    //=============
    //KoShi  Battle
    //=============

    IEnumerator BossBattleKoShi(int phase)
    {
#if SHOW_DEBUG_LOG
        Debug.Log("Start KoShi battle - phase " + phase);
#endif
        ko_Shi.Phase = phase;
        ko_Shi.MoveIn();
        yield return new WaitForSeconds(2f);
        TurnLeft();
        SetBossMode(true);
        yield return new WaitForSeconds(1f);
        HUDManager.instance.ShowHPBar(BOSS_ID_KO_SHI);

        if (phase == 1)
        {
            while (ko_Shi.healthPoint > BOSS_HP * 2f /3f)
            {
                yield return null;
            }
            SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
            EffectManager.instance.Explode(ko_Shi.transform.position, ExplosionType.LARGE);
            ko_Shi.MoveForward();            
        }
        else if (phase == 3)
        {
            while (ko_Shi.healthPoint > BOSS_HP / 3f)
            {
                yield return null;
            }
            SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
            EffectManager.instance.Explode(ko_Shi.transform.position, ExplosionType.LARGE);
            ko_Shi.MoveBackward();
        }

        yield return new WaitForSeconds(2f);
        HUDManager.instance.HideHPBar(BOSS_ID_KO_SHI);

        while (!EnemyManager.instance.IsEmpty ||
            !GhostManager.instance.IsFixedEmpty ||
            ko_Shi.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(3f);
        StartCoroutine(TraceBattle(phase));
    }

    //=============
    //RoShi  Battle
    //=============

    IEnumerator BossBattleRoShi(int phase)
    {
#if SHOW_DEBUG_LOG
        Debug.Log("Start RoShi battle - phase " + phase);
#endif
        SetBossMode(true);
        ro_Shi.Phase = phase;
        ro_Shi.MoveIn();
        yield return new WaitForSeconds(2f);
        TurnRight();
        yield return new WaitForSeconds(1f);
        HUDManager.instance.ShowHPBar(BOSS_ID_RO_SHI);

        if (phase == 2)
        {
            while (ro_Shi.healthPoint > BOSS_HP * 2f / 3f)
            {
                yield return null;
            }
            SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
            EffectManager.instance.Explode(ro_Shi.transform.position, ExplosionType.LARGE);
            ro_Shi.MoveBackward();
        }
        else if (phase == 4)
        {
            while (ro_Shi.healthPoint > BOSS_HP / 3f)
            {
                yield return null;
            }
            SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
            EffectManager.instance.Explode(ro_Shi.transform.position, ExplosionType.LARGE);
            ro_Shi.MoveForward();
        }

        yield return new WaitForSeconds(2f);
        HUDManager.instance.HideHPBar(BOSS_ID_RO_SHI);

        while (!EnemyManager.instance.IsEmpty ||
            !GhostManager.instance.IsFixedEmpty ||
            ro_Shi.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(3f);
        StartCoroutine(TraceBattle(phase));
    }

    //=============
    //Trace  Battle
    //=============

    IEnumerator TraceBattle(int phase)
    {
#if SHOW_DEBUG_LOG
        Debug.Log("Start trace battle - phase " + phase);
#endif

        TurnForward();
        SetBossMode(false);
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(ChangeSpeed(SPEED_MIN, SPEED_MAX, 4f));
        yield return new WaitForSeconds(4f);

        int count = 8 + phase * 2;
        float gap = 0.9f - 0.05f * phase;
        while (count > 0)
        {
            float x = Mathf.Clamp(Random.Range(-4f, 4f), -3f, 3f);
            EnemyManager.instance.SetEnemy(bombEnemyPrefab, new Vector3(x, 0f, 28f));
            count--;
            yield return new WaitForSeconds(gap);
        }
        while (!EnemyManager.instance.IsEmpty)
        {
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(ChangeSpeed(SPEED_MAX, SPEED_MIN, 4f));
        yield return new WaitForSeconds(3f);
        
        StartCoroutine(NoramlBattle(phase + 1));
    }

    //=============
    //Final  Battle
    //=============

    IEnumerator FinalBossBattle()
    {
#if SHOW_DEBUG_LOG
        Debug.Log("Start final battle");
#endif
        SetBossMode(true);
        
        Vector3[] enemyPositions = new Vector3[4] { new Vector3(0f, 0f, -6f), new Vector3(-5f, 0f, 1f), new Vector3(0f, 0f, 9f), new Vector3(5f, 0f, 1f) };
        for (int i = 0; i < 4; i++)
        {
            EnemyJunior enemy = EnemyManager.instance.SetEnemy(baseEnmeyPrefab, enemyPositions[i]);
            enemy.IsBossMode = true;
            yield return new WaitForSeconds(0.45f);
        }

        while (!EnemyManager.instance.IsEmpty ||
            !GhostManager.instance.IsFixedEmpty)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);

        enemyPositions = new Vector3[4] { new Vector3(4f, 0f, 8f), new Vector3(-4f, 0f, 8f), new Vector3(-5f, 0f, -5f), new Vector3(5f, 0f, -5f) };
        for (int i = 0; i < 4; i++)
        {
            EnemyJunior enemy = EnemyManager.instance.SetEnemy(baseEnmeyPrefab, enemyPositions[i]);
            enemy.IsBossMode = true;
            yield return new WaitForSeconds(0.45f);
        }

        while (!EnemyManager.instance.IsEmpty ||
            !GhostManager.instance.IsFixedEmpty)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        
        ko_Shi.Phase = 5;
        ro_Shi.Phase = 5;
        ko_Shi.MoveIn();
        ro_Shi.MoveIn();
        yield return new WaitForSeconds(3f);

        HUDManager.instance.ShowHPBar(BOSS_ID_KO_SHI);
        HUDManager.instance.ShowHPBar(BOSS_ID_RO_SHI);
    }

    IEnumerator CheckBossState()
    {
        while (bossCount > 0)
        {
            if (ko_Shi.IsInBattle)
                HUDManager.instance.SetHP(BOSS_ID_KO_SHI, ko_Shi.healthPoint / BOSS_HP);
            if (ro_Shi.IsInBattle)
                HUDManager.instance.SetHP(BOSS_ID_RO_SHI, ro_Shi.healthPoint / BOSS_HP);
            yield return null;
        }

        if (IsSceneOver) yield break;

        GhostManager.instance.DestroyAll();
        EnemyManager.instance.DestroyAll();
        EnemyBulletManager.instance.DestroyAll();

        SlowDown(0.2f, 5f);
        SoundManager.instance.StopBgm(4f);

        IsSceneOver = true;
        Curtain.instance.ChangeColor(5f, Curtain.white_clear, Curtain.white, CallNextScene);
    }

    //
    //Fuctions for mode change
    //

    private void SetBossMode(bool value)
    {
        GhostManager.IsBossMode = value;
    }

    //
    //Fuctions for camera view etc.
    //
    
    private void TurnLeft(float duration = 1.5f)
    {
        StartCoroutine(RotateViewBy(90f, duration, false));
        map.SetWalls(1);
        GhostManager.Region = map.regionLeft;
    }

    private void TurnRight(float duration = 1.5f)
    {
        StartCoroutine(RotateViewBy(-90f, duration, false));
        map.SetWalls(2);
        GhostManager.Region = map.regionRight;
    }

    private void TurnForward(float duration = 1.5f)
    {
        StartCoroutine(RotateViewTo(viewAngle, 0f, duration, true));
        map.SetWalls(0);
        GhostManager.Region = map.regionForward;
        float playerZ = Player.instance.PositionZ;
        if (playerZ < -6.5f) Player.instance.MoveBy(new Vector3(0f, 0f, -6.5f - playerZ), duration);
        if (playerZ > 9.5f) Player.instance.MoveBy(new Vector3(0f, 0f, 9.5f - playerZ), duration);
    }

    IEnumerator RotateViewTo(float beg, float end, float duration, bool track, float wait = 0f)
    {
        Player.IsMovable = false;

        viewAngle = beg;
        SetCameraAngle(viewAngle);
        yield return new WaitForSeconds(wait);

        float time = 0f;
        while (time < duration)
        {
            float rate = (1f - Mathf.Cos(Mathf.PI * time / duration)) / 2f;
            float angle = Mathf.Lerp(beg, end, rate);
            SetCameraAngle(angle);
            yield return null;
            time += Time.deltaTime;
        }

        viewAngle = end;
        SetCameraAngle(viewAngle);
        SetTrack(track);

        Player.IsMovable = true;
    }

    IEnumerator RotateViewBy(float delta, float duration, bool track, float wait = 0f)
    {
        Player.IsMovable = false;

        float time = 0f;
        while (trackedCamera.RemainDspl.magnitude > Mathf.Epsilon)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time < wait)
        {
            yield return null;
            time += Time.deltaTime;
        }
        time = 0f;
        while (time < duration)
        {
            float rate = (1f - Mathf.Cos(Mathf.PI * time / duration)) / 2f;
            float angle = viewAngle + delta * rate;
            SetCameraAngle(angle);
            yield return null;
            time += Time.deltaTime;
        }

        viewAngle += delta;
        SetCameraAngle(viewAngle);
        SetTrack(track);

        Player.IsMovable = true;
    }
    
    private void SetCameraAngle(float value)
    {
        Vector3 playerOffset = Player.instance.transform.position;
        float rad = Mathf.PI * value / 180f;
        trackedCamera.Offset = new Vector3(12f * Mathf.Sin(rad), 18f, -10f * Mathf.Cos(rad));
        trackedCamera.AnchorOffset = new Vector3(-2f * Mathf.Sin(rad) - playerOffset.x * Mathf.Abs(Mathf.Sin(rad)), 0f, -playerOffset.z);
    }

    private void SetTrack(bool value)
    {
        trackedCamera.xTrack = value;
    }

    IEnumerator ChangeSpeed(float beg, float end, float duration)
    {
        SetGlobalSpeed(beg);
        float time = 0f;
        while (time < duration)
        {
            float rate = 0.5f - 0.5f * Mathf.Cos(Mathf.PI * time / duration);
            float speed = Mathf.Lerp(beg, end, rate);
            SetGlobalSpeed(speed);
            yield return null;
            time += Time.deltaTime;
        }
        SetGlobalSpeed(end);
    }

    private void SetGlobalSpeed(float value)
    {
        map.globalVelocity = value;
        dots.globalVelocity = new Vector3(0f, 0f, value * 2.5f);
        Player.instance.SetFlameSpeed(new Vector3(0f, 0f, -value / 2f));
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