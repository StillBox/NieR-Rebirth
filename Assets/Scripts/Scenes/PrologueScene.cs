using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueScene : SceneController
{
    private const float BGM_LOOP_POINT = 23.2101f;
    private const float BGM_LOOP_LENGTH = 120f;

    [SerializeField] private AudioClip bgm;
    [SerializeField] private TextAsset startMessage;
    [SerializeField] private TextAsset viewMessage;
    [SerializeField] private TextAsset soundMessage;
    [SerializeField] private TextAsset fireMessage;
    [SerializeField] private TextAsset finalMessage;

    [SerializeField] private Light playerLight;
    [SerializeField] private InteractivePoint viewCheck0;
    [SerializeField] private InteractivePoint viewCheck1;
    [SerializeField] private InteractivePoint soundCheck0;
    [SerializeField] private InteractivePoint soundCheck1;
    [SerializeField] private InteractivePoint weaponCheck;
    [SerializeField] private InteractivePoint finalCheck;

    private const float LIGHT_INTENSITY = 3f;
    private const float LIGHT_ANGLE = 30f;
    private int viewCheckedCount = 0;
    private int soundCheckedCount = 0;

    void Start()
    {
        GameManager.gameScene = GameScene.Prologue;
        HUDManager.instance.SetCamera(Camera.main);
        PauseMenu.instance.SetCamera(GameCamera.UI);

        Player.IsMovable = false;
        Player.IsArmed = false;
        playerLight.spotAngle = LIGHT_ANGLE;
        playerLight.intensity = 0f;
        AudioListener.volume = 0f;

        viewCheck0.SetActive(false);
        viewCheck1.SetActive(false);
        soundCheck0.SetActive(false);
        soundCheck1.SetActive(false);
        weaponCheck.SetActive(false);
        finalCheck.SetActive(false);

        Curtain.instance.ChangeColor(2f, Curtain.black, Curtain.black_clear);
        StartCoroutine(IntroPhase());
    }

    void Update()
    {
        if (STBInput.GetButtonDown("Pause"))
        {
            PauseMenu.instance.CallPauseMenu();
        }
    }

    public void OnViewChecked()
    {
        viewCheckedCount++;
        StartCoroutine(ExtendLightAngle(LIGHT_ANGLE * viewCheckedCount, LIGHT_ANGLE * (viewCheckedCount + 1), 0.5f));
        StartCoroutine(ExtendLightIntensity(LIGHT_INTENSITY * viewCheckedCount, LIGHT_INTENSITY * (viewCheckedCount + 1), 0.5f));
        if (viewCheckedCount == 2)
            StartCoroutine(SoundPhase());
    }

    public void OnSoundChecked()
    {
        soundCheckedCount++;
        StartCoroutine(IncreaseVolume(0.5f * (soundCheckedCount - 1), 0.5f * soundCheckedCount, 0.5f));
        if (soundCheckedCount == 1)
        {
            SoundManager.instance.PlayBgm(bgm);
            SoundManager.instance.SetBgmLoop(BGM_LOOP_POINT, BGM_LOOP_LENGTH);
        }
        else if (soundCheckedCount == 2)
            StartCoroutine(WeaponPhase());
    }

    public void OnWeaponChecked()
    {
        StartCoroutine(FinalPhase());
    }

    public void OnFinalChecked()
    {
        if (IsSceneOver) return;
        IsSceneOver = true;
        HUDManager.instance.ResetAll();
        SoundManager.instance.PlayUiEfx(UiEfx.SCENE_CHANGE);
        Curtain.instance.ChangeColor(2f, Curtain.white_clear, Curtain.white, BackToDark);
    }

    private void BackToDark()
    {
        Curtain.instance.ChangeColor(1f, Curtain.white, Curtain.black, LoadMenuScene);
    }

    private void LoadMenuScene()
    {
        GameManager.SetScene(GameScene.Menu);
        GameManager.SetUnlockedProgress(GameProgress.StoryIntro);
    }

    private void ShowSnowflake()
    {
        Camera.main.SetSnowflake(0.4f);
        SoundManager.instance.PlayUiEfx(UiEfx.SNOW);
    }

    IEnumerator ExtendLightAngle(float beg, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float angle = Mathf.Lerp(beg, end, time / duration);
            playerLight.spotAngle = angle;
            yield return null;
            time += Time.deltaTime;
        }
        playerLight.spotAngle = end;
    }

    IEnumerator ExtendLightIntensity(float beg, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float intensity = Mathf.Lerp(beg, end, time / duration);
            playerLight.intensity = intensity;
            yield return null;
            time += Time.deltaTime;
        }
        playerLight.intensity = end;
    }

    IEnumerator IncreaseVolume(float beg, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float volume = Mathf.Lerp(beg, end, time / duration);
            AudioListener.volume = volume;
            yield return null;
            time += Time.deltaTime;
        }
        AudioListener.volume = end;
    }

    IEnumerator CallingFrom2B()
    {
        SoundManager.instance.PlayUiEfx(UiEfx.CALL_9S_1);
        Camera.main.SetSnowflake(0.5f);
        yield return new WaitForSeconds(3f);

        SoundManager.instance.PlayUiEfx(UiEfx.CALL_9S_2);
        Camera.main.SetSnowflake(1f);
    }

    //Coroutines for scene phases

    IEnumerator IntroPhase()
    {
        yield return new WaitForSeconds(0.5f);

        AudioListener.volume = GameManager.mainVolume;
        ShowSnowflake();
        yield return new WaitForSeconds(0.5f);

        AudioListener.volume = 0f;
        Message[] messages = Message.GetMessages(startMessage);
        HUDManager.instance.ShowMessages(messages);
        yield return new WaitForSeconds(1.5f);

        AudioListener.volume = GameManager.mainVolume;
        ShowSnowflake();
        yield return new WaitForSeconds(3.5f);

        AudioListener.volume = 0f;
        StartCoroutine(ViewPhase());
    }

    IEnumerator ViewPhase()
    {
        StartCoroutine(ExtendLightIntensity(0f, 3f, 2f));
        Player.IsMovable = true;

        Message[] messages = Message.GetMessages(viewMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        viewCheck0.SetActive(true);
        viewCheck1.SetActive(true);
    }

    IEnumerator SoundPhase()
    {
        Message[] messages = Message.GetMessages(soundMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        soundCheck0.SetActive(true);
        soundCheck1.SetActive(true);
    }

    IEnumerator WeaponPhase()
    {
        Message[] messages = Message.GetMessages(fireMessage);
        HUDManager.instance.ShowMessages(messages);
        yield return new WaitForSeconds(7f);

        StartCoroutine(CallingFrom2B());
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        weaponCheck.SetActive(true);
    }

    IEnumerator FinalPhase()
    {
        Player.IsArmed = true;

        Message[] messages = Message.GetMessages(finalMessage);
        HUDManager.instance.ShowMessages(messages);
        while (HUDManager.instance.isShowingMessage) yield return null;
        yield return new WaitForSeconds(0.5f);

        finalCheck.SetActive(true);
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
                GameManager.SetScene(GameScene.Prologue);
            }

            rect.x += 80f;
            if (GUI.Button(rect, "Skip"))
            {
                AudioListener.volume = GameManager.mainVolume;
                OnFinalChecked();
            }
        }
    }
}