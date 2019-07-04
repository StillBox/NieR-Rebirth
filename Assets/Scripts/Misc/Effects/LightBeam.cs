using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    [SerializeField] private Material material;

    private const float SPEED = 30f;
    
    private float height = 0f;
    private float alpha = 1f;
    private float intensity = 0.5f;
    
    void Start()
    {
        transform.localScale = new Vector3(1f, height, 1f);
        StartCoroutine(Shoot());
    }
    
    IEnumerator Shoot()
    {
        height = 0f;
        alpha = 1f;
        intensity = 0.5f;
        transform.localScale = new Vector3(1f, height, 1f);
        material.color = new Color(1f, 1f, 1f, alpha);
        material.SetColor("_EmissionColor", new Color(intensity, intensity, intensity, 1f));

        while (intensity <= 1f)
        {
            yield return null;
            height += Time.deltaTime * SPEED;
            intensity += Time.deltaTime * 3f;
            transform.localScale = new Vector3(1f, height, 1f);
            material.SetColor("_EmissionColor", new Color(intensity, intensity, intensity, 1f));
        }
        while (intensity > 0.5f)
        {
            yield return null;
            height += Time.deltaTime * SPEED;
            intensity -= Time.deltaTime * 1.5f;
            alpha -= Time.deltaTime * 2f;
            transform.localScale = new Vector3(1f, height, 1f);
            material.color = new Color(1f, 1f, 1f, alpha);
            material.SetColor("_EmissionColor", new Color(intensity, intensity, intensity, 1f));
        }
        while (alpha > 0f)
        {
            yield return null;
            height += Time.deltaTime * SPEED;
            alpha -= Time.deltaTime * 2f;
            transform.localScale = new Vector3(1f, height, 1f);
            material.color = new Color(1f, 1f, 1f, alpha);
        }
        Destroy(gameObject);
    }
}
