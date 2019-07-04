using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalTracer : BaseMover
{
    public override void Begin(float wait = 0)
    {
        if (!isSpeedSet)
        {
            maxSpeed = 1.5f;
            accel = 1.5f;
        }
        if (!isRotSet)
        {
            rot = 1.5f;
        }
        base.Begin(wait);
    }

    protected override IEnumerator Move(float wait = 0)
    {
        yield return new WaitForSeconds(wait);

        while (isExcuting)
        {
            float dir = Player.instance.PositionZ - transform.GetPositionZ();
            Vector3 velocity = Mathf.Sign(dir) * Vector3.forward * speed;            
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
            speed = Mathf.Min(Mathf.Abs(dir) / 2f, speed);
        }
    }
}
