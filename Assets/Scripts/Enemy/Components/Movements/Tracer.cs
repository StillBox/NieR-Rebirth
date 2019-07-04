using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : BaseMover
{
    private Rect boundary = Rect.zero;
    private bool isXBound = false;
    private bool isZBound = false;
    
    public void SetXBoundary(float min, float max)
    {
        isXBound = true;
        boundary.xMin = min;
        boundary.xMax = max;
    }

    public void SetZBoundary(float min, float max)
    {
        isZBound = true;
        boundary.yMin = min;
        boundary.yMax = max;
    }

    public override void Begin(float wait = 0)
    {
        if (!isSpeedSet) SetSpeed(1.5f, 1.5f);
        if (!isRotSet) SetRotRate(3f);
        base.Begin(wait);
    }

    protected override IEnumerator Move(float wait = 0)
    {
        yield return new WaitForSeconds(wait);

        while (isExcuting)
        {
            Vector3 velocity = transform.forward * speed;
            if (parent.controller != null)
            {
                if (!parent.controller.isGrounded) velocity.y = -9.8f;
                parent.controller.Move(velocity * Time.deltaTime);
            }
            else
            {
                transform.Translate(velocity * Time.deltaTime, Space.World);
            }
            if (isXBound) parent.PositionX = Mathf.Clamp(parent.PositionX, boundary.xMin, boundary.xMax);
            if (isZBound) parent.PositionZ = Mathf.Clamp(parent.PositionZ, boundary.yMin, boundary.yMax);

            yield return null;
            speed = Mathf.Min(maxSpeed, speed + accel * Time.deltaTime);
        }
    }
}
