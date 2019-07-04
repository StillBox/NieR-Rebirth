using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossEx1 : Enemy
{
    [SerializeField] protected EnemySateliteEx1 satelite1;
    [SerializeField] protected EnemySateliteEx1 satelite2;

    public int Phase { get; set; }
    
    public override bool IsInBattle
    {
        get
        {
            return isInBattle;
        }
        set
        {
            isInBattle = value;
            if (satelite1 != null) satelite1.IsInBattle = value;
            if (satelite2 != null) satelite2.IsInBattle = value;
        }
    }

    protected int sateliteCount = 2;
    protected bool isMovable = true;
    protected bool isMainWeaponOn = false;
    protected bool isSubWeaponOn = false;
    protected float speed = 0f;

    //Functions for movement

    public void MoveIn()
    {
        gameObject.SetActive(true);
        armor.ResetArmor();
        StartCoroutine(MoveIn(0f, 10f, 3f));
    }

    public void MoveForward()
    {
        IsInBattle = false;
        StartCoroutine(MoveOut(24f, 10f, 2f));
    }

    public void MoveBackward()
    {
        IsInBattle = false;
        StartCoroutine(MoveOut(-24f, 10f, 2f));
    }

    public void MoveRound()
    {
        if (Phase == 5)
            StartCoroutine(MoveAround(-6f, 9f, 3f, 0.5f));
        else
            StartCoroutine(MoveAround(-12f, 12f, 4f, 1f));
    }

    protected virtual IEnumerator MoveIn(float target, float maxSpeed, float minSpeed = 0f)
    {
        if (satelite1 != null) satelite1.gameObject.SetActive(Phase >= 3);
        if (satelite2 != null) satelite2.gameObject.SetActive(Phase >= 3);

        float origin = PositionZ;
        float dir = target - origin;
        while (dir * (target - PositionZ) > 0)
        {
            speed = (target - PositionZ) * 2f;
            speed = Mathf.Sign(dir) * Mathf.Clamp(Mathf.Abs(speed), minSpeed, maxSpeed);
            controller.Move(new Vector3(0f, 0f, speed * Time.deltaTime));
            yield return null;
        }
        speed = target - origin;
        PositionZ = target;

        if (Phase >= 3)
        {
            if (satelite1 != null) satelite1.Launch(1f);
            if (satelite2 != null) satelite2.Launch(-1f);
        }
        yield return new WaitForSeconds(1.5f);

        IsInBattle = true;
        StartFire();
        MoveRound();
    }

    protected virtual IEnumerator MoveOut(float target, float maxSpeed, float minSpeed = 0f)
    {
        if (Phase == 3 || Phase == 4)
        {
            if (satelite1 != null) satelite1.Land(1f);
            if (satelite2 != null) satelite2.Land(-1f);
        }
        StopMainWeapon();
        StopSubWeapon();

        yield return new WaitForSeconds(1f);
        StartBurstWeapon();

        yield return new WaitForSeconds(3f);
        float origin = PositionZ;
        float dir = target - origin;
        while (dir * (target - PositionZ) > 0)
        {
            speed = (PositionZ - origin) * 2f;
            speed = Mathf.Sign(dir) * Mathf.Clamp(Mathf.Abs(speed), minSpeed, maxSpeed);
            controller.Move(new Vector3(0f, 0f, speed * Time.deltaTime));
            yield return null;
        }
        PositionZ = target;
        SetActive(false);
    }

    protected virtual IEnumerator MoveAround(float min, float max, float maxSpeed, float minSpeed = 0f)
    {
        while (IsInBattle)
        {
            if (isMovable)
            {
                if (speed > 0)
                {
                    speed = Mathf.Min(max - PositionZ, PositionZ - min) * 2f;
                    speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                    controller.Move(new Vector3(0f, 0f, speed * Time.deltaTime));
                    if (PositionZ > max)
                    {
                        speed = -speed;
                        PositionZ = max;
                    }
                }
                else
                {
                    speed = Mathf.Max(min - PositionZ, PositionZ - max) * 2f;
                    speed = Mathf.Clamp(speed, -maxSpeed, -minSpeed);
                    controller.Move(new Vector3(0f, 0f, speed * Time.deltaTime));
                    if (PositionZ < min)
                    {
                        speed = -speed;
                        PositionZ = min;
                    }
                }
            }
            yield return null;
        }
    }

    //For attack

    public virtual void StartFire()
    {
        switch (Phase)
        {
            case 1:
            case 2:
                StartMainWeapon();
                break;
            case 3:
            case 4:
                StartSubWeapon();
                break;
            case 5:
                if (sateliteCount == 0)
                    StartMainWeapon();
                else
                    StartSubWeapon();
                break;
        }
    }

    public virtual void StartMainWeapon() { }
    public virtual void StartSubWeapon() { }
    public virtual void StartBurstWeapon() { }
    public virtual void StopMainWeapon() { isMainWeaponOn = false; }
    public virtual void StopSubWeapon() { isSubWeaponOn = false; }

    protected virtual IEnumerator MainWeapon(float gap) { yield break; }
    protected virtual IEnumerator SubWeapon(float gap) { yield break; }
    protected virtual IEnumerator BurstWeapon(float gap) { yield break; }
    
    //For damage and death

    public void OnSateliteDestroyed()
    {
        sateliteCount--;
        if (sateliteCount == 0)
        {
            StopSubWeapon();
            StartMainWeapon();
        }
    }
    
    protected override void Explode()
    {
        if (satelite1 != null) satelite1.Damage(999);
        if (satelite2 != null) satelite2.Damage(999);
        base.Explode();
    }
}