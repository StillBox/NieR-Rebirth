using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrowEx2 : Enemy
{
    [SerializeField] private Tracer tracer;
    [SerializeField] private TimeDetonator detonator;
    
    void Start()
    {
        IsInBattle = true;
        tracer.Set(5f, 10f, 6f);
        tracer.Begin(0.5f);
        detonator.Set(6f);
    }
    
    override protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Player"))
        {
            CharacterController controller = hit.collider.GetComponent<CharacterController>();
            if (controller == null) return;

            Vector3 pushDir = new Vector3(hit.normal.x, 0f, hit.normal.z);
            Vector3 moveDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            float pushLength = hit.moveLength * Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(moveDir, pushDir));
            controller.Move(pushDir * pushLength);
            Player.instance.arrow = GetComponent<Enemy>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.arrow = null;
        }
    }
}