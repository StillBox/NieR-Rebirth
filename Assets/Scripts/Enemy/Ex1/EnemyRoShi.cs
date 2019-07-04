using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoShi : EnemyBossEx1
{
    private int[] clipLevels;
    private int totalClip = 0;
    private int bulletCount = 5;
    private float deltaAngle = 0f;

    //Functions for attack

    public override void StartMainWeapon()
    {
        isMainWeaponOn = true;
        switch (Phase)
        {
            case 2:
                clipLevels = new int[] { 2, 3, 4 };
                deltaAngle = 32f;
                bulletCount = 5;
                StartCoroutine(MainWeapon(0.16f));
                break;
            case 4:
                clipLevels = new int[] { 4, 5, 6, 7, 8 };
                deltaAngle = 15f;
                bulletCount = 7;
                StartCoroutine(MainWeapon(0.12f));
                break;
            case 5:
                clipLevels = new int[] { 4, 5, 4 };
                deltaAngle = 24f;
                bulletCount = 6;
                StartCoroutine(MainWeapon(0.14f));
                break;
        }
    }

    public override void StartSubWeapon()
    {
        isSubWeaponOn = true;
        switch (Phase)
        {
            case 4:
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
            case 2:
                StartCoroutine(BurstWeapon(0.2f));
                break;
            case 4:
                StartCoroutine(BurstWeapon(0.12f));
                break;
        }
    }

    protected override IEnumerator MainWeapon(float gap)
    {
        isMovable = false;

        float time = 0f;
        int ghostClip = 0;
        int clip = 0;
        float clipGap = gap * (bulletCount + 2);

        while (isMainWeaponOn)
        {
            if (time >= 0)
            {
                isMovable = false;
                time -= 2 * clipGap;
                ghostClip = Random.Range(0, clipLevels.Length);
            }

            if (!isMovable)
            {
                int count = clipLevels[clip];
                bool[] ghosts = new bool[count];
                for (int i = 0; i < count; i++) ghosts[i] = false;
                if (clip == ghostClip && Phase != 5) ghosts[Random.Range(0, count)] = true;

                float center = (count - 1f) / 2f;
                for (int i = 0; i < count; i++)
                {
                    StartCoroutine(RunningFire(bulletCount, deltaAngle * (i - center), gap, ghosts[i], i == 0));
                }

                yield return new WaitForSeconds(clipGap);
                clip++;
                totalClip++;
                if (clip == clipLevels.Length)
                {
                    clip = 0;
                    isMovable = true;
                }
            }
            else
            {
                yield return null;
                time += Time.deltaTime;
            }
        }
    }

    protected override IEnumerator SubWeapon(float gap)
    {
        float time = -gap / 2f;
        int[] levels = Phase == 3 ? new int[] { 3, 2, 3, 2 } : new int[] { 2, 3, 2 };
        int clip = 0;

        while (isSubWeaponOn)
        {
            if (time >= 0)
            {
                isMovable = false;
                time -= gap / 2f;
            }

            if (!isMovable)
            {
                int count = levels[clip];

                float center = (count - 1f) / 2f;
                for (int i = 0; i < count; i++)
                {
                    StartCoroutine(RunningFire(6, 15f * (i - center), 0.1f, false, i == 0));
                }

                yield return new WaitForSeconds(gap / 8f);
                clip++;
                totalClip++;
                if (clip == levels.Length)
                {
                    clip = 0;
                    isMovable = true;
                }
            }
            else
            {
                yield return null;
                time += Time.deltaTime;
            }
        }
    }

    IEnumerator RunningFire(int count, float angle, float gap, bool ghost, bool isMain = false)
    {
        Vector3 euler = transform.eulerAngles;
        Vector3 playerDir = Player.instance.transform.position - transform.position;
        float playerAngle = Vector3.Angle(transform.forward, playerDir) * Mathf.Sign(playerDir.z);
        euler.y += angle + playerAngle;

        bool even = totalClip % 2 == 0;

        int ghostCount = ghost ? Random.Range(0, count) : count;
        for (int i = 0; i < count; i++)
        {
            if (!isInBattle) yield break;

            BulletType type = i == ghostCount ? BulletType.GHOST :
                even || i % 2 == 0 ? BulletType.HARD : BulletType.SOFT;
            EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
            if (isMain) SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
            euler.y += (isMainWeaponOn ? 15f : 7f) / count * (even ? 1f : -1f);
            yield return new WaitForSeconds(gap);
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
            if (Phase == 4)
                angle += Time.deltaTime * 90f;
        }
    }
}
