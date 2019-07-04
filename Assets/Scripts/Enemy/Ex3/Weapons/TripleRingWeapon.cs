using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleRingWeapon : BaseWeapon
{
    private int count = 2;

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
        int waveCounter = 0;
        BulletType type = baseType;
        while (isExcuting && waveCounter < 3)
        {
            if (timer >= 0)
            {
                if (counter < 3)
                {
                    Fire(BaseAngle, 360f / count, count, type, isTypeAltered);
                    SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                    counter++;
                    timer -= BaseGap;
                    if (isTypeAltered) type = AlterType(type);
                }
                else
                {
                    counter = 0;
                    waveCounter++;
                    timer -= BaseGap * 3f;
                }
            }
            yield return null;
            timer += Time.deltaTime;
        }
        isExcuting = false;
    }
}
