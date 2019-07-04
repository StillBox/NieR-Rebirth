using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class BlackBox : MonoBehaviour
{
    public float _posPeriod = 2f;
    public float _widthPeriod = 2f;

    private Material _material;
    private float _timer = 0f;
    private float _lightPos = 0f;
    private float _lightWidth = 0f;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateMesh();
        _material = GetComponent<MeshRenderer>().material;
        if (!GameManager.IsUnlocked(GameProgress.StroyCh1))
            gameObject.SetActive(false);
    }

    private void Update()
    {
        _lightPos += Time.deltaTime / _posPeriod;
        if (_lightPos >= 2f) _lightPos -= 2f;

        _timer += Time.deltaTime;
        float phase = 2 * Mathf.PI * _timer / _widthPeriod;
        _lightWidth = 0.7f - 0.3f * Mathf.Cos(phase);

        _material.SetFloat("_LightPos", _lightPos);
        _material.SetFloat("_LightWidth", _lightWidth);
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = CreateMesh();
        Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation);
    }

    private Mesh CreateMesh()
    {
        Vector3 node0 = new Vector3(-0.5f, 0f, -0.5f);
        Vector3 node1 = new Vector3(0.5f, 0f, -0.5f);
        Vector3 node2 = new Vector3(0.5f, 0f, 0.5f);
        Vector3 node3 = new Vector3(-0.5f, 0f, 0.5f);
        Vector3 halfHeight = new Vector3(0f, 0.5f, 0f);

        Mesh mesh = new Mesh
        {
            name = "BlackBox",
            vertices = new Vector3[]
            {
                //top
                node0 + halfHeight,
                node3 + halfHeight,
                node2 + halfHeight,
                node1 + halfHeight,
                //bottom
                node0 - halfHeight,
                node1 - halfHeight,
                node2 - halfHeight,
                node3 - halfHeight,
                //01
                node0 - halfHeight,
                node0 + halfHeight,
                node1 + halfHeight,
                node1 - halfHeight,
                //12
                node1 - halfHeight,
                node1 + halfHeight,
                node2 + halfHeight,
                node2 - halfHeight,
                //23
                node2 - halfHeight,
                node2 + halfHeight,
                node3 + halfHeight,
                node3 - halfHeight,
                //30
                node3 - halfHeight,
                node3 + halfHeight,
                node0 + halfHeight,
                node0 - halfHeight
            },
            triangles = new int[] {
                0, 1, 2, 0, 2, 3,
                4, 5, 6, 4, 6, 7,
                8, 9, 10, 8, 10, 11,
                12, 13, 14, 12, 14, 15,
                16, 17, 18, 16, 18, 19,
                20, 21, 22, 20, 22, 23
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f),
                new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(2f, 1f), new Vector2(2f, 0f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f),
                new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(2f, 1f), new Vector2(2f, 0f)
            }
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}
