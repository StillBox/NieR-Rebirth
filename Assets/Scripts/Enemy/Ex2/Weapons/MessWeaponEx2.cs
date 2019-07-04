using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessWeaponEx2 : BaseWeapon
{
    private int count = 3;

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
        BulletType type = baseType;
        while (isExcuting)
        {
            if (timer >= 0f)
            {
                float addAngle = Random.Range(0f, 360f);
                Fire(BaseAngle + addAngle, 360f / count, count, type, isTypeAltered);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                timer -= BaseGap;
                if (isTypeAltered) type = AlterType(type);
            }

            yield return null;
            timer += Time.deltaTime;
        }
    }
}
