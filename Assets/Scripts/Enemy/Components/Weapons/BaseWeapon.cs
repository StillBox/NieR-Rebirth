using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public Space space = Space.Self;

    //For Bullet Type and Alter Mode
    public BulletType baseType = BulletType.SOFT;
    public bool isTypeAltered = true;

    public void SetType(BulletType type, bool alter)
    {
        baseType = type;
        isTypeAltered = alter;
    }

    //For Rotation and angle
    protected float angleBeg = 0f;
    protected float rotSpeed = 0f;

    protected bool isFloatAngle = false;
    protected float minFloatAngle = 0f;
    protected float maxFloatAngle = 0f;

    protected Angle angle = 0f;
    protected Angle BaseAngle
    {
        get
        {
            Angle baseAngle = angle;
            baseAngle += Random.Range(minFloatAngle, maxFloatAngle);
            return baseAngle;
        }
    }

    public void SetRotation(float beg, float speed)
    {
        angleBeg = beg;
        rotSpeed = speed;
    }

    public void SetFloatAngle(bool value, float min, float max)
    {
        isFloatAngle = value;
        minFloatAngle = min;
        maxFloatAngle = max;
    }

    //For gap, etc.
    protected bool isFloatGap = false;
    protected float minGapRate = 1f;
    protected float maxGapRate = 1f;

    protected float gap = 1f;
    protected float timer = 0f;

    protected float BaseGap
    {
        get
        {
            float baseGap = gap;
            if (isFloatGap) baseGap *= Random.Range(minGapRate, maxGapRate);
            else baseGap *= minGapRate;
            return baseGap;
        }
    }

    public void SetGap(float gap)
    {
        this.gap = gap;
        while (timer < -gap)
        {
            timer += gap;
        }
    }

    public void SetFloatGap(bool value, float min, float max)
    {
        isFloatGap = value;
        minGapRate = min;
        maxGapRate = max;
        while (timer < -BaseGap)
        {
            timer += BaseGap;
        }
    }

    //For States
    protected bool isExcuting = false;
    public bool IsExcuting
    {
        get { return isExcuting; }
    }

    protected void StartWeapon()
    {
        isExcuting = true;
        StartCoroutine(Rotate());
    }

    public void StopWeapon()
    {
        isExcuting = false;
    }

    public void Fire(float angle, BulletType type)
    {
        Vector3 euler = new Vector3(0f, angle, 0f);
        if (space == Space.Self) euler += transform.eulerAngles;
        EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
    }

    public void Fire(float angle, float delta, int count, BulletType proto, bool alter)
    {
        Vector3 beg = new Vector3(0f, angle, 0f);
        if (space == Space.Self) beg += transform.eulerAngles;
        BulletType type = proto;
        for (int i = 0; i < count; i++)
        {
            Vector3 euler = beg + new Vector3(0f, delta * i, 0f);
            if (alter) type = AlterType(type);
            EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
        }
    }

    public void Fire(float angle, float delta, int count, BulletType proto, int alterIndex)
    {
        Vector3 beg = new Vector3(0f, angle, 0f);
        if (space == Space.Self) beg += transform.eulerAngles;
        for (int i = 0; i < count; i++)
        {
            Vector3 euler = beg + new Vector3(0f, delta * i, 0f);
            BulletType type = i == alterIndex ? AlterType(proto) : proto;
            EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
        }
    }

    protected virtual IEnumerator Rotate()
    {
        angle = angleBeg;
        while (true)
        {
            yield return null;
            angle += rotSpeed * Time.deltaTime;
        }
    }

    protected BulletType AlterType(BulletType type)
    {
        if (type == BulletType.ARROW)
            return BulletType.ARROW;
        else if (type == BulletType.HARD)
            return BulletType.SOFT;
        else
            return BulletType.HARD;
    }
}