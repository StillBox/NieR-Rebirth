using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb0 : Enemy
{
    [SerializeField] BombDetector detector;

    public float detectorRadius = 4f;

    private void Start()
    {
        IsInBattle = true;

        detector.Expand(detectorRadius, detectorRadius / 10f);
    }
}
