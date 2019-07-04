using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour
{
    private const float SPEED = 0.2f;
    private float offset;
    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        offset = 0f;
    }
    
    void Update()
    {
        offset -= SPEED * Time.deltaTime;
        while (offset <= 0f)
        {
            offset += 1f;
        }
        material.SetFloat("_Offset", offset);
    }
}