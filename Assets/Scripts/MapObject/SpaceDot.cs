using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceDot : MonoBehaviour
{
    public static FloatingDots holder;

    private float speed;
    private Vector3 direction;

    void Start()
    {
        float scale = Random.Range(0.2f, 0.4f);
        transform.localScale = Vector3.one * scale;
        transform.position = holder.RandomPosition();

        speed = Random.Range(0.2f, 0.3f);
        float angle = Random.Range(0, Mathf.PI * 2);
        direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
    }
    
    void Update()
    {
        transform.Translate((direction * speed + holder.globalVelocity) * Time.deltaTime);

        Vector3 position = transform.position;

        if (position.x < holder.region.xMin)
        {
            position.x = holder.region.xMax;
            transform.position = position;
        }
        else if (position.x > holder.region.xMax)
        {
            position.x = holder.region.xMin;
            transform.position = position;
        }

        if (position.y < FloatingDots.MIN_HEIGHT)
        {
            position.y = FloatingDots.MAX_HEIGHT;
            transform.position = position;
        }
        else if (position.y > FloatingDots.MAX_HEIGHT)
        {
            position.y = FloatingDots.MIN_HEIGHT;
            transform.position = position;
        }

        if (position.z < holder.region.yMin)
        {
            position.z = holder.region.yMax;
            transform.position = position;
        }
        else if (position.z > holder.region.yMax)
        {
            position.z = holder.region.yMin;
            transform.position = position;
        }
    }
}