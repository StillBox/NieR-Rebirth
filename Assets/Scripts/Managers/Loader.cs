using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject soundManager;
    public GameObject inputManager;
    public GameObject effectManager;
    public GameObject hudManager;
    public GameObject pauseMenu;
    public GameObject curtain;

    private void Awake()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);

        if (SoundManager.instance == null)
            Instantiate(soundManager);

        if (STBInput.instance == null)
            Instantiate(inputManager);

        if (EffectManager.instance == null)
            Instantiate(effectManager);

        if (HUDManager.instance == null)
            Instantiate(hudManager);
        
        if (PauseMenu.instance == null)
            Instantiate(pauseMenu);

        if (Curtain.instance == null)
            Instantiate(curtain);

        GameManager.instance.LoadData();
    }
}