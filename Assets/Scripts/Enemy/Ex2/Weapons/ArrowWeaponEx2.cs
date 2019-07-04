using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowWeaponEx2 : BaseWeapon
{
    [SerializeField] private EnemyArrowEx2 arrowPrefab;

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
        int counter = 0;
        while (isExcuting)
        {
            if (timer >= 0f)
            {
                if (counter < 4)
                {
                    Enemy arrow = EnemyManager.instance.SetEnemy(arrowPrefab, transform.position);
                    Vector3 euler = transform.eulerAngles;
                    float addAngle = counter < 2 ? 90f : 150f;
                    if (counter % 2 == 0) addAngle *= -1f;
                    euler.y += addAngle;
                    arrow.transform.eulerAngles = euler;
                    arrow.MoveBy(arrow.transform.forward * 4f, 0.5f);

                    counter++;
                    timer -= 0.08f;
                }
                else
                {
                    counter = 0;
                    timer -= BaseGap - 0.32f;
                }
            }
            yield return null;
            timer += Time.deltaTime;
        }
    }
}
