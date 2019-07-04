using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType1 : Enemy
{
    public bool isShielded;
    [SerializeField] private GameObject fore;
    [SerializeField] private GameObject shield;

    [SerializeField] private Tracer tracer;
    [SerializeField] private NormalWeapon weapon;
    
    void Start()
    {
        fore.SetActive(!isShielded);
        shield.SetActive(isShielded);

        IsInBattle = true;

        tracer.Set(1.5f, 3f, 1.5f);
        tracer.Begin();
        
        weapon.StartWeapon(1f, 1f);
    }
    
    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireMesh(CreateColliderMesh());
    }

    public static Mesh CreateColliderMesh()
    {
        float width = WIDTH;
        float length = LENGTH;

        Vector3 portMid = new Vector3(-width / 2f, 0f, 0f);
        Vector3 portAft = new Vector3(-width / 2f, 0f, -length / 2f);
        Vector3 stbdMid = new Vector3(width / 2f, 0f, 0f);
        Vector3 stbdAft = new Vector3(width / 2f, 0f, -length / 2f);
        Vector3 fore = new Vector3(0f, 0f, length / 2f);
        Vector3 height = new Vector3(0f, 2f, 0f);

        Mesh mesh = new Mesh
        {
            name = "Enemy Type 1",
            vertices = new Vector3[]
            {
                //top
                portAft + height,
                portMid + height,
                stbdAft + height,
                stbdMid + height,
                fore + height,
                //bottom
                stbdAft,
                stbdMid,
                portAft,
                portMid,
                fore,
                //port
                portAft,
                portMid,
                portAft + height,
                portMid + height,
                //stbd
                stbdAft + height,
                stbdMid + height,
                stbdAft,
                stbdMid,
                //transom
                portAft,
                portAft + height,
                stbdAft,
                stbdAft + height,
                //port_fore
                portMid,
                fore,
                portMid + height,
                fore + height,
                //stbd_fore
                stbdMid + height,
                fore + height,
                stbdMid,
                fore
            },
            triangles = new int[] {
                0, 1, 2, 2, 1, 3, 3, 1, 4,
                5, 6, 7, 7, 6, 8, 8, 6, 9,
                10, 11, 12, 12, 11, 13,
                14, 15, 16, 16, 15, 17,
                18, 19, 20, 20, 19, 21,
                22, 23, 24, 24, 23, 25,
                26, 27, 28, 28, 27, 29
            },
            uv = new Vector2[] {
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
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
    */
}