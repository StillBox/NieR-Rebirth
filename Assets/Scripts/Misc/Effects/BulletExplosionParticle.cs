using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosionParticle : MonoBehaviour
{
    private const float MAX_SPEED = 8f;
    private const float SPREAD_SIZE = 0.5f;

    private Material material;
    private Vector3 velocity;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        float scale = Random.Range(0.03f, 0.04f);
        transform.localScale = new Vector3(scale, scale, scale);
        velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        if (velocity.magnitude > 1f)
            velocity.Normalize();
        velocity *= MAX_SPEED;
        StartCoroutine(Fade());
    }

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);
        if (Mathf.Abs(transform.localPosition.x) >= SPREAD_SIZE || Mathf.Abs(transform.localPosition.z) > SPREAD_SIZE)
        {
            float x = Mathf.Max(-SPREAD_SIZE, Mathf.Min(SPREAD_SIZE, transform.localPosition.x));
            float y = transform.localPosition.y;
            float z = Mathf.Max(-SPREAD_SIZE, Mathf.Min(SPREAD_SIZE, transform.localPosition.z));
            transform.localPosition = new Vector3(x, y, z);
            velocity = Vector3.zero;
        }
    }

    IEnumerator Fade()
    {
        float time = 0f;
        SetAlpha(0.8f);
        while (time < 0.1f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time < 0.2f)
        {
            SetAlpha(8f * (0.2f - time));
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
