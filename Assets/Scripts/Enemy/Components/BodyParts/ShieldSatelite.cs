using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ShieldSatelite : MonoBehaviour
{
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
        float radius = 0.4f;
        float width = 0.6f;
        float length = 0.3f;

        Vector3 fore = new Vector3(0f, 0f, radius + length);
        Vector3 port = new Vector3(-width / 2f, 0f, radius);
        Vector3 stbd = new Vector3(width / 2f, 0f, radius);
        Vector3 halfHeight = new Vector3(0f, width / 2f, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Shield 2",
            vertices = new Vector3[]
            {
                //top
                port + halfHeight,
                fore,
                stbd + halfHeight,
                //bottom
                port - halfHeight,
                fore,
                port + halfHeight,
                //port
                stbd - halfHeight,
                fore,
                port - halfHeight,
                //stbd
                stbd + halfHeight,
                fore,
                stbd - halfHeight,
                //transom
                port - halfHeight,
                port + halfHeight,
                stbd - halfHeight,
                stbd + halfHeight
            },
            triangles = new int[] {
                0, 1, 2,
                3, 4, 5,
                6, 7, 8,
                9, 10, 11,
                12, 13, 14, 14, 13, 15
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f)
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    private Mesh CreateColliderMesh()
    {
        float radius = 0.4f;
        float width = 0.6f;
        float length = 0.3f;

        Vector3 fore = new Vector3(0f, 0f, radius + length);
        Vector3 port = new Vector3(-width / 2f, 0f, radius);
        Vector3 stbd = new Vector3(width / 2f, 0f, radius);
        Vector3 halfHeight = new Vector3(0f, width / 2f, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Shield 2",
            vertices = new Vector3[]
            {
                port + halfHeight,
                stbd + halfHeight,
                stbd - halfHeight,
                port - halfHeight,
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