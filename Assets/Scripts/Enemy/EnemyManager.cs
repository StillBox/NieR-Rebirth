using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance = null;

    private Dictionary<int, Enemy> enemies;
    private Transform enemiesHolder;

    public int Count { get { return enemies.Count; } }
    public bool IsEmpty { get { return enemies.Count == 0; } }
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        enemies = new Dictionary<int, Enemy>();
        enemiesHolder = new GameObject("Enemies").transform;
    }
    
    public T SetEnemy<T>(T prefab, Vector3 position) where T : Enemy
    {
        T enemy = Instantiate(prefab, position, Quaternion.identity, enemiesHolder);
        enemy.SetActive(false);
        enemy.SetActive(true);
        Add(enemy);
        return enemy;
    }
    
    public void Add(Enemy enemy)
    {
        enemies.Add(enemy.GetInstanceID(), enemy);
    }

    public void Remove(int id)
    {
        enemies.Remove(id);
    }

    public void Remove(Enemy enemy)
    {
        enemies.Remove(enemy.GetInstanceID());
    }

    public void DestroyAll()
    {
        List<int> ids = new List<int>();
        foreach (int id in enemies.Keys)
            ids.Add(id);
        foreach (int id in ids)
        {
            Enemy enemy = enemies[id];
            if (enemy.gameObject.activeSelf)
                enemy.Damage(999);
            else
                Destroy(enemy.gameObject);

        }
    }
}