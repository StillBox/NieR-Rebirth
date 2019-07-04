using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public static float speed = 16f;
    private float timeLife = 0f;
    
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        timeLife += Time.deltaTime;
        if (timeLife >= 3f)
        {
            SetActive(false);
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        timeLife = 0f;
    }

    private void OnHit(bool damage)
    {
        SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HardObject"))
        {
            OnHit(false);
            SoundManager.instance.PlayEfx(Efx.HIT_HARD, transform.position);
            EffectManager.instance.Explode(other.ClosestPoint(transform.position), ExplosionType.HARD);
        }
        else if (other.CompareTag("SoftObject"))
        {
            OnHit(true);
            other.SendMessage("Damage");
            EffectManager.instance.Explode(other.ClosestPoint(transform.position), ExplosionType.SOFT);
        }
        else if (other.CompareTag("AirWall"))
        {
            AirWall airWall = other.GetComponent<AirWall>();
            if (airWall && AirWall.isBulletBlocked)
            {
                OnHit(false);
                SoundManager.instance.PlayEfx(Efx.HIT_HARD, transform.position);
                EffectManager.instance.Explode(other.ClosestPoint(transform.position), ExplosionType.HARD);
            }
        }
        else if (other.CompareTag("EnemyShield"))
        {
            OnHit(false);
            SoundManager.instance.PlayEfx(Efx.HIT_HARD_ENEMY, transform.position);
            EffectManager.instance.Explode(other.ClosestPoint(transform.position), ExplosionType.SOFT);
        }
        else if (other.CompareTag("Enemy"))
        {
            OnHit(true);
            other.GetComponent<Enemy>().Damage();
            EffectManager.instance.Explode(other.ClosestPoint(transform.position), ExplosionType.SOFT);
        }
        else if (other.CompareTag("EnemyBullet"))
        {
            EnemyBullet bullet = other.GetComponent<EnemyBullet>();
            if (bullet.canBeHitted)
            {
                OnHit(true);
                bullet.Damage();
            }
        }
    }
}
