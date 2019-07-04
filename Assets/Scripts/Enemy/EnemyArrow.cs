using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : Enemy
{
    [SerializeField] private Tracer tracer;
    [SerializeField] private TimeDetonator detonator;
    
    void Start()
    {
        IsInBattle = true;

        tracer.Set(5f, 10f, 6f, 5f);
        tracer.Begin(0.3f);
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

    /*
    private void OnDrawGizmosSelected()
    {
        Mesh mesh = CreateColliderMesh();
        Gizmos.color = Color.green;
        Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation);
    }

    private Mesh CreateColliderMesh()
    {
        float width = 0.6f;
        float height = 1f;
        float length = 0.8f;

        Vector3 fore = new Vector3(0f, 0f, length);
        Vector3 port = new Vector3(-width / 2f, 0f, 0f);
        Vector3 stbd = new Vector3(width / 2f, 0f, 0f);
        Vector3 h = new Vector3(0f, height, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Type 3",
            vertices = new Vector3[]
            {
                //bottom
                stbd,
                fore,
                port,
                //top
                port + h,
                fore + h,
                stbd + h,
                //port
                port,
                fore,
                port + h,
                fore + h,
                //stbd
                stbd + h,
                fore + h,
                stbd,
                fore,
                //transom
                port,
                port + h,
                stbd,
                stbd + h
            },
            triangles = new int[] {
                0, 1, 2,
                3, 4, 5,
                6, 7, 8, 8, 7, 9,
                10, 11, 12, 12, 11, 13,
                14, 15, 16, 16, 15, 17
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }
    */
}