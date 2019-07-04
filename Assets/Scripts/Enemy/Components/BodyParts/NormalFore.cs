using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class NormalFore : MonoBehaviour
{
    private readonly float width = 0.64f;
    private readonly float length = 0.45f;
    private readonly float height = 0.6f;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateMesh();
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
            name = "Enemy Type 1",
            vertices = new Vector3[]
            {
                //top_fore
                portMid + halfHeight,
                fore,
                stbdMid + halfHeight,
                //bottom_fore
                portMid - halfHeight,
                fore,
                portMid + halfHeight,
                //port_fore
                stbdMid - halfHeight,
                fore,
                portMid - halfHeight,
                //stbd_fore
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
}