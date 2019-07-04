using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostParticle : MonoBehaviour
{
    public const float lifeTime = 1.5f;
    private const float MAX_SPEED = 0.1f;

    private Material material;
    private Vector3 velocity;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        float scale = Random.Range(0.08f, 0.16f);
        transform.localScale = new Vector3(scale, scale, scale);
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
        velocity.Normalize();
        Vector3 position = velocity * 0.2f;
        position.y += 0.5f;
        transform.localPosition = position;
        velocity *= Random.Range(0f, MAX_SPEED);
        StartCoroutine(Fade(lifeTime));
    }

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.Self);
    }

    IEnumerator Fade(float duration)
    {
        float time = 0f;
        while (time / duration < 0.2f)
        {
            SetAlpha(2f * time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(0.4f);
        while (time / duration < 0.6f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time / duration < 1f)
        {
            SetAlpha(1f * (1f - time / duration));
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(0f);
        Destroy(gameObject);
    }

    private void SetAlpha(float value)
    {
        material.SetFloat("_Alpha", value);
    }
}