using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossWeaponEx2 : BaseWeapon
{
    private float duration = 0f;
    private float addAngleBeg = 0f;
    private float addAngleEnd = 0f;
    private bool halfGap = false;

    public void StartWeapon(float duration, float addAngleBeg, float addAngleEnd, float gap, bool halfGap, float wait = 0f)
    {
        this.duration = duration;
        this.addAngleBeg = addAngleBeg;
        this.addAngleEnd = addAngleEnd;
        this.gap = gap;
        this.halfGap = halfGap;

        StartWeapon();
        StartCoroutine(Weapon( wait));
    }
    
    IEnumerator Weapon(float wait)
    {
        yield return new WaitForSeconds(0.4f * wait);

        timer = halfGap ? -0.5f * BaseGap : 0f;
        float elapsed = -0.6f * wait;
        BulletType type = baseType;
        while (isExcuting)
        {
            if (timer >= 0f)
            {
                float rate = Mathf.Clamp(elapsed / duration, 0f, 1f);
                float addAngle = Mathf.Lerp(addAngleBeg, addAngleEnd, rate);

                Fire(BaseAngle + addAngle, 90f, 4, type, isTypeAltered);
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);

                timer -= BaseGap;
                if (isTypeAltered) type = AlterType(type);
            }

            yield return null;
            timer += Time.deltaTime;
            elapsed += Time.deltaTime;
        }
    }
}
