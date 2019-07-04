using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class EnemyType0 : Enemy
{
    public enum MotionMode
    {
        TRACE,
        MARBLE,
        SPRINTER,
        CRUISE_WAVE,
        CRUISE_MOBIUS,
        CRUISE_CIRCLE
    }
    public readonly MotionMode[] motionModes =
    {
        MotionMode.TRACE,
        MotionMode.MARBLE,
        MotionMode.SPRINTER,
        MotionMode.CRUISE_WAVE,
        MotionMode.CRUISE_MOBIUS,
        MotionMode.CRUISE_CIRCLE
    };

    public enum FireMode
    {
        NORMAL,
        RAPID,
        RING,
        SIDE
    }
    public readonly FireMode[] modeGroup0 =
    {
        FireMode.RING,
        FireMode.SIDE
    };
    public readonly FireMode[] modeGroup1 =
    {
        FireMode.NORMAL,
        FireMode.RAPID,
        FireMode.RING
    };

    public MotionMode motionMode;
    public FireMode fireMode;
    
    void Start()
    {
        IsInBattle = true;

        motionMode = motionModes[Random.Range(0, motionModes.Length)];
        switch (motionMode)
        {
            case MotionMode.TRACE:
                gameObject.AddComponent<Tracer>().Begin();
                break;
            case MotionMode.MARBLE:
                gameObject.AddComponent<Marble>().Begin();
                break;
            case MotionMode.SPRINTER:
                gameObject.AddComponent<Sprinter>().Begin();
                break;
            case MotionMode.CRUISE_WAVE:
                gameObject.AddComponent<WaveCruiser>().Begin();
                break;
            case MotionMode.CRUISE_CIRCLE:
                gameObject.AddComponent<CircleCruiser>().Begin();
                break;
            case MotionMode.CRUISE_MOBIUS:
                gameObject.AddComponent<MobiusCruiser>().Begin();
                break;
        }

        if (motionMode == MotionMode.SPRINTER)
        {
            fireMode = modeGroup0[Random.Range(0, modeGroup0.Length)];
        }
        else
        {
            fireMode = modeGroup1[Random.Range(0, modeGroup1.Length)];
        }
        switch (fireMode)
        {
            case FireMode.NORMAL:
                gameObject.AddComponent<NormalWeapon>().StartWeapon(0.25f, 0.5f);
                break;
            case FireMode.RAPID:
                gameObject.AddComponent<RapidWeapon>().StartWeapon(8, 1f, 0.5f);
                break;
            case FireMode.RING:
                gameObject.AddComponent<RingWeapon>().StartWeapon(8, 2f, 0.5f);
                break;
            case FireMode.SIDE:
                gameObject.AddComponent<SideWeapon>().StartWeapon(0.25f, 0.5f);
                break;
        }
    }

    //For movement
    /*
    private const float TRACE_SPEED = 2f;
    IEnumerator Trace()
    {
        while (true)
        {
            Vector3 moveDir = Player.instance.transform.position - transform.position;
            speed = Mathf.Min(speed + 2f * Time.deltaTime, TRACE_SPEED);
            Vector3 movement = moveDir.normalized * speed * Time.deltaTime;
            controller.Move(movement);
            yield return null;
        }
    }

    private const float DASH_SPEED = 10f;
    private const float DASH_GAP = 0.6f;
    IEnumerator Dash()
    {
        while (true)
        {
            Vector3 moveDir = Player.instance.transform.position - transform.position;
            speed = Mathf.Epsilon;
            while (speed > 0f)
            {
                speed = Mathf.Min(speed + 20f * Time.deltaTime, DASH_SPEED);
                Vector3 movement = moveDir.normalized * speed * Time.deltaTime;
                controller.Move(movement);
                yield return null;
            }
            yield return new WaitForSeconds(DASH_GAP);
        }
    }

    private const float PATROL_SPEED = 3f;
    private const float PATROL_RADIUS = 3f;
    IEnumerator Patrol()
    {
        Vector3 center = transform.position;
        Vector3 playerDir = Player.instance.transform.position - transform.position;
        float phase = -Vector3.SignedAngle(Vector3.forward, playerDir, Vector3.up) * Mathf.Deg2Rad;

        while (true)
        {
            phase += PATROL_SPEED / PATROL_RADIUS * Time.deltaTime;
            Vector3 target = center + PATROL_RADIUS * new Vector3(Mathf.Sin(phase), 0f, Mathf.Cos(phase));
            Vector3 offset = target - transform.position;
            speed = Mathf.Min(speed + 20f * Time.deltaTime, PATROL_SPEED);
            offset = offset.normalized * Mathf.Min(offset.magnitude, speed * Time.deltaTime);
            controller.Move(offset);
            yield return null;
        }
    }

    override protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Player"))
        {
            CharacterController controller = hit.collider.GetComponent<CharacterController>();
            if (controller == null) return;

            Vector3 pushDir = new Vector3(hit.normal.x, 0f, hit.normal.z);
            Vector3 moveDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            float pushSpeed = speed * Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(moveDir, pushDir));
            controller.Move(pushDir * pushSpeed * Time.deltaTime);
        }
        else if (!hit.collider.CompareTag("Floor"))
        {
            speed = 0f;
        }
    }
    */

    //For attack
    /*
    private void Fire()
    {
        float gap = 1f;

        switch (fireMode)
        {
            case FireMode.NORMAL:
                gap = 0.5f;
                FireMode0();
                break;
            case FireMode.RAPID:
                gap = 1f;
                FireMode1();
                break;
            case FireMode.CROSS:
                gap = 1.5f;
                StartCoroutine(FireMode2());
                break;
        }
        SoundManager.instance.PlayEfx(9, transform.position);

        Invoke("Fire", gap);
    }

    void FireMode0()
    {
        Vector3 position = transform.position;
        Vector3 playerDir = Player.instance.transform.position - position;
        Vector3 euler = Quaternion.LookRotation(playerDir).eulerAngles;
        EnemyBulletManager.instance.SetBullet(EnemyBullet.RandomBullet(), position, euler);
    }

    void FireMode1()
    {
        Vector3 position = transform.position;
        Vector3 euler = transform.eulerAngles;
        EnemyBulletManager.instance.SetBullet(EnemyBullet.RandomBullet(), position, euler);
        euler.y += 90f;
        EnemyBulletManager.instance.SetBullet(EnemyBullet.RandomBullet(), position, euler);
        euler.y += 90f;
        EnemyBulletManager.instance.SetBullet(EnemyBullet.RandomBullet(), position, euler);
        euler.y += 90f;
        EnemyBulletManager.instance.SetBullet(EnemyBullet.RandomBullet(), position, euler);
    }

    IEnumerator FireMode2()
    {
        int count = 0;
        float time = 0f;
        while (count < 5)
        {
            if (time >= 0.11f)
            {
                time -= 0.11f;
                count++;
                FireMode0();
            }
            yield return null;
            time += Time.deltaTime;
        }
    }
    */
}