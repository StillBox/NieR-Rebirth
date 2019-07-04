using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinter : BaseMover
{
    private float gap = 0.6f;
    private bool isColliderHit = false;

    public void SetGap(float gap)
    {
        this.gap = gap;
    }

    public override void Begin(float wait = 0)
    {
        if (!isSpeedSet) SetSpeed(10f, 20f);
        if (!isRotSet) SetRotRate(10f);
        base.Begin(wait);
    }

    protected override IEnumerator Move(float wait = 0)
    {
        yield return new WaitForSeconds(wait + gap);

        while (isExcuting)
        {
            Vector3 direction = transform.forward;
            while (!isColliderHit)
            {
                Vector3 velocity = direction * speed;
                if (parent.controller != null)
                {
                    if (!parent.controller.isGrounded) velocity.y = -9.8f;
                    parent.controller.Move(velocity * Time.deltaTime);
                }
                else
                {
                    transform.Translate(velocity * Time.deltaTime, Space.World);
                }

                yield return null;
                speed = Mathf.Min(maxSpeed, speed + accel * Time.deltaTime);
            }
            yield return new WaitForSeconds(gap);
            isColliderHit = false;
        }
    }

    protected override IEnumerator Turning(float wait)
    {
        if (defaultLookAtPlayer)
            transform.LookAt(Player.instance.transform);

        yield return new WaitForSeconds(wait);

        while (true)
        {
            if (isColliderHit)
            {
                Vector3 dir = Player.instance.transform.position - transform.position;
                dir.y = 0f;
                Quaternion rotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rot * Time.deltaTime);
            }
            yield return null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Floor") &&
            !hit.collider.CompareTag("Player"))
        {
            isColliderHit = true;
        }
    }
}
