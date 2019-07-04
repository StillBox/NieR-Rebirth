using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySatelite : MonoBehaviour
{
    public float speed = 60f;
    
    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f);
    }
}