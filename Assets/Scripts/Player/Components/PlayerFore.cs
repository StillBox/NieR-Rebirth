using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class PlayerFore : MonoBehaviour
{
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
        float gap = 0.01f;
        float coreSize = 0.1f;
        float scale = 2.33f;
        float angleFore = Mathf.PI * 20.5f / 180f;
        float angleCenter = Mathf.PI * 27f / 180f;

        Vector3 aft = new Vector3(0f, 0f, gap / Mathf.Cos(Mathf.PI / 6f));
        Vector3 port = aft + new Vector3(-scale * coreSize * Mathf.Cos(angleCenter), 0f, scale * coreSize * Mathf.Sin(angleCenter));
        Vector3 stbd = aft + new Vector3(scale * coreSize * Mathf.Cos(angleCenter), 0f, scale * coreSize * Mathf.Sin(angleCenter));
        Vector3 fore = aft + new Vector3(0f, 0f, scale * coreSize * (Mathf.Sin(angleCenter) + Mathf.Cos(angleCenter) / Mathf.Tan(angleFore)));
        Vector3 halfHeight = new Vector3(0f, 0.05f, 0f);

        Mesh mesh = new Mesh
        {
            name = "Fore Mesh",
            vertices = new Vector3[]
            {
                //top
                aft + halfHeight,
                port + halfHeight,
                stbd + halfHeight,
                fore + halfHeight,
                //bottom
                aft - halfHeight,
                stbd - halfHeight,
                port - halfHeight,
                fore - halfHeight,
                //port_fore
                port - halfHeight,
                fore - halfHeight,
                port + halfHeight,
                fore + halfHeight,
                //port_aft
                aft - halfHeight,
                port - halfHeight,
                aft + halfHeight,
                port + halfHeight,
                //stbd_fore
                stbd - halfHeight,
                stbd + halfHeight,
                fore - halfHeight,
                fore + halfHeight,
                //stbd_aft
                aft - halfHeight,
                aft + halfHeight,
                stbd - halfHeight,
                stbd + halfHeight
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