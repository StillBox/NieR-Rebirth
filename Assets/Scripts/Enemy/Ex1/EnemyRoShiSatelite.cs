using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoShiSatelite : EnemySateliteEx1
{
    private const float ORIGIN_OFFSET = 1.8f;
    private const float ORBIT_OFFSET = 7f;
    private const float PERIOD = 6f;
    private const float FIRE_GAP = 0.2f;

    [SerializeField] private HorizontalTracer tracer;
    [SerializeField] private RingWeapon weapon;

    public float begRate = 0f;
    public Transform parent = null;
    
    protected override IEnumerator Launch()
    {
        transform.SetParent(null);
        MoveTo(new Vector3(Player.instance.PositionX, 0f, ORBIT_OFFSET * orbitDir), 1f);
        StartCoroutine(ChangeCoreSscale(1f, 0f, 1f));
        yield return new WaitForSeconds(1.5f);
        isOrbiting = true;
        StartCoroutine(Orbit());
    }

    protected override IEnumerator Orbit()
    {
        tracer.SetSpeed(8f, 16f);
        tracer.Begin();
        yield return new WaitForSeconds(PERIOD * begRate);

        while (isOrbiting)
        {
            tracer.Stop(true);
            weapon.StartWeapon(2, FIRE_GAP);

            yield return new WaitForSeconds(PERIOD);
            if (!isOrbiting) yield break;

            weapon.StopWeapon();
            tracer.Begin();

            yield return new WaitForSeconds(0.4f * PERIOD);
            /*
            if (fireTime > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    BulletType type = fireCount % 2 == 0 ? BulletType.SOFT : BulletType.HARD;
                    EnemyBulletManager.instance.SetBullet(type, transform.position, new Vector3(0f, 180f * i, 0f));
                }
                fireCount++;
                SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
                yield return new WaitForSeconds(FIRE_GAP);
                fireTime -= FIRE_GAP;
                if (fireTime <= 0f) timer += PERIOD * 0.4f;
            }
            else
            {
                float xDir = Player.instance.PositionX - PositionX;
                float speed = Mathf.Min(Mathf.Abs(xDir) * 10f, maxSpeed);
                transform.Translate(Mathf.Sign(xDir) * Vector3.back * speed * Time.deltaTime);                
                yield return null;
                timer -= Time.deltaTime;
                if (timer <= 0f) fireTime += PERIOD;
            }
            */
        }
    }

    protected override IEnumerator Land()
    {
        tracer.Stop();
        weapon.StopWeapon();

        transform.SetParent(parent);
        StartCoroutine(ChangeCoreSscale(0f, 1f, 1f));
        float time = 0f;
        Vector3 beg = transform.localPosition;
        Vector3 end = new Vector3(ORIGIN_OFFSET * orbitDir, 0f, 0f);
        Vector3 dir = end - beg;
        while (time < 1f)
        {
            Vector3 position = beg + dir * time;
            transform.localPosition = position;
            yield return null;
            time += Time.deltaTime;
        }
    }    
}