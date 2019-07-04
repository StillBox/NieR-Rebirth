using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletPrefabInfo
{
    public EnemyBullet prefab;
    public int count;
}

public class EnemyBulletManager : MonoBehaviour
{
    public static EnemyBulletManager instance = null;

    public BulletPrefabInfo[] bulletPrefabs;
    public EnemyArrow arrowPrefab;

    private Dictionary<BulletType, Queue<EnemyBullet>> bulletPools;
    private Transform enemyBulletsHolder;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        bulletPools = new Dictionary<BulletType, Queue<EnemyBullet>>();
    }

    void Start()
    {
        enemyBulletsHolder = new GameObject("EnemyBullets").transform;
        
        for (int i = 0; i < bulletPrefabs.Length; i++)
        {
            BulletPrefabInfo info = bulletPrefabs[i];
            Queue<EnemyBullet> bulletPool = new Queue<EnemyBullet>();

            Transform typeHolder = new GameObject(info.prefab.type.ToString()).transform;
            typeHolder.SetParent(enemyBulletsHolder);

            for (int k = 0; k < info.count; k++)
            {
                EnemyBullet bullet = Instantiate(info.prefab);
                bullet.transform.SetParent(typeHolder);
                bullet.SetActive(false);
                bulletPool.Enqueue(bullet);
            }

            bulletPools.Add(info.prefab.type, bulletPool);
        }
    }

    public void SetBullet(BulletType type, Vector3 position, Vector3 direction)
    {
        if (type == BulletType.ARROW)
        {
            EnemyArrow arrow = Instantiate(arrowPrefab);
            arrow.transform.position = position;
            arrow.transform.eulerAngles = direction;
            arrow.transform.Translate(arrow.transform.forward * 0.4f, Space.World);
            arrow.MoveBy(arrow.transform.forward * 2f, 0.3f);
        }
        else
        {
            EnemyBullet bullet = bulletPools[type].Dequeue();
            bullet.SetActive(true);
            bullet.transform.position = position;
            bullet.transform.localEulerAngles = direction;
            bulletPools[type].Enqueue(bullet);
        }
    }

    public void DestroyAll()
    {
        for (int i = 0; i < bulletPrefabs.Length; i++)
        {
            BulletPrefabInfo info = bulletPrefabs[i];
            Queue<EnemyBullet> bulletPool = bulletPools[info.prefab.type];
            
            foreach (EnemyBullet bullet in bulletPool)
            {
                if (bullet.isActiveAndEnabled)
                {
                    bullet.Damage();
                    bullet.SetActive(false);
                }
            }
        }
    }
}