using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchLight : MonoBehaviour
{
    [SerializeField] private Transform beam;
    [SerializeField] private GameObject halo;

    private Transform transHalo;
    private Material material;

    private void Awake()
    {
        transHalo = halo.GetComponent<Transform>();
        material = halo.GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        beam.localScale = new Vector3(1f, 0f, 1f);
        transHalo.localScale = new Vector3(0f, 0f, 1f);
        material.SetFloat("_Fade", 0f);
        StartCoroutine(Shoot());
        StartCoroutine(ShowHalo());
    }

    IEnumerator Shoot()
    {
        float time = 0f;
        while (time < 0.2f)
        {
            beam.transform.localScale = new Vector3(1f, time * 100f, 1f);
            yield return null;
            time += Time.deltaTime;
        }
        beam.transform.localScale = new Vector3(1f, 40f, 1f);
        while (time < 0.35f)
        {
            float scale = (0.35f - time) * 4f + 0.4f; 
            beam.transform.localScale = new Vector3(scale, 40f, scale);
            yield return null;
            time += Time.deltaTime;
        }
        beam.transform.localScale = new Vector3(0.4f, 40f, 0.4f);
        while (time < 0.55f)
        {
            float scale = (0.55f - time) * 200f;
            beam.transform.localScale = new Vector3(0.4f, scale, 0.4f);
            yield return null;
            time += Time.deltaTime;
        }
        beam.transform.localScale = new Vector3(0.4f, 0f, 0.4f);
        Destroy(gameObject);
    }

    IEnumerator ShowHalo()
    {
        float time = 0;
        material.SetFloat("_Fade", 0.15f);
        while (time <0.15f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time < 0.25f)
        {
            float scale = (time - 0.15f) * 18f;
            transHalo.localScale = new Vector3(scale, scale, 1f);
            yield return null;
            time += Time.deltaTime;
        }
        transHalo.localScale = new Vector3(1.8f, 1.8f, 1f);
        while (time < 0.4f)
        {
            float fade = 0.15f + (time - 0.25f) * 3f;
            material.SetFloat("_Fade", fade);
            yield return null;
            time += Time.deltaTime;
        }
        material.SetFloat("_Fade", 0.6f);
        while (time < 0.5f)
        {
            float fade = 0.6f - (time - 0.4f) * 6f;
            material.SetFloat("_Fade", fade);
            yield return null;
            time += Time.deltaTime;
        }
        material.SetFloat("_Fade", 0f);
    }
}