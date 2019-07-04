using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu
{
    public static PauseMenu instance = null;
    public static bool isPaused;

    [SerializeField] private StaticImage top;
    [SerializeField] private StaticImage bottom;
    [SerializeField] private StaticImage panel;
    [SerializeField] private StaticImage background;
    [SerializeField] private StaticControl backgroundMark;
    [SerializeField] private StaticText controlInfo;

    [SerializeField] private Menu optionsMenu;
    [SerializeField] private Menu modeOptions;
    [SerializeField] private Menu soundOptions;
    [SerializeField] private OptionsBox optionsBox;

    private Camera renderCamera;

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

    private void Start()
    {
        SetActive(false);
        isPaused = false;

        optionsMenu.SetActive(false);
        modeOptions.SetActive(false);
        soundOptions.SetActive(false);
        optionsBox.SetActive(false);
    }

    protected override void Update()
    {
        if (!IsCurrentActive()) return;

        base.Update();

        if (STBInput.GetButtonDown("Cancel"))
        {
            SoundManager.instance.PlayUiEfx(UiEfx.CANCEL);
            ContinueGame();
        }
    }

    public void SetCamera(Camera renderCamera)
    {
        this.renderCamera = renderCamera;
        GetComponent<Canvas>().worldCamera = renderCamera;
        GetComponent<Canvas>().planeDistance = 1f;
    }

    //Functions for pause menu
    
    public void CallPauseMenu()
    {
        if (isPaused || Curtain.instance.IsChanging) return;
        
        isPaused = true;
        Time.timeScale = 0f;
        SoundManager.instance.PauseEfx();
        SoundManager.instance.PlayUiEfx(UiEfx.DECIDE_3);
        SoundManager.instance.SetReverbBgm(true);
        MenuController.currentMenu = instance;
        OptionsBox.current = optionsBox;
        FadeIn(0.6f);
    }

    public void ContinueGame()
    {
        FadeOut(0.2f);
        SoundManager.instance.SetReverbBgm(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnContinueButtonDown()
    {
        ContinueGame();
    }

    public void OnOptionsButtonDown()
    {
        background.FadeIn(0.2f);
        backgroundMark.FadeIn(0.2f, 0.2f);
        StartCoroutine(ShowOptionsMenu(0.2f));
    }

    public void OnBackButtonDown()
    {
        ContinueGame();
        SoundManager.instance.StopBgm(1.5f);
        SceneController.instance.IsSceneOver = true;
        Curtain.instance.ChangeColor(2f, Curtain.black_clear, Curtain.black, CallMenuScene);
    }

    public void OnExitButtonDown()
    {
        SoundManager.instance.StopBgm(1f);
        Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, ExitGame);
    }

    void CallMenuScene()
    {
        SceneController.instance.ResetProgress();
        GameManager.SetScene(GameScene.Menu);
    }

    //Functions for in game options menu

    IEnumerator ShowOptionsMenu(float wait = 0f)
    {
        float time = 0f;
        while (time < wait)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        MenuController.currentMenu = optionsMenu;
        optionsMenu.FadeIn(0.6f);
    }

    public void BackToPauseMenu()
    {
        backgroundMark.FadeOut(0.1f);
        background.FadeOut(0.2f, 0.1f);
        MenuController.currentMenu = instance;
        LastPressed.SetPressed(false);
    }

    public void OnModeOptionsDown()
    {
        MenuController.currentMenu = modeOptions;
        modeOptions.FadeIn(0.3f);
    }

    public void OnSoundOptionsDown()
    {
        MenuController.currentMenu = soundOptions;
        soundOptions.FadeIn(0.3f);
    }

    public void BackToOptionsMenu()
    {
        MenuController.currentMenu.FadeOut(0.2f);
        optionsMenu.LastPressed.SetPressed(false);
        MenuController.currentMenu = optionsMenu;
    }

    //Fade in & out functions

    protected override void OnFadeIn(float duration)
    {
        Camera.main.SetBlur(1f, duration * 0.2f);

        top.FadeIn(duration * 0.3f);
        bottom.FadeIn(duration * 0.3f);
        panel.FadeIn(duration * 0.3f);

        foreach (TitleText titleText in titleTexts)
        {
            titleText.FadeIn(duration * 0.4f, 0.1f);
        }
        foreach (StaticText staticText in staticTexts)
        {
            staticText.FadeIn(duration * 0.5f, 0.3f);
        }
        foreach (StaticImage staticImage in staticImages)
        {
            staticImage.FadeIn(duration * 0.5f, 0.3f);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].FadeIn(duration * 0.5f, 0.1f * (i + 3));
        }
    }

    protected override void OnFadeOut(float duration)
    {
        Camera.main.SetBlur(0f, duration * 0.5f);

        top.FadeOut(duration);
        bottom.FadeOut(duration);
        panel.FadeOut(duration);

        foreach (TitleText titleText in titleTexts)
        {
            titleText.FadeOut(duration);
        }
        foreach (StaticText staticText in staticTexts)
        {
            staticText.FadeOut(duration);
        }
        foreach (StaticImage staticImage in staticImages)
        {
            staticImage.FadeOut(duration);
        }
        foreach (SubButton button in buttons)
        {
            button.FadeOut(duration);
        }
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        Time.timeScale = 1f;
        isPaused = false;
        SoundManager.instance.UnPauseEfx();
    }
}