using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class SquareHalo : MonoBehaviour
{
    public float width = 0.1f;

    private float fadeRate = 0.1f;
    private float size = 0.5f;
    private float alpha = 0f;

    private Mesh mesh;
    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateMesh(size);
        material.SetFloat("_Alpha", alpha);
    }

    public void OnPlayerIn()
    {
        fadeRate = 5f;
        StartCoroutine(Diffuse());
    }

    public void OnPlayerOut()
    {
        fadeRate = 15f;
    }

    IEnumerator Diffuse()
    {
        size = 0.5f;
        alpha = 2.5f;
        while (alpha > 0f)
        {
            GetComponent<MeshFilter>().mesh = CreateMesh(size);
            material.SetFloat("_Alpha", Mathf.Min(1f, alpha));
            yield return null;
            size += Time.deltaTime * 2f;
            alpha -= Time.deltaTime * fadeRate;
        }
        size = 0.5f;
        alpha = 0f;
        GetComponent<MeshFilter>().mesh = CreateMesh(size);
        material.SetFloat("_Alpha", alpha);
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = CreateMesh(1f);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
    }

    private Mesh CreateMesh(float size)
    {
        Mesh mesh = new Mesh
        {
            name = "Fore Mesh",
            vertices = new Vector3[]
            {
                //s
                new Vector3(-size, 0f, -size),
                new Vector3(-size, 0f, -size + width),
                new Vector3(size, 0f, -size),
                new Vector3(size, 0f, -size + width),
                //n
                new Vector3(-size, 0f, size - width),
                new Vector3(-size, 0f, size),
                new Vector3(size, 0f, size - width),
                new Vector3(size, 0f, size),
                //w
                new Vector3(-size, 0f, -size + width),
                new Vector3(-size, 0f, size - width),
                new Vector3(-size + width, 0f, -size + width),
                new Vector3(-size + width, 0f, size - width),
                //e
                new Vector3(size - width, 0f, -size + width),
                new Vector3(size - width, 0f, size - width),
                new Vector3(size, 0f, -size + width),
                new Vector3(size, 0f, size - width),
            },
            triangles = new int[] {
                0, 1, 2, 2, 1, 3,
                4, 5, 6, 6, 5, 7,
                8, 9, 10, 10, 9, 11,
                12, 13, 14, 14, 13, 15,
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f),
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}
