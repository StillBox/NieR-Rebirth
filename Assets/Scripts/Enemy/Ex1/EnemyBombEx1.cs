using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBombEx1 : Enemy
{
    [SerializeField] private Tracer tracer;
    [SerializeField] private BombDetector detector;
    [SerializeField] private TimeDetonator detonator;

    public bool IsBossMode = false;

    private float speed = 0f;

    void Start()
    {
        IsInBattle = true;

        if (IsBossMode)
        {
            tracer.Set(2f, 4f, 5f);
            tracer.SetXBoundary(-3f, 3f);
            tracer.Begin(1f);
            detector.Expand(6f, 0.6f, 1f);
            detonator.Set(7f);
        }
        else
        {
            detector.Expand(6f, 0.6f);
            StartCoroutine(DashBack(15f, 30f));
        }
    }
    
    IEnumerator DashBack(float max, float rot, float wait = 0f)
    {
        transform.Rotate(0f, Random.Range(0f, 180f), 0f);
        yield return new WaitForSeconds(wait);

        while (IsInBattle)
        {
            speed = max;
            Vector3 velocity = Vector3.back * speed;
            if (!controller.isGrounded)
                velocity.y = -9.8f;
            controller.Move(velocity * Time.deltaTime);
            transform.Rotate(0f, rot * Time.deltaTime, 0f);
            if (PositionZ <= -8f)
                Damage(999);
            yield return null;
        }
    }

    protected override void Explode()
    {
        Destroy(detector.gameObject);
        base.Explode();
    }
}