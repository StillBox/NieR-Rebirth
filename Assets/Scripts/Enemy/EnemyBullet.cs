using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BulletType
{
    SOFT,
    HARD,
    GHOST,
    ARROW
}

public class EnemyBullet : MonoBehaviour
{
    public static BulletType RandomBullet(float value = 0.7f)
    {
        return Random.Range(0f, 1f) < value ? BulletType.SOFT : BulletType.HARD;
    }

    public BulletType type;
    public bool canBeHitted;

    private const float SPEED = 8f;
    private float timeLife = 0f;
    
    void Update()
    {
        transform.Translate(transform.forward * SPEED * Time.deltaTime, Space.World);
        timeLife += Time.deltaTime;
        if (timeLife >= 5f)
        {
            SetActive(false);
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        timeLife = 0f;
    }

    public void OnHit()
    {
        SetActive(false);
    }

    public void Damage()
    {
        SetActive(false);
        SoundManager.instance.PlayEfx(Efx.DESTROY_BULLET, transform.position);
        EffectManager.instance.Explode(transform.position, ExplosionType.SOFT);
        if (type == BulletType.GHOST)
            GhostManager.instance.SetGhost(transform.position, GhostType.JUNIOR);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoftObject"))
        {
            OnHit();
        }
        else if (other.CompareTag("HardObject"))
        {
            OnHit();
            SoundManager.instance.PlayEfx(Efx.HIT_HARD, transform.position);
        }
        else if (other.CompareTag("AirWall"))
        {
            if (AirWall.isBulletBlocked)
            {
                OnHit();
                SoundManager.instance.PlayEfx(Efx.HIT_HARD, transform.position);
            }
        }
        else if (other.CompareTag("Wingman"))
        {
            Wingman wingman = other.GetComponent<Wingman>();
            if (wingman != null)
            {
                wingman.Damage();
            }
            OnHit();
        }
        else if (other.CompareTag("PlayerCore"))
        {
            Player.instance.Damage();
            OnHit();
        }
    }
}
