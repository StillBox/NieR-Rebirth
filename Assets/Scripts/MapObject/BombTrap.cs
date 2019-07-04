using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrap : MonoBehaviour
{
    [SerializeField] private EnemyBomb0 bombPrefab;
    public float startTime = 0f;
    public float radius = 4f;

    private EnemyBomb0 bomb = null;
    private float timer = 0f;

    private void Start()
    {
        timer = startTime;
    }

    void Update()
    {
        if (bomb == null)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                bomb = EnemyManager.instance.SetEnemy(bombPrefab, transform.position);
                bomb.detectorRadius = radius;
                timer = 3f;
            }
        }
    }
}