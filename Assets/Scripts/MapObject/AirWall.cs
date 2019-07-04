using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirWall : MonoBehaviour
{
    public static bool isBulletBlocked = false;

    private void OnDrawGizmos()
    {
        Color color = gameObject.layer == LayerMask.NameToLayer("EnemyBlock") ? Color.blue :
            isBulletBlocked ? Color.red : Color.green;
        color.a = 0.2f;
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, new Vector3(1f, 2f, 1f));
    }
}