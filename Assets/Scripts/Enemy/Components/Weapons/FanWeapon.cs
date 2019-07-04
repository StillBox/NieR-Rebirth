using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanWeapon : BaseWeapon
{
    private int count = 1;
    private float delta = 30f;

    public void StartWeapon(int count, float delta, float gap, float wait = 0f)
    {
        this.count = count;
        this.delta = delta;
        this.gap = gap;

        StartWeapon();
        StartCoroutine(Weapon(wait));
    }

    public void Set(int count, float delta, float gap)
    {
        this.count = count;
        this.delta = delta;
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
                Fire(BaseAngle - delta * (count - 1) / 2f, delta, count, type, isTypeAltered);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                timer -= BaseGap;
                if (isTypeAltered) type = AlterType(type);
            }

            yield return null;
            timer += Time.deltaTime;
        }
    }
}
