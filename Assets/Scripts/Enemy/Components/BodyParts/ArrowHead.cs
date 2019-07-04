using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class ArrowHead : MonoBehaviour
{
    public readonly float width = 0.6f;
    public readonly float length = 0.8f;
    public readonly float height = 0.3f;

    public readonly Color colorBeg = new Color(0.555f, 0.324f, 0.277f);
    public readonly Color colorEnd = new Color(1f, 0.621f, 0.543f);
    
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
        Vector3 port = new Vector3(-width / 2f, 0f, 0f);
        Vector3 stbd = new Vector3(width / 2f, 0f, 0f);
        Vector3 top = new Vector3(0f, height, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Type 3",
            vertices = new Vector3[]
            {
                //bottom
                stbd,
                fore,
                port,
                //port
                port,
                fore,
                top,
                //stbd
                top,
                fore,
                stbd,
                //transom
                port,
                top,
                stbd
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