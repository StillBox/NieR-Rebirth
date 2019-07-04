using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProtoEx3 : Enemy
{
    public static int deathCount = 0;
    public static List<EnemyProtoEx3> protos = new List<EnemyProtoEx3>();

    public static void AllReset()
    {
        deathCount = 0;
        protos = new List<EnemyProtoEx3>();
    }

    public int id = 0;

    [SerializeField] private BossShield shield;
    [SerializeField] private EmilGhost ghostPrefab;
    [SerializeField] private Tracer tracer;
    [SerializeField] private NormalWeapon weapon;

    public override bool IsInBattle
    {
        get { return isInBattle; }
        set
        {
            isInBattle = value;
            shield.gameObject.SetActive(!value);
            controller.radius = value ? 0.4f : 0.5f;
            if (!isInBattle)
            {
                tracer.Stop();
                weapon.StopWeapon();
            }
        }
    }

    void Start()
    {
        protos.Add(this);
        id = protos.Count - 1;

        IsInBattle = true;

        tracer.Set(0.5f, 1f, 1.5f);
        tracer.Begin();

        weapon.SetType(BulletType.SOFT, false);
        weapon.StartWeapon(3f, 1.5f);
    }
    
    public void Burst()
    {
        EnemyManager.instance.Remove(this);
        EffectManager.instance.Explode(transform.position, ExplosionType.SMALL);
        Destroy(gameObject);
    }

    public override void Damage(int damagePoint = 1)
    {
        if (IsInBattle)
        {
            base.Damage(damagePoint);
        }
        else
        {
            SoundManager.instance.PlayEfx(Efx.HIT_HARD_ENEMY, transform.position);
        }
    }
    
    protected override void OnDeath()
    {
        deathCount++;

        foreach (EnemyProtoEx3 proto in protos)
        {
            proto.IsInBattle = false;
        }

        if (deathCount <= 4)
        {
            for (int i = 0; i < 2; i++)
            {
                EmilGhost ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
                bool newPosition = true;
                Vector3 position = Vector3.zero;
                while (newPosition)
                {
                    newPosition = false;
                    position = new Vector3(Random.Range(-11f, 11f), 0f, Random.Range(-8f, 8f));
                    foreach (EnemyProtoEx3 proto in protos)
                    {
                        if ((position - proto.transform.position).magnitude <= MapEx3.GRID_SIZE)
                            newPosition = true;
                    }
                }
                ghost.rebirthPosition = position;
                ghost.rebirthTime = 3f + 0.5f * i;
            }
        }
    }
}