using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBombEx2 : Enemy
{
    [SerializeField] private Tracer tracer;
    [SerializeField] private BombDetector detector;
    [SerializeField] private TimeDetonator detonator;
    
    void Start()
    {
        IsInBattle = true;

        tracer.Set(3f, 6f, 10f);
        tracer.SetXBoundary(-13f, 13f);
        tracer.SetZBoundary(-9f, 9f);
        tracer.Begin(0.5f);
        detector.Expand(6f, 0.6f);
        detonator.Set(6f);
    }

    protected override void Explode()
    {
        Destroy(detector.gameObject);
        base.Explode();
    }
}