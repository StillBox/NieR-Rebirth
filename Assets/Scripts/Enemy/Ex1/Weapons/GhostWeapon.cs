using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWeapon : BaseWeapon
{
    public void StartWeapon(float gap, float wait)
    {
        this.gap = gap;

        StartWeapon();
        StartCoroutine(Weapon(wait));
    }

    IEnumerator Weapon(float wait)
    {
        yield return new WaitForSeconds(wait);

        timer = 0f;
        while (isExcuting)
        {
            if (timer >= 0f)
            {
                BulletType type = Random.Range(0, 8) == 0 ? BulletType.GHOST : BulletType.SOFT;
                Fire(BaseAngle, type);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                timer -= BaseGap;
            }

            yield return null;
            timer += Time.deltaTime;
        }
    }
}
