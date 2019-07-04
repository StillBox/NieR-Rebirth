using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySenior : Enemy
{
    [SerializeField] private VerticalTracer tracer;
    [SerializeField] private RingWeapon ringWeapon;

    public bool IsBossMode = false;
    
    void Start()
    {
        IsInBattle = true;

        if (IsBossMode)
        {
            ringWeapon.SetRotation(Random.Range(-90f, 90f), 20f);
            ringWeapon.StartWeapon(2, 0.25f, 0.5f);
        }
        else
        {
            tracer.SetSpeed(2f, 4f);
            tracer.Begin();

            switch (Random.Range(0, 2))
            {
                case 0:
                    ringWeapon.SetRotation(0f, 0f);
                    break;
                case 1:
                    ringWeapon.SetRotation(45f, 0f);
                    break;
            }
            ringWeapon.StartWeapon(4, 0.25f, 0.5f);
        }   
    }

    //For Movement
    /*
    IEnumerator VerticalTrace(float maxSpeed, float accel)
    {
        while (IsInBattle)
        {
            float dir = Player.instance.PositionZ - PositionZ;
            Vector3 velocity = Mathf.Sign(dir) * Vector3.forward * speed;
            if (!controller.isGrounded) velocity.y = -9.8f;
            controller.Move(velocity * Time.deltaTime);
            yield return null;
            speed = Mathf.Min(maxSpeed, speed + accel * Time.deltaTime);
            speed = Mathf.Min(Mathf.Abs(dir) / 2f, speed);
        }
    }
    */

    //For Attack
    /*
    IEnumerator CrossWeapon(float gap, float wait)
    {
        yield return new WaitForSeconds(wait);

        int mode = Random.Range(0, 3);
        float angle = mode == 0 ? 0f : mode == 1 ? 45f : 0f;
        while (true)
        {
            Fire(angle, 90f, 4, BulletType.SOFT, true);
            SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
            yield return new WaitForSeconds(gap);
            if (mode == 2)
                angle += 45f;
            else
                angle += 90f;
        }
    }

    private const float ROTATE_SPEED = 30f;

    IEnumerator RotateWeapon(float gap, float wait)
    {
        yield return new WaitForSeconds(wait);

        float time = 0f;
        float angle = 0f;
        float speed = Mathf.Sign(Random.Range(-1f, 1f)) * ROTATE_SPEED;
        bool odd = true;
        while (true)
        {
            if (time <= 0f)
            {
                Fire(angle, 180f, 2, odd ? BulletType.HARD : BulletType.SOFT, true);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
                time += gap;
                odd = !odd;
            }
            yield return null;
            time -= Time.deltaTime;
            angle += speed * Time.deltaTime;
        }
    }

    private void Fire(float angleBeg, float angleDelta, int count, BulletType typeProto, bool alter = false)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 euler = new Vector3(0f, angleBeg + angleDelta * i, 0f);
            BulletType type = typeProto;
            if (alter && i % 2 == 1)
            {
                type = typeProto == BulletType.HARD ? BulletType.SOFT : BulletType.HARD;
            }
            EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
        }
    }
    */
}
