using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public class BreathingLight : MonoBehaviour
{
    [SerializeField] private Material lightMaterial;
    public float period;

    private Material material;
    private float updateTime;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        if (material == null)
        {
            GetComponent<MeshRenderer>().material = lightMaterial;
            material = GetComponent<MeshRenderer>().material;
        }
        updateTime = 0f;
    }
    
    void Update()
    {
        updateTime += Time.deltaTime;
        if (updateTime >= period)
            updateTime -= period;
        float power = 2f * (updateTime / period - 0.5f);
        power = 2f * Mathf.Pow(power, 4f);
        power = Mathf.Clamp(power, 0.01f, 3f);
        material.SetFloat("_Power", power);
    }
}