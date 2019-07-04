using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ShieldFore : MonoBehaviour
{
    private readonly float width = 0.64f;
    private readonly float length = 0.45f;
    private readonly float height = 0.6f;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateMesh();
        GetComponent<MeshCollider>().sharedMesh = CreateColliderMesh();
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = CreateMesh();
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
    }

    private Mesh CreateMesh()
    {
        Vector3 fore = new Vector3(0f, 0f, length);
        Vector3 portMid = new Vector3(-width / 2f, 0f, 0f);
        Vector3 stbdMid = new Vector3(width / 2f, 0f, 0f);
        Vector3 halfHeight = new Vector3(0f, height / 2f, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Shield 1",
            vertices = new Vector3[]
            {
                //top
                portMid + halfHeight,
                fore,
                stbdMid + halfHeight,
                //bottom
                portMid - halfHeight,
                fore,
                portMid + halfHeight,
                //port
                stbdMid - halfHeight,
                fore,
                portMid - halfHeight,
                //stbd
                stbdMid + halfHeight,
                fore,
                stbdMid - halfHeight,
            },
            triangles = new int[] {
                0, 1, 2,
                3, 4, 5,
                6, 7, 8,
                9, 10, 11
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f)
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    private Mesh CreateColliderMesh()
    {
        float width = 0.7f;
        float height = 0.6f;
        float length = 1.2f;

        Vector3 fore = new Vector3(0f, 0f, length / 2f);
        Vector3 portMid = new Vector3(-width / 2f, 0f, 0f);
        Vector3 stbdMid = new Vector3(width / 2f, 0f, 0f);
        Vector3 halfHeight = new Vector3(0f, height / 2f, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Shield 1",
            vertices = new Vector3[]
            {
                portMid + halfHeight,
                stbdMid + halfHeight,
                stbdMid - halfHeight,
                portMid - halfHeight,
                fore
            },
            triangles = new int[] {
                0, 4, 1,
                1, 4, 2,
                2, 4, 3,
                3, 4, 0,
                0, 1, 3, 3, 1, 2
            },
        };
        return mesh;
    }
}
