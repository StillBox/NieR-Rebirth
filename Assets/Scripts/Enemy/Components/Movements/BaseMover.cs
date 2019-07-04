using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMover : MonoBehaviour
{
    public bool defaultLookAtPlayer = true;
    public bool keepLookingAtPlayer = true;

    protected Enemy parent = null;

    //For Move Speed and Rotation
    protected bool isSpeedSet = false;
    protected float begSpeed = 0f;
    protected float maxSpeed = 0f;
    protected float accel = 0f;

    protected bool isRotSet = false;
    protected float rot = 0f;

    //For States
    protected float speed = 0f;
    protected bool isExcuting = false;

    private void Awake()
    {
        parent = GetComponent<Enemy>();
    }

    public void Set(float maxSpeed, float accel, float rot, float begSpeed = 0f)
    {
        isSpeedSet = true;
        this.begSpeed = begSpeed;
        this.maxSpeed = maxSpeed;
        this.accel = accel;
        isRotSet = true;
        this.rot = rot;
    }

    public void SetSpeed(float maxSpeed, float accel, float begSpeed = 0f)
    {
        isSpeedSet = true;
        this.begSpeed = begSpeed;
        this.maxSpeed = maxSpeed;
        this.accel = accel;
    }

    public void SetRotRate(float rot)
    {
        isRotSet = true;
        this.rot = rot;
    }

    public virtual void Begin(float wait = 0f)
    {
        speed = begSpeed;
        isExcuting = true;
        StartCoroutine(Move(wait));
        if (keepLookingAtPlayer)
            StartCoroutine(Turning(wait));
    }

    public void Stop(bool reset = false)
    {
        if (reset) speed = begSpeed;
        isExcuting = false;
    }

    public void Continue()
    {
        isExcuting = true;
        StartCoroutine(Move());
    }

    protected virtual IEnumerator Move(float wait = 0f)
    {
        yield break;
    }

    protected virtual IEnumerator Turning(float wait = 0f)
    {
        if (defaultLookAtPlayer)
            transform.LookAt(Player.instance.transform);

        yield return new WaitForSeconds(wait);

        while (true)
        {
            Vector3 dir = Player.instance.transform.position - transform.position;
            dir.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rot * Time.deltaTime);
            yield return null;
        }
    }
}
