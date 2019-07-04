using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    public Enemy[] enemies;

    public int Count
    {
        get { return enemies.Length; }
    }

    public Enemy Get(int index)
    {
        if (index < 0 || index >= Count) return null;
        return enemies[index];
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            StartCoroutine(Activate());
        }
        else
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.SetActive(value);
            }
        }
    }

    public bool IsClear
    {
        get
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                    return false;
            }
            return true;
        }
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (Enemy enemy in enemies)
        {
            Vector3 playerDir = Player.instance.transform.position - enemy.transform.position;
            if (playerDir.magnitude <= 2f)
            {
                playerDir = playerDir.normalized * 2f;
                enemy.transform.position = Player.instance.transform.position - playerDir;
            }
            enemy.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
