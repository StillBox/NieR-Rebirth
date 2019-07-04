using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class CuboidBody : MonoBehaviour
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
        Vector3 portMid = new Vector3(-width / 2f, 0f, 0f);
        Vector3 portAft = new Vector3(-width / 2f, 0f, -length);
        Vector3 stbdMid = new Vector3(width / 2f, 0f, 0f);
        Vector3 stbdAft = new Vector3(width / 2f, 0f, -length);
        Vector3 halfHeight = new Vector3(0f, height / 2f, 0f);
            
        Mesh mesh = new Mesh
        {
            name = "Enemy Type 1",
            vertices = new Vector3[]
            {
                //top
                portAft + halfHeight,
                portMid + halfHeight,
                stbdAft + halfHeight,
                stbdMid + halfHeight,
                //bottom
                stbdAft - halfHeight,
                stbdMid - halfHeight,
                portAft - halfHeight,
                portMid - halfHeight,
                //port
                portAft - halfHeight,
                portMid - halfHeight,
                portAft + halfHeight,
                portMid + halfHeight,
                //stbd
                stbdAft + halfHeight,
                stbdMid + halfHeight,
                stbdAft - halfHeight,
                stbdMid - halfHeight,
                //transom
                portAft - halfHeight,
                portAft + halfHeight,
                stbdAft - halfHeight,
                stbdAft + halfHeight
            },
            triangles = new int[] {
                0, 1, 2, 2, 1, 3,
                4, 5, 6, 6, 5, 7,
                8, 9, 10, 10, 9, 11,
                12, 13, 14, 14, 13, 15,
                16, 17, 18, 18, 17, 19
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f)
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}