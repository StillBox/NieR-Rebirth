using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : SceneController
{
    [SerializeField] private CutOff cutOff;
    [SerializeField] private LoadingText[] loadingTexts;
    
    private void Start()
    {
        Curtain.instance.ChangeColor(0.5f, Curtain.black, Curtain.black_clear);
        cutOff.TurnOn();
        StartCoroutine(LoadScene(0.5f));
    }

    IEnumerator LoadScene(float wait = 0f)
    {
        yield return new WaitForSeconds(wait);

        LoadingText current = loadingTexts[0];
        current.FadeIn(0.2f);
        yield return new WaitForSeconds(0.5f);

        GameManager.LoadScene(false);
        
        int count = loadingTexts.Length;

        for (int index = 1; index < count; index++)
        {
            current.Finish();
            current = loadingTexts[index];
            current.FadeIn(0.2f);
            float progress = 0.9f * index / (count - 1);
            while (GameManager.LoadProgress < progress || !current.IsComplete) yield return null;
        }

        yield return new WaitForSeconds(1f);
        Curtain.instance.ChangeColor(1f, Curtain.black_clear, Curtain.black, ActivateScene);
    }

    private void ActivateScene()
    {
        if (GameManager.gameScene == GameScene.Story)
        {
            if (GameManager.storyChapter != StoryChapter.Intro)
            {
                Curtain.instance.ChangeColor(1f, Curtain.black, Curtain.white, GameManager.ActivateScene);
                return;
            }
        }

        GameManager.ActivateScene();
    }
}
