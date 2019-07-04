using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDetonator : MonoBehaviour
{
    private Enemy parent = null;

    private float timer = 999f;
    private bool isExcuting = false;

    private void Awake()
    {
        parent = GetComponent<Enemy>();
    }

    void Update()
    {
        if (isExcuting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                if (parent != null) parent.Damage(999);
                isExcuting = false;
            }
        }
    }

    public void Set(float time)
    {
        timer = time;
        isExcuting = true;
    }

    public void Pause()
    {
        isExcuting = false;
    }

    public void UnPause()
    {
        isExcuting = true;
    }
}
