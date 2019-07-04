using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class ArrowTail : MonoBehaviour
{
    private Vector3[] worldPoints = new Vector3[6];
    private Vector3[] localPoints = new Vector3[6];

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            worldPoints[i] = transform.position;
            localPoints[i] = Vector3.zero;
        }
        GetComponent<MeshFilter>().mesh = CreateMesh();
    }

    private void LateUpdate()
    {
        for (int i = 5; i > 0; i--)
        {
            worldPoints[i] = Vector3.Lerp(worldPoints[i], worldPoints[i - 1], Time.deltaTime / 0.1f);
        }
        worldPoints[0] = transform.position;

        for (int i = 0; i < 6; i++)
        {
            localPoints[i] = worldPoints[i] - transform.position;
        }
        transform.eulerAngles = Vector3.zero;

        GetComponent<MeshFilter>().mesh = CreateMesh();
    }

    /*
    private void OnDrawGizmos()
    {
        Mesh mesh = CreateMesh();
        Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation);
    }
    */

    private Mesh CreateMesh()
    {
        float width = 0.6f;

        Vector3[] port = new Vector3[6];
        Vector3[] stbd = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            Vector3 fore = i == 0 ? localPoints[0] - localPoints[1] : localPoints[i - 1] - localPoints[i];
            Vector3 aft = i == 5 ? localPoints[4] - localPoints[5] : localPoints[i] - localPoints[i + 1];
            Vector3 ave = fore + aft;
            Vector3 portVec = new Vector3(-ave.z, 0f, ave.x).normalized;
            Vector3 stbdVec = new Vector3(ave.z, 0f, -ave.x).normalized;
            port[i] = localPoints[i] + portVec * width / 2f;
            stbd[i] = localPoints[i] + stbdVec * width / 2f;
        }

        Mesh mesh = new Mesh
        {
            name = "Tail",
            vertices = new Vector3[]
            {
                port[0],
                stbd[0],
                port[1],
                stbd[1],
                port[1],
                stbd[1],
                port[2],
                stbd[2],
                port[2],
                stbd[2],
                port[3],
                stbd[3],
                port[3],
                stbd[3],
                port[4],
                stbd[4],
                port[4],
                stbd[4],
                port[5],
                stbd[5],
            },
            triangles = new int[] {
                0, 1, 2,
                2, 1, 3,
                4, 5, 6,
                6, 5, 7,
                8, 9, 10,
                10, 9, 11,
                12, 13, 14,
                14, 13, 15,
                16, 17, 18,
                18, 17, 19
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f)
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}