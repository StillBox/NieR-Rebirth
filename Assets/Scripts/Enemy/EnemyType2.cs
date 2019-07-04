using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType2 : Enemy
{
    public enum FireMode
    {
        NORMAL,
        ARROW,
    }
    public FireMode fireMode;
    public bool isShielded;
    
    [SerializeField] private GameObject[] shields;
    [SerializeField] private RingWeapon ringWeapon;

    void Start()
    {
        IsInBattle = true;

        foreach (GameObject shield in shields)
        {
            shield.SetActive(isShielded);
        }

        switch (fireMode)
        {
            case FireMode.NORMAL:
                ringWeapon.SetRotation(Random.Range(-90f, 90f), 60f);
                ringWeapon.StartWeapon(4, 1f, 1f);
                break;
            case FireMode.ARROW:
                ringWeapon.SetType(BulletType.ARROW, false);
                ringWeapon.SetRotation(90f, 60f);
                ringWeapon.StartWeapon(2, 4f, 1f);
                break;
        }
    }
    
    /*
    void Fire()
    {
        switch (fireMode)
        {
            case FireMode.NORMAL:
                FireCross();
                break;
            case FireMode.ARROW:
                FireArrow();
                break;
        }
    }

    void FireCross()
    {
        Vector3 position = transform.position;
        Vector3 euler = transform.eulerAngles;
        for (int i = 0; i < 4; i++)
        {
            euler.y += 90f;
            EnemyBulletManager.instance.SetBullet(EnemyBullet.RandomBullet(), position, euler);
        }
    }

    void FireArrow()
    {
        Vector3 position = transform.position;
        Vector3 euler = transform.eulerAngles;
        for (int i = 0; i < 2; i++)
        {
            euler.y += 180f;
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.position = position;
            arrow.transform.eulerAngles = euler;
            arrow.transform.Translate(arrow.transform.forward * 0.5f, Space.World);
        }
    }
    */
}