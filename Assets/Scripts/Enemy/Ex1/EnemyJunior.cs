using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJunior : Enemy
{
    [SerializeField] private Tracer tracer;
    [SerializeField] private NormalWeapon normalWeapon;
    [SerializeField] private GhostWeapon ghostWeapon;

    public bool IsBossMode = false;
    
    void Start()
    {
        IsInBattle = true;

        tracer.Set(1.5f, 3f, 1.5f);
        tracer.Begin();

        if (IsBossMode)
            normalWeapon.StartWeapon(1f, 0.5f);
        else
            ghostWeapon.StartWeapon(1f, 0.5f);
    }
    
    //For Attack
    /*
    IEnumerator GhostWeapon(float gap, float wait)
    {
        yield return new WaitForSeconds(wait);
        while (IsInBattle)
        {
            if (Random.Range(-1f, 7f) < 0f)
                Fire(BulletType.GHOST);
            else
                Fire(BulletType.HARD);
            SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
            yield return new WaitForSeconds(gap);
        }
    }

    IEnumerator NormalWeapon(float gap, float wait)
    {
        yield return new WaitForSeconds(wait);
        while (IsInBattle)
        {
            Fire(BulletType.SOFT);
            SoundManager.instance.PlayEfx(Efx.FIRE_ENEMY, transform.position);
            yield return new WaitForSeconds(gap);
        }
    }

    private void Fire(BulletType type)
    {
        Vector3 position = transform.position;
        Vector3 euler = transform.eulerAngles;
        EnemyBulletManager.instance.SetBullet(type, position, euler);
    }
    */
    //For Damage and Death
    
    override protected void OnDeath()
    {
        GhostManager.instance.SetGhost(transform.position, GhostType.SENIOR);
        base.OnDeath();
    }
}