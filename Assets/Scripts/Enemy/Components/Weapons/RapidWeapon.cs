using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidWeapon : BaseWeapon
{
    private int count = 1;

    public void StartWeapon(int count, float gap, float wait = 0f)
    {
        this.count = count;
        this.gap = gap;

        StartWeapon();
        StartCoroutine(Weapon(wait));
    }

    IEnumerator Weapon(float wait)
    {
        yield return new WaitForSeconds(wait);

        timer = 0f;
        int counter = 0;
        BulletType type = baseType;
        while (isExcuting)
        {
            if (timer >= 0f)
            {
                if (counter < count)
                {
                    Fire(BaseAngle, type);
                    SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                    counter++;
                    timer -= 0.08f;
                    if (isTypeAltered) type = AlterType(type);
                }
                else
                {
                    counter = 0;
                    timer -= BaseGap;
                }
            }
            yield return null;
            timer += Time.deltaTime;
        }
    }
}
