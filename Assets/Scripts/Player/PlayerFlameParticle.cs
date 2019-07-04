using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlameParticle : MonoBehaviour
{
    private const float MAX_SPEED = 1.5f;

    public PlayerFlame flameJet;

    private Material material;
    private Vector3 velocity;
    
    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        float scale = Random.Range(0.08f, 0.2f);
        transform.localScale = new Vector3(scale, 0.01f, scale);
        float rotation = Random.Range(-90f, 90f);
        transform.Rotate(Vector3.up, rotation);
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        if (velocity.magnitude > 1f)
            velocity.Normalize();
        velocity *= MAX_SPEED;
        velocity -= flameJet.globalVelocity;
        StartCoroutine(Fade());
    }

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.up, 420f * Time.deltaTime);
    }

    IEnumerator Fade()
    {
        float time = 0f;
        while (time < 0.05f)
        {
            SetAlpha(20f * time);
            yield return null;
            time += Time.deltaTime;
        }
        SetAlpha(1f);
        while (time < 0.15f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time < 0.4f)
        {
            SetAlpha(4f * (0.4f - time));
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