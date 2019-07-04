using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKoShi : EnemyBossEx1
{
    [SerializeField] private EnemyBombEx1 bombPrefab;

    private int clipLevel = 1;
    private float deltaAngle = 30;

    //Functions for attack

    public override void StartMainWeapon()
    {
        isMainWeaponOn = true;
        switch (Phase)
        {
            case 1:
                clipLevel = 1;
                deltaAngle = 45f;
                StartCoroutine(MainWeapon(0.32f));
                break;
            case 3:
                clipLevel = 2;
                deltaAngle = 30f;
                StartCoroutine(MainWeapon(0.16f));
                break;
            case 5:
                clipLevel = 2;
                deltaAngle = 36f;
                StartCoroutine(MainWeapon(0.24f));
                break;
        }
    }

    public override void StartSubWeapon()
    {
        isSubWeaponOn = true;
        switch (Phase)
        {
            case 3:
                StartCoroutine(SubWeapon(8f));
                break;
            case 5:
                StartCoroutine(SubWeapon(16f));
                break;
        }
    }

    public override void StartBurstWeapon()
    {
        switch (Phase)
        {
            case 1:
                StartCoroutine(BurstWeapon(0.25f));
                break;
            case 3:
                StartCoroutine(BurstWeapon(0.15f));
                break;
        }
    }

    protected override IEnumerator MainWeapon(float gap)
    {
        float time = 0f;
        int ghostGap = Phase == 1 ? 12 : 24;
        int count = ghostGap;
        int totalCount = 0;

        while (isMainWeaponOn)
        {
            if (time >= gap)
            {
                if (count == ghostGap && Phase != 5)
                {
                    int ghostIndex = Random.Range(-clipLevel, clipLevel + 1);
                    for (int i = -clipLevel; i <= clipLevel; i++)
                    {
                        BulletType type = i == ghostIndex ? BulletType.GHOST :
                            i == totalCount % (2 * clipLevel + 1) - clipLevel ? BulletType.SOFT : BulletType.HARD;
                        Vector3 euler = transform.eulerAngles;
                        euler.y += deltaAngle * i + 15f * Mathf.Sin(totalCount * 0.3f);
                        EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
                    }
                    count = 0;
                }
                else
                {
                    for (int i = -clipLevel; i <= clipLevel; i++)
                    {
                        BulletType type = i == totalCount % (2 * clipLevel + 1) - clipLevel ? BulletType.SOFT : BulletType.HARD;
                        Vector3 euler = transform.eulerAngles;
                        euler.y += deltaAngle * i + 15f * Mathf.Sin(totalCount * 0.3f);
                        EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
                    }
                    count++;
                }
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
                time -= gap;
                totalCount++;
            }
            yield return null;
            time += Time.deltaTime;
        }
    }

    protected override IEnumerator SubWeapon(float gap)
    {
        float time = 0f;

        while (isSubWeaponOn)
        {
            if (time >= 0f)
            {
                if (Phase == 3)
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        EnemyBombEx1 bomb = EnemyManager.instance.SetEnemy(bombPrefab, transform.position);
                        bomb.IsBossMode = true;
                        bomb.healthPoint = 2;
                        bomb.MoveTo(new Vector3(-3f, 0f, 11f * i), 1f);
                    }
                }
                else if (Phase == 5)
                {
                    EnemyBombEx1 bomb = EnemyManager.instance.SetEnemy(bombPrefab, transform.position);
                    bomb.IsBossMode = true;
                    bomb.healthPoint = 1;
                    bomb.MoveTo(new Vector3(0f, 0f, 8f), 1f);
                }
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
                time -= gap;
            }
            yield return null;
            time += Time.deltaTime;
        }
    }

    protected override IEnumerator BurstWeapon(float gap)
    {
        int count = 64;
        float time = 0f;
        float angle = 0f;
        while (count > 0)
        {
            if (time >= gap)
            {
                Vector3 euler = transform.eulerAngles;
                euler.y += angle;
                for (int i = 0; i < 6; i++)
                {
                    BulletType type = BulletType.HARD;
                    euler.y += 60f;
                    EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
                }
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
                time -= gap;
                count--;
            }
            yield return null;
            time += Time.deltaTime;
            if (Phase == 3)
                angle += Time.deltaTime * 45f;
        }
    }
}