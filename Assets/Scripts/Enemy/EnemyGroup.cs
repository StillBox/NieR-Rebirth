using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGroup : MonoBehaviour
{
    public EnemyWave[] waves;
    public UnityEvent onClear;

    private bool isActivated;
    private int waveCount;
    private int currentWave;

    public bool IsAllClear
    {
        get;  private set;
    }

    private void Awake()
    {
        SetActive(false);
        isActivated = false;
    }

    void Start()
    {
        waveCount = waves.Length;
        currentWave = -1;
    }

    private void Update()
    {
        if (!isActivated) return;

        if (IsAllClear) return;

        if (currentWave < 0 || waves[currentWave].IsClear)
        {
            currentWave++;
            if (currentWave == waveCount)
            {
                IsAllClear = true;
                onClear.Invoke();
            }
            else
                waves[currentWave].SetActive(true);
        }
    }

    private void SetActive(bool value)
    {
        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].SetActive(value);
        }
    }

    public void Activate()
    {
        isActivated = true;
    }
}