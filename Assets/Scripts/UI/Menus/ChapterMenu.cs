using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterMenu : Menu
{
    [SerializeField] private SubButton[] chapterButtons;
    [SerializeField] private SubButton[] extraButtons;
    [SerializeField] private SubButton[] storyButtons;
    [SerializeField] private InfoBox infoBox;

    private int chapterCount = 5;
    private int extraCount = 4;
    private int storyCount = 6;

    protected override void Update()
    {
        if (!IsCurrentActive()) return;

        base.Update();

        if (STBInput.GetButtonDown("Cancel"))
        {
            SoundManager.instance.PlayUiEfx(UiEfx.CANCEL);
            MenuController.instance.BackToMainMenu(true);
        }
    }

    protected override void OnFadeIn(float duration)
    {
        chapterCount = GameManager.IsUnlocked(GameProgress.HackCh4) ? 5 :
            GameManager.IsUnlocked(GameProgress.HackCh3) ? 4 :
            GameManager.IsUnlocked(GameProgress.HackCh2) ? 3 :
            GameManager.IsUnlocked(GameProgress.HackCh1) ? 2 : 1;

        extraCount = GameManager.IsUnlocked(GameProgress.MainClear) ? 4 : 1;

        storyCount = GameManager.IsUnlocked(GameProgress.StoryCoda) ? 5 :
            GameManager.IsUnlocked(GameProgress.StoryCh3) ? 4 :
            GameManager.IsUnlocked(GameProgress.StoryCh2) ? 3 :
            GameManager.IsUnlocked(GameProgress.StroyCh1) ? 2 : 1;

        infoBox.FadeIn(0.6f * duration);
        foreach (StaticImage staticImage in staticImages)
        {
            staticImage.FadeIn(duration * 0.5f);
        }
        foreach (TitleText titleText in titleTexts)
        {
            titleText.FadeIn(duration * 0.4f);
        }

        foreach (SubButton button in chapterButtons)
        {
            button.SetActive(false);
        }

        for (int i = 0; i < chapterCount; i++)
        {
            chapterButtons[i].SetActive(true);
            chapterButtons[i].FadeIn(duration * 0.4f, duration * 0.1f * i);
            chapterButtons[i].navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = null,
                selectOnRight = extraButtons[0],
                selectOnUp = i == 0 ? null : chapterButtons[i - 1],
                selectOnDown = i == chapterCount - 1 ? null : chapterButtons[i + 1]
            };
        }

        foreach (SubButton button in extraButtons)
        {
            button.SetActive(false);
        }

        for (int i = 0; i < extraCount; i++)
        {
            extraButtons[i].SetActive(true);
            extraButtons[i].FadeIn(duration * 0.4f, duration * 0.1f * i);
            extraButtons[i].navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = chapterButtons[0],
                selectOnRight = storyButtons[0],
                selectOnUp = i == 0 ? null : extraButtons[i - 1],
                selectOnDown = i == extraCount - 1 ? null : extraButtons[i + 1]
            };
        }

        foreach (SubButton button in storyButtons)
        {
            button.SetActive(false);
        }

        for (int i = 0; i < storyCount; i++)
        {
            storyButtons[i].SetActive(true);
            storyButtons[i].FadeIn(duration * 0.4f, duration * 0.15f * i);
            storyButtons[i].navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = extraButtons[0],
                selectOnRight = null,
                selectOnUp = i == 0 ? null : storyButtons[i - 1],
                selectOnDown = i == storyCount - 1 ? null : storyButtons[i + 1]
            };
        }
    }

    protected override void OnFadeOut(float duration)
    {
        infoBox.FadeOut(0.6f * duration);
        foreach (StaticImage staticImage in staticImages)
        {
            staticImage.FadeOut(duration * 0.6f);
        }
        foreach (TitleText titleText in titleTexts)
        {
            titleText.FadeOut(duration * 0.6f);
        }

        for (int i = 0; i < chapterCount; i++)
        {
            chapterButtons[i].FadeOut(duration * 0.6f);
        }

        for (int i = 0; i < extraCount; i++)
        {
            extraButtons[i].FadeOut(duration * 0.6f);
        }

        for (int i = 0; i < storyCount; i++)
        {
            storyButtons[i].FadeOut(duration * 0.6f);
        }
    }
}