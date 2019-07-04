using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex3Scene : SceneController
{
    private const float BOSS_HP = 150f;
    private const float BOSS_HP_FINAL = 40f;

    private const int BOSS_ID = 0;
    private const int BOSS_ID_FINAL = 999;

    private readonly float[] BGM_LOOP_POINT = { 80f, 11.8991f };
    private readonly float[] BGM_LOOP_LENGTH = { 180.403f, 96.0025f };
    private const float SPB = 60f / 127.9f;
    
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private TextAsset protoMessage;
    [SerializeField] private TextAsset battleMessage;
    [SerializeField] private TextAsset finalMessage;

    [SerializeField] private MapEx3 map;
    [SerializeField] private FloatingDots dots;

    [SerializeField] private EmilGhost ghostPrefab;
    [SerializeField] private EnemyBossEx3 emilPrefab;
    [SerializeField] private EnemyFinalEx3 finalPrefab;

    private List<EnemyBossEx3> emils;
    private EnemyFinalEx3 emilFinal;
    private Coroutine mainLoopCoroutine = null;
    private int currentBgm = -1;

    private static int restartCount = 0;
    private static bool isMainLoppEntered = false;
    private static bool isFinalEntered = false;

    override public void ResetProgress()
    {
        base.ResetProgress();
        restartCount = 0;
        isMainLoppEntered = false;
        isFinalEntered = false;
        EnemyProtoEx3.AllReset();
    }
        
    void Start ()
    {
        IsSceneOver = false;
        GameManager.gameScene = GameScene.Ex3;
        HUDManager.instance.SetCamera(Camera.main);
        PauseMenu.instance.SetCamera(GameCamera.UI);

        Player.IsArmed = true;
        Player.IsMovable = true;

        Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear);

        emils = new List<EnemyBossEx3>();

        if (isFinalEntered)
        {
            StartCoroutine(FinalPhase());
        }
        else if (isMainLoppEntered)
        {
            for (int i = 0; i < 9; i++)
            {
                bool newPosition = true;
                Vector3 position = Vector3.zero;
                while (newPosition)
                {
                    newPosition = false;
                    position = new Vector3(Random.Range(-11f, 11f), 0f, Random.Range(-8f, 8f));
                    if ((position - Player.instance.transform.position).magnitude <= MapEx3.GRID_SIZE)
                        newPosition = true;
                    for (int j = 0; j < i; j++)
                    {
                        if ((position - emils[j].transform.position).magnitude <= MapEx3.GRID_SIZE)
                            newPosition = true;
                    }
                }
                EnemyBossEx3 emil = EnemyManager.instance.SetEnemy(emilPrefab, position);
                emils.Add(emil);
                emil.id = i;
                emil.healthPoint = (int)BOSS_HP;
                HUDManager.instance.AddHPBar(BOSS_ID + emil.id, "？？？？", 0f, 0.06f, 0.06f, emil.transform);
                HUDManager.instance.AutoFadeHPBar(BOSS_ID + emil.id, true);
                HUDManager.instance.ShowHPBar(BOSS_ID + emil.id);
            }
            PlatformDown();
            mainLoopCoroutine = StartCoroutine(MainLoop());
            StartCoroutine(CheckBossStates());
        }
        else
        {
            EmilGhost ghost = Instantiate(ghostPrefab, new Vector3(0f, 0f, 3f), Quaternion.identity);
            ghost.rebirthPosition = new Vector3(0f, 0f, 3f);
            ghost.rebirthTime = 3f;
            StartCoroutine(PrePhase());
        }
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
        if (!isMainLoppEntered) EnemyProtoEx3.AllReset();
        HUDManager.instance.ResetAll();
        GameManager.SetScene(GameScene.Ex3);
    }

    public void CallNextScene()
    {
        ResetProgress();
        GameManager.SetExtraClear(3);
        GameManager.SetScene(GameScene.Menu);
    }

    private void ShowSnowflake(float duration)
    {
        Camera.main.SetSnowflake(duration);
        SoundManager.instance.PlayUiEfx(UiEfx.SNOW);
    }

    //For Phases

    private bool isOrbitingPhase = false;
    private bool isLineDashPhase = false;
    private bool isLandMovePhase = false;

    IEnumerator PrePhase()
    {
        for (int i = 0; i < 5; i++)
        {
            while (EnemyProtoEx3.deathCount <= i)
            {
                yield return new WaitForSeconds(0.5f);
            }
            HUDManager.instance.ShowMessage(Message.GetMessage(protoMessage, i));
        }
        yield return new WaitForSeconds(3f);

        ShowSnowflake(0.4f);
        yield return new WaitForSeconds(1f);
        ShowSnowflake(0.4f);
        yield return new WaitForSeconds(1f);

        foreach (EnemyProtoEx3 proto in EnemyProtoEx3.protos)
        {
            EnemyBossEx3 emil = EnemyManager.instance.SetEnemy(emilPrefab, proto.transform.position);
            emils.Add(emil);
            emil.id = proto.id;
            emil.healthPoint = (int)BOSS_HP;
            HUDManager.instance.AddHPBar(BOSS_ID + emil.id, "？？？？", 0f, 0.06f, 0.06f, emil.transform);
            HUDManager.instance.ShowHPBar(BOSS_ID + emil.id);
            HUDManager.instance.AutoFadeHPBar(BOSS_ID + emil.id, true);
            proto.Burst();
        }
        SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, Vector3.zero);
        ShowSnowflake(1.5f);
        yield return new WaitForSeconds(1.5f);
        PlatformDown();
        mainLoopCoroutine = StartCoroutine(MainLoop());
        StartCoroutine(CheckBossStates());
    }

    IEnumerator MainLoop()
    {
        if (!SoundManager.instance.IsPlaying)
        {
            currentBgm = 0;
            SoundManager.instance.PlayBgm(bgms[currentBgm], 2f);
            SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT[currentBgm], BGM_LOOP_LENGTH[currentBgm]);
        }
        isMainLoppEntered = true;
        yield return new WaitForSeconds(3.5f);
        
        //Outer Ring

        StartCoroutine(OrbitPhase(2, 2));
        while (isOrbitingPhase) yield return null;

        StartCoroutine(GridLineDash(2));
        while (isLineDashPhase) yield return null;

        StartCoroutine(OrbitPhase(1, 2, false));
        while (isOrbitingPhase) yield return null;

        map.MoveIn(2);
        yield return new WaitForSeconds(5f);

        StartCoroutine(LandMove(12f, 3));
        while (isLandMovePhase) yield return null;

        //Middle Ring

        map.SetRingUnstable(2);

        StartCoroutine(OrbitPhase(1, 3));
        while (isOrbitingPhase) yield return null;

        StartCoroutine(GridLineDash(1));
        while (isLineDashPhase) yield return null;

        StartCoroutine(OrbitPhase(0, 2, false));
        while (isOrbitingPhase) yield return null;

        map.MoveIn(1);
        yield return new WaitForSeconds(1f);
        map.MoveIn(2);
        yield return new WaitForSeconds(5f);

        StartCoroutine(LandMove(12f, 3));
        while (isLandMovePhase) yield return null;

        //Inner Ring

        map.SetRingUnstable(1);
        map.SetRingUnstable(2);

        StartCoroutine(OrbitPhase(0, 4));
        while (isOrbitingPhase) yield return null;

        StartCoroutine(GridLineDash(0));
        while (isLineDashPhase) yield return null;

        StartCoroutine(OrbitPhase(0, 3, false));
        while (isOrbitingPhase) yield return null;
        
        map.MoveIn(0);
        yield return new WaitForSeconds(1f);
        map.MoveIn(1);
        yield return new WaitForSeconds(1f);
        map.MoveIn(2);
        yield return new WaitForSeconds(5f);

        StartCoroutine(LandMove(12f, 5));
        while (isLandMovePhase) yield return null;

        mainLoopCoroutine = StartCoroutine(MainLoop());
    }

    IEnumerator OrbitPhase(int ringIndex, int roundCount, bool moveOut = true)
    {
        isOrbitingPhase = true;

        int colOffset = ringIndex + 2;
        int rowOffset = ringIndex + 1;
        foreach (EnemyBossEx3 emil in emils)
            emil.StartOrbit(colOffset, rowOffset, roundCount, moveOut);

        while (!emils[0].IsOrbitOver)
        {
            if (!isOrbitingPhase) yield break;
            yield return null;
        }

        isOrbitingPhase = false;
        foreach (EnemyBossEx3 emil in emils)
            emil.StopOrbit();
    }

    IEnumerator GridLineDash(int ringIndex)
    {
        isLineDashPhase = true;

        int offset = ringIndex + 1;
        List<int> rows = GetRandomList(offset, true);
        List<int> cols = GetRandomList(offset);
        
        for (int i = 0; i < offset * 2 + 1; i++)
        {
            if (!isLineDashPhase) yield break;
            int row = rows[i];
            map.SetRowUnstable(ringIndex, row);
            EffectManager.instance.SetWarningRegion(new Vector3(-36f, 0.5f, row * MapEx3.GRID_SIZE), new Vector3(36f, 0.5f, row * MapEx3.GRID_SIZE), 2f);
            foreach (EnemyBossEx3 emil in emils)
                emil.StartGridRowDash(row, i % 2 == 0, 0.5f, 3.5f);
            yield return new WaitForSeconds(4f);

            if (!isLineDashPhase) yield break;
            int col = cols[i];
            map.SetColUnstable(ringIndex, col);
            EffectManager.instance.SetWarningRegion(new Vector3(col * MapEx3.GRID_SIZE, 0.5f, -30f), new Vector3(col * MapEx3.GRID_SIZE, 0.5f, 30f), 2f);
            foreach (EnemyBossEx3 emil in emils)
                emil.StartGridColDash(col, i % 2 == 0, 0.5f, 3.5f);
            yield return new WaitForSeconds(4f);            
        }

        isLineDashPhase = false;
    }

    IEnumerator LandMove(float period, int waveCount)
    {
        isLandMovePhase = true;

        foreach (EnemyBossEx3 emil in emils)
            emil.StartRandomMove(5f);
        yield return new WaitForSeconds(6f);

        int count = 0;
        while (count < waveCount)
        {
            if (!isLandMovePhase) yield break;

            int emilCount = emils.Count;
            List<int> list = new List<int>();
            bool[] attack = new bool[emilCount];
            for (int i = 0; i < emilCount; i++)
            {
                if (emils[i].IsInBattle) list.Add(i);
                attack[i] = false;
            }

            float timeWeapon = 0.75f * period;
            if (list.Count > 0)
            {
                int id = list[Random.Range(0, list.Count)];
                list.Remove(id);
                emils[id].StartRingWeapon(timeWeapon);
                attack[id] = true;
            }
            if (list.Count > 0)
            {
                int id = list[Random.Range(0, list.Count)];
                list.Remove(id);
                emils[id].StartForkedWeapon(timeWeapon);
                attack[id] = true;
            }
            if (list.Count > 0)
            {
                int id = list[Random.Range(0, list.Count)];
                list.Remove(id);
                emils[id].StartConvergedWeapon(timeWeapon);
                attack[id] = true;
            }
            for (int i = 0; i < emilCount; i++)
            {
                if (!attack[i]) emils[i].StartRandomMove(period, 2);
            }
            yield return new WaitForSeconds(period);
            count++;
        }

        isLandMovePhase = false;
    }
    
    List<int> GetRandomList(int offset, bool fixedBound = false)
    {
        List<int> list = new List<int>();
        List<int> sorted = new List<int>();
        for (int i = -offset; i <= offset; i++) sorted.Add(i);

        if (fixedBound)
        {
            list.Add(offset);
            list.Add(-offset);
            sorted.Remove(offset);
            sorted.Remove(-offset);
        }

        while (sorted.Count > 0)
        {
            int index = Random.Range(0, sorted.Count);
            list.Add(sorted[index]);
            sorted.RemoveAt(index);
        }

        return list;
    }

    //For checking states and HUD updates

    IEnumerator CheckBossStates()
    {
        int msgCount = 0;
        int beatenCount = 0;

        List<EnemyBossEx3> inBattleEmils = new List<EnemyBossEx3>(emils);
        
        while (msgCount < 9)
        {
            List<EnemyBossEx3> temp = new List<EnemyBossEx3>(inBattleEmils);
            foreach (EnemyBossEx3 emil in temp)
            {
                HUDManager.instance.SetHP(BOSS_ID + emil.id, emil.healthPoint / BOSS_HP);
                if (emil.healthPoint <= 0)
                {
                    HUDManager.instance.HideHPBar(BOSS_ID + emil.id);
                    inBattleEmils.Remove(emil);
                    beatenCount++;
                }
            }
            
            if (beatenCount > msgCount)
            {
                HUDManager.instance.ShowMessage(Message.GetMessage(battleMessage, msgCount));
                msgCount++;
            }
            yield return null;
        }

        StopMainLoop();
        foreach (EnemyBossEx3 emil in emils) emil.Stop();
        yield return new WaitForSeconds(3f);
        StartCoroutine(FinalPhase());
    }

    private void StopMainLoop()
    {
        StopCoroutine(mainLoopCoroutine);
        isOrbitingPhase = false;
        isLineDashPhase = false;
        isLandMovePhase = false;
    }
    
    //Final

    IEnumerator FinalPhase()
    {
        if (isFinalEntered)
        {
            if (!SoundManager.instance.IsPlaying)
            {
                currentBgm = 1;
                SoundManager.instance.PlayBgm(bgms[currentBgm], 2f);
                SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT[currentBgm], BGM_LOOP_LENGTH[currentBgm]);
            }
        }
        else
        {
            isFinalEntered = true;
            if (currentBgm != 1)
            {
                currentBgm = 1;
                SoundManager.instance.PlayBgm(bgms[currentBgm], 6f);
                SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT[currentBgm], BGM_LOOP_LENGTH[currentBgm]);
            }
            yield return new WaitForSeconds(5f);

            map.MoveIn(0);
            map.MoveIn(1);
            map.MoveIn(2);
            yield return new WaitForSeconds(4f);

            PlatformStop();
            foreach (EnemyBossEx3 emil in emils) emil.StartLand(24f, 2);
            yield return new WaitForSeconds(4f);

            HUDManager.instance.ShowMessages(Message.GetMessages(finalMessage));
            yield return new WaitForSeconds(20f);

            foreach (EnemyBossEx3 emil in emils)
            {
                emil.Burst();
            }
            SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, Vector3.zero);
        }

        Vector3 position = new Vector3(0f, 0f, 3f);
        emilFinal = EnemyManager.instance.SetEnemy(finalPrefab, position);
        emilFinal.healthPoint = (int)BOSS_HP_FINAL;
        HUDManager.instance.AddHPBar(BOSS_ID_FINAL, "？？？？", 0.5f, 0.1f, 0.6f);
        HUDManager.instance.ShowHPBar(BOSS_ID_FINAL);
        yield return new WaitForSeconds(3f);

        map.SetAllUnstable();
        emilFinal.StartCountDown();

        while (emilFinal.IsInBattle)
        {
            HUDManager.instance.SetHP(999, emilFinal.healthPoint / BOSS_HP_FINAL);
            yield return null;
        }
        HUDManager.instance.SetHP(BOSS_ID_FINAL, 0f);
        map.SetAllStable();

        yield return new WaitForSeconds(5f);
        HUDManager.instance.HideHPBar(BOSS_ID_FINAL);
        
        PlatformDown(160f, 6f);
        map.MoveInHouse(10f);
        yield return new WaitForSeconds(10f);
        map.DestroyFloors();
        PlatformStop(0f);
    }

    public void OnFinalChecked()
    {
        if (IsSceneOver) return;

        SlowDown(0.2f, 5f);
        SoundManager.instance.StopBgm(4f);
        SoundManager.instance.PlayUiEfx(UiEfx.SCENE_CHANGE);

        IsSceneOver = true;
        Curtain.instance.ChangeColor(5f, Curtain.white_clear, Curtain.white, CallNextScene);
    }

    //Functions for view / camera

    private void PlatformDown(float maxSpeed = 60f, float duration = 10f)
    {
        map.IsMoving = true;
        StartCoroutine(ChangeSpeed(0f, maxSpeed, duration));
    }

    private void PlatformStop(float maxSpeed = 60f, float duration = 10f)
    {
        map.IsMoving = false;
        StartCoroutine(ChangeSpeed(maxSpeed, 0f, duration));
    }

    IEnumerator ChangeSpeed(float beg, float end, float duration)
    {
        SetGlobalSpeed(beg);
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float speed = Mathf.Lerp(beg, end, rate * rate);
            SetGlobalSpeed(speed);
            yield return null;
            time += Time.deltaTime;
        }
        SetGlobalSpeed(end);
    }

    private void SetGlobalSpeed(float value)
    {
        dots.globalVelocity = new Vector3(0f, value, 0f);
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
