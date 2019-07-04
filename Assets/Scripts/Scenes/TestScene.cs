using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : SceneController
{
    public CubeGroup cubeGroup;
    public Snowflake snowflake;
    public Camera mainCamera;
    public Camera uiCamera;

    public GameObject platform;
    public Enemy[] testEnemies;
    
    void Start()
    {
        Player.IsMovable = true;
        Player.IsArmed = true;
        
        HUDManager.instance.SetCamera(mainCamera);
        PauseMenu.instance.SetCamera(uiCamera);
        /*
        foreach (Enemy enemy in testEnemies)
            enemy.SetActive(true);
            */
    }

    void Search()
    {
        EffectManager.instance.SetSearchLight(new Vector3(-3f, 0f, 0f));
    }

    private void Update()
    {
        if (STBInput.GetButtonDown("Pause"))
        {
            PauseMenu.instance.CallPauseMenu();
        }        
    }

    /*
    Enemy CreateRandomEnemy()
    {
        Enemy newEnemy = Instantiate(enemyPrefab, GetRandomPositionOnPlatform(), Quaternion.identity);
        newEnemy.SetActive(true);
        return newEnemy;
    }

    Vector3 GetRandomPositionOnPlatform()
    {
        Vector3 center = platform.transform.position;
        float width = platform.transform.localScale.x;
        float height = platform.transform.localScale.z;
        Vector3 position = center;
        position.x += Random.Range(-width / 2f + 2f, width / 2f - 2f);
        position.z += Random.Range(-height / 2f + 2f, height / 2f - 2f);
        return position;
    }
    */
}