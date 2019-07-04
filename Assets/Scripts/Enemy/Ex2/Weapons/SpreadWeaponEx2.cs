using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadWeaponEx2 : BaseWeapon
{
    private float count;
    private float period;
    private bool halfGap = false;

    private List<float> timeSpread;

    public void StartWeapon(int count, float period, float gap, bool halfGap, float wait = 0f)
    {
        this.count = count;
        this.period = period;
        this.gap = gap;
        this.halfGap = halfGap;

        StartWeapon();
        StartCoroutine(Weapon(wait));
    }

    IEnumerator Weapon(float wait)
    {
        yield return new WaitForSeconds(wait);

        timer = halfGap ? -0.5f * BaseGap : 0f;
        bool odd = true;
        int counter = 0;
        float elapsed = 0f;
        timeSpread = new List<float>();
        while (isExcuting)
        {
            int actCount = timeSpread.Count;
            if (timer >= 0f)
            {
                for (int i = 0; i < actCount; i++)
                {
                    BulletType type = BulletType.HARD;
                    float phase = 0.25f * Mathf.PI * timeSpread[i] / period;
                    float addAngle = 180f * Mathf.Sin(phase);
                    if (odd)
                    {
                        if ((counter - actCount + i) % 3 == 0)
                            type = BulletType.SOFT;
                        Fire(BaseAngle + addAngle, type);
                    }
                    else
                    {
                        if ((counter - actCount + i) % 3 == 1)
                            type = BulletType.SOFT;
                        Fire(BaseAngle - addAngle, type);
                    }
                }
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
                timer -= BaseGap;
                odd = !odd;
            }
            if (elapsed >= 0f && counter < count)
            {
                elapsed -= period / 3f;
                timeSpread.Add(0f);
                counter++;
            }

            yield return null;
            timer += Time.deltaTime;
            elapsed += Time.deltaTime;
            for (int i = 0; i < actCount; i++)
            {
                timeSpread[i] += Time.deltaTime;
            }
            if (timeSpread.Count > 0 && timeSpread[0] >= 2 * period)
            {
                timeSpread.RemoveAt(0);
            }
        }
    }
}