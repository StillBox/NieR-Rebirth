using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb1 : Enemy
{
    [SerializeField] private BombDetector detector;
    [SerializeField] private Tracer tracer;
    [SerializeField] private TimeDetonator detonator;

    void Start()
    {
        IsInBattle = true;

        detector.Expand(6f, 0.6f);
        tracer.Set(2f, 4f, 1.5f);
        tracer.Begin();
        detonator.Set(6f);
    }
}