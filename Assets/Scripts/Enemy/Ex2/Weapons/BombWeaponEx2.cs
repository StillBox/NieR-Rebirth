using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWeaponEx2 : BaseWeapon
{
    [SerializeField] private EnemyBombEx2 bombPrefab;

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
        while (isExcuting)
        {
            if (timer >= 0f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Enemy bomb = EnemyManager.instance.SetEnemy(bombPrefab, transform.position);
                    bomb.MoveTo(transform.right * (2f * i - 1f) * 7f, 0.5f);
                }
                SoundManager.instance.PlayEfx(Efx.SHOW_UP, transform.position);

                timer -= BaseGap;
            }

            yield return null;
            timer += Time.deltaTime;
        }
    }
}