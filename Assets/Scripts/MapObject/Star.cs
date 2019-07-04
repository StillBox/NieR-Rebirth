using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private Transform core;
    [SerializeField] private Transform halo;

    private Vector3 velocity;
    private float lifeTime;
    private float gapTime;
    private bool isClose;

    void Start()
    {
        Reset();
    }
    
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    private void Reset()
    {
        float z = Random.Range(-3f, 40f);
        float width = 9.6f * (1f + z / 10f);
        float height = 5.4f * (1f + z / 10f);
        float x = Random.Range(-width * 0.6f, width * 0.4f);
        float y = Random.Range(-height * 0.5f, height * 0.4f);
        transform.position = new Vector3(x, y, z);
        halo.localScale = new Vector3(1f, 1f, 1f);
        core.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        transform.localScale = new Vector3(0f, 0f, 0f);

        if (z <= 0f)
        {
            core.gameObject.SetActive(false);
            isClose = true;
        }
        else
        {
            core.gameObject.SetActive(true);
            isClose = false;
        }
        halo.gameObject.SetActive(true);

        velocity.y = Random.Range(-0.2f, 0.2f);
        velocity.x = Random.Range(2f * Mathf.Abs(velocity.y), 1f);
        velocity.y += 0.1f;
        velocity *= (z * 0.1f + 1f) * 0.25f;

        lifeTime = Random.Range(4f, 16f);
        gapTime = Random.Range(1f, 4f);
        StartCoroutine(StartTimeline());
    }

    IEnumerator StartTimeline()
    {
        float time = 0f;
        while (time < 0.25f)
        {
            float scale = 2 * time;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
            time += Time.deltaTime;
        }
        while (time < lifeTime - 0.5f)
        {
            if (!isClose)
            {
                float scale = Random.Range(0.9f, 1.1f);
                core.localScale = new Vector3(scale, scale, scale) * 0.15f;
                scale = scale * scale;
                halo.localScale = new Vector3(scale, scale, scale);
            }
            yield return null;
            time += Time.deltaTime;
        }
        while (time < lifeTime - 0.25f)
        {
            float scale = 4f * (lifeTime - 0.25f - time);
            halo.localScale = new Vector3(scale, scale, scale);
            yield return null;
            time += Time.deltaTime;
        }
        while (time < lifeTime)
        {
            float scale = 2f * (lifeTime - time);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
            time += Time.deltaTime;
        }
        core.gameObject.SetActive(false);
        halo.gameObject.SetActive(false);
        Invoke("Reset", gapTime);
    }
}