using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : Enemy
{
    [SerializeField] private GameObject bossShield;
    [SerializeField] private Tracer tracer;
    [SerializeField] private FanWeapon fanWeapon;
    [SerializeField] private RingWeapon ringWeapon;

    private int weaponLevel = 0;
    public int WeaponLevel
    {
        get { return weaponLevel; }
        set
        {
            weaponLevel = value;
            switch (value)
            {
                case 0:
                    fanWeapon.Set(3, 45f, 2f);
                    break;
                case 1:
                    fanWeapon.Set(5, 30f, 2f);
                    break;
                case 2:
                    fanWeapon.Set(5, 30f, 1f);
                    break;
            }
        }
    }

    private bool isInvincible = false;
    public bool IsInvincible
    {
        get { return isInvincible; }
        set
        {
            isInvincible = value;
            bossShield.SetActive(isInvincible);
            controller.radius = isInvincible ? 1f : 0.4f;
            if (!isInvincible)
            {
                SoundManager.instance.PlayEfx(Efx.BREAK, transform.position);
                fanWeapon.StopWeapon();
                ringWeapon.SetType(BulletType.HARD, false);
                ringWeapon.SetRotation(0f, 45f);
                ringWeapon.StartWeapon(8, 0.2f, 1f);
            }
        }
    }
            
    void Start()
    {
        IsInvincible = true;
        IsInBattle = true;

        tracer.Set(1f, 2f, 3f);
        tracer.Begin();

        fanWeapon.SetType(BulletType.HARD, false);
        switch (weaponLevel)
        {
            case 0:
                fanWeapon.StartWeapon(3, 45f, 2f, 1f);
                break;
            case 1:
                fanWeapon.StartWeapon(5, 30f, 2f, 1f);
                break;
            case 2:
                fanWeapon.StartWeapon(5, 30f, 1f, 1f);
                break;
        }
    }

    public override void Damage(int damagePoint = 1)
    {
        if (IsInvincible)
            SoundManager.instance.PlayEfx(Efx.HIT_HARD_ENEMY, transform.position);
        else
            base.Damage(damagePoint);
    }
}