using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKoShiSatelite : EnemySateliteEx1
{
    private const float ORIGIN_RADIUS = 1.8f;
    private const float ORBIT_RADIUS = 3.2f;
    private const float ORBIT_SPEED = 60f;

    private const float FIRE_GAP = 0.2f;

    [SerializeField] private NormalWeapon weapon;

    private Angle localAngle;
    
    protected override IEnumerator Launch()
    {
        StartCoroutine(ChangeCoreSscale(1f, 0f, 1f));
        float time = 0f;
        while (time < 1f)
        {
            float radius = Mathf.Lerp(ORIGIN_RADIUS, ORBIT_RADIUS, time);
            localAngle = (5f * time + orbitDir) * 90f;
            SetLocalPosition(radius, localAngle);
            yield return null;
            time += Time.deltaTime;
        }
        yield return new WaitForSeconds(0.5f);     
        
        localAngle = (1f + orbitDir) * 90f;
        SetLocalPosition(ORBIT_RADIUS, localAngle);
        isOrbiting = true;
        StartCoroutine(Orbit());
    }

    protected override IEnumerator Orbit()
    {
        while (isOrbiting)
        {
            localAngle += ORBIT_SPEED * Time.deltaTime;
            SetLocalPosition(ORBIT_RADIUS, localAngle * orbitDir);
            transform.LookAt(Player.instance.transform);

            yield return null;
            if (weapon.IsExcuting)
            {
                if (!IsInBattle || localAngle < 0f) weapon.StopWeapon();
            }
            else
            {
                if (IsInBattle && localAngle >= 0) weapon.StartWeapon(FIRE_GAP, 0f);
            }
            /*
            if (IsInBattle && localAngle>= 0f)
            {
                time += Time.deltaTime;
                if (time >= 0f)
                {
                    transform.LookAt(Player.instance.transform);
                    BulletType type = odd ? BulletType.SOFT : BulletType.HARD;
                    EnemyBulletManager.instance.SetBullet(type, transform.position, transform.eulerAngles);
                    SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
                    time -= FIRE_GAP;
                    odd = !odd;
                }
            }
            */
        }
    }

    protected override IEnumerator Land()
    {
        StartCoroutine(ChangeCoreSscale(0f, 1f, 1f));
        float time = 0f;
        float beg = localAngle;
        while (time < 1f)
        {
            float radius = Mathf.Lerp(ORBIT_RADIUS, ORIGIN_RADIUS, time);
            float angle = Mathf.Lerp(beg, (4f + orbitDir) * 90f, time);
            SetLocalPosition(radius, angle);
            yield return null;
            time += Time.deltaTime;
        }
    }

    private void SetLocalPosition(float radius, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        transform.localPosition = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * radius;
    }
}