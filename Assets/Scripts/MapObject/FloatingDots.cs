using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDots : MonoBehaviour
{
    public const float MIN_HEIGHT = -120f;
    public const float MAX_HEIGHT = 12f;

    [SerializeField] private SpaceDot cubePrefab;
    [SerializeField] private SpaceDot spherePrefab;

    public Rect region;
    public float density;

    public Vector3 globalVelocity = new Vector3(0f, 0f, 0f);

    void Start()
    {
        SpaceDot.holder = this;

        int count = (int)(region.width * region.height * density);
        for (int i = 0; i < count; i++)
        {
            Instantiate(cubePrefab, transform);
        }
    }

    public Vector3 RandomPosition()
    {
        Vector3 position = new Vector3();
        position.x = Random.Range(region.xMin, region.xMax);
        position.y = Random.Range(MIN_HEIGHT, MAX_HEIGHT);
        position.z = Random.Range(region.yMin, region.yMax);
        return position;
    }
}