using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvergedWeapon : BaseWeapon
{
    private float duration = 0f;

    public void StartWeapon(float duration, float gap, float wait = 0f)
    {
        this.duration = duration;
        this.gap = gap;

        StartWeapon();
        StartCoroutine(Weapon(wait));
    }

    IEnumerator Weapon(float wait)
    {
        yield return new WaitForSeconds(wait);

        timer = 0f;
        float elapsed = 0f;
        bool odd = true;
        BulletType type = baseType;
        while (isExcuting && elapsed < duration)
        {
            if (timer >= 0f)
            {
                float rate = Mathf.Clamp(elapsed - 0.3f * duration, 0f, 0.5f * duration) / (0.5f * duration);
                float delta = 45f * (1f + Mathf.Cos(Mathf.PI * rate));
                Fire(BaseAngle + (odd ? delta : -delta), type);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                timer -= BaseGap;
                odd = !odd;
                if (isTypeAltered && odd) type = AlterType(type);
            }
            yield return null;
            timer += Time.deltaTime;
            elapsed += Time.deltaTime;
        }
        isExcuting = false;
    }
}
