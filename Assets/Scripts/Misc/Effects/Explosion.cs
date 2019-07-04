using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] explodes;
    public float explodeTime = 0.5f;

    private Material[] materials;
    
    private void Awake()
    {
        materials = new Material[2];
        for (int i = 0; i < 2; i++)
        {
            materials[i] = explodes[i].GetComponent<Renderer>().material;
        }
    }

    void Start()
    {
        Explode();
    }
    
    public void Explode()
    {
        explodes[0].Play();
        explodes[1].Play();
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float time = 0f;
        while (time < 0.2f * explodeTime)
        {
            SetAlpha(2.5f * time / explodeTime);
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(0.5f);
        while (time < 0.6f * explodeTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time < 1f * explodeTime)
        {
            SetAlpha(1.25f * (1f - time / explodeTime));
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(0f);
        Destroy(gameObject);
    }

    private void SetAlpha(float value)
    {
        foreach (Material mat in materials)
        {
            materials[0].SetFloat("_Alpha", value);
            materials[1].SetFloat("_Alpha", value);
        }
    }
}
