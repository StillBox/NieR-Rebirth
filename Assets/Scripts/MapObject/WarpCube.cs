using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class WarpCube : MonoBehaviour
{
    public Vector4 warp = Vector4.one;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateMesh();
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = CreateMesh();
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation, transform.localScale);
    }

    private Mesh CreateMesh()
    {
        Vector3 baseNode0 = new Vector3(-0.5f, 0f, -0.5f * warp[0]);
        Vector3 baseNode1 = new Vector3(-0.5f, 0f, 0.5f * warp[1]);
        Vector3 baseNode2 = new Vector3(0.5f, 0f, -0.5f * warp[2]);
        Vector3 baseNode3 = new Vector3(0.5f, 0f, 0.5f * warp[3]);
        Vector3 halfHeight = new Vector3(0f, 0.5f, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Type 1",
            vertices = new Vector3[]
            {
                //top
                baseNode0 + halfHeight,
                baseNode1 + halfHeight,
                baseNode2 + halfHeight,
                baseNode3 + halfHeight,
                //bottom
                baseNode1 - halfHeight,
                baseNode0 - halfHeight,
                baseNode3 - halfHeight,
                baseNode2 - halfHeight,
                //port
                baseNode1 - halfHeight,
                baseNode1 + halfHeight,
                baseNode0 - halfHeight,
                baseNode0 + halfHeight,
                //stbd
                baseNode2 - halfHeight,
                baseNode2 + halfHeight,
                baseNode3 - halfHeight,
                baseNode3 + halfHeight,
                //aft
                baseNode0 - halfHeight,
                baseNode0 + halfHeight,
                baseNode2 - halfHeight,
                baseNode2 + halfHeight,
                //fore
                baseNode3 - halfHeight,
                baseNode3 + halfHeight,
                baseNode1 - halfHeight,
                baseNode1 + halfHeight,
            },
            triangles = new int[] {
                0, 1, 2, 2, 1, 3,
                4, 5, 6, 6, 5, 7,
                8, 9, 10, 10, 9, 11,
                12, 13, 14, 14, 13, 15,
                16, 17, 18, 18, 17, 19,
                20, 21, 22, 22, 21, 23
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
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