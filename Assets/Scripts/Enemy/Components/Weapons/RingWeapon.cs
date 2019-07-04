using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingWeapon : BaseWeapon
{
    private int count = 2;

    public void SingleFire(int count)
    {
        Fire(BaseAngle, 360f / count, count, baseType, isTypeAltered);
        SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
    }

    public void StartWeapon(int count, float gap, float wait = 0f)
    {
        this.count = count;
        this.gap = gap;
        StartWeapon();
        StartCoroutine(Weapon(wait));
    }

    public void Set(int count, float gap)
    {
        this.count = count;
        this.gap = gap;
    }

    IEnumerator Weapon(float wait)
    {
        yield return new WaitForSeconds(wait);

        timer = 0f;
        BulletType type = baseType;
        while (isExcuting)
        {
            if (timer >= 0f)
            {
                Fire(BaseAngle, 360f / count, count, type, isTypeAltered);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                timer -= BaseGap;
                if (isTypeAltered) type = AlterType(type);
            }

            yield return null;
            timer += Time.deltaTime;
        }
    }
}
