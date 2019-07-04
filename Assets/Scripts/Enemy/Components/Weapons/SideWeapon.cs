using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWeapon : BaseWeapon
{
    public void StartWeapon(float gap, float wait = 0f)
    {
        this.gap = gap;

        StartWeapon();
        StartCoroutine(Weapon(wait));
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
                Fire(BaseAngle + 90f, 180f, 2, type, isTypeAltered);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                timer -= BaseGap;
                if (isTypeAltered) type = AlterType(type);
            }

            yield return null;
            timer += Time.deltaTime;
        }
    }
}
