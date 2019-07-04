using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : BaseMover
{
    private Vector3 direction;
    
    public override void Begin(float wait = 0)
    {
        if (!isSpeedSet) SetSpeed(4f, 4f);
        if (!isRotSet) SetRotRate(4f);
        base.Begin(wait);
    }

    protected override IEnumerator Move(float wait = 0)
    {
        yield return new WaitForSeconds(wait);

        float angle = Random.Range(0, 2) == 0 ? 30f : 60f;
        angle += Random.Range(0, 2) == 0 ? 90f : 0f;
        angle *= (Random.Range(0, 2) == 0 ? -1f : 1f) * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        while (isExcuting)
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
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Floor") &&
            !hit.collider.CompareTag("Player"))
        {
            Vector3 normal = hit.normal;
            direction = Vector3.Reflect(direction, normal);
        }
    }
}
