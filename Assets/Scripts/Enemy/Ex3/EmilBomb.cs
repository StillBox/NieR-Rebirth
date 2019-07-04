using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilBomb : MonoBehaviour
{
    private const float SPEED = 0.3f;
    private float offset;
    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        offset = 0f;
        SetScale(0f);
    }

    void Update()
    {
        offset += SPEED * Time.deltaTime;
        while (offset >= 1f)
        {
            offset -= 1f;
        }
        material.SetFloat("_Offset", offset);
    }

    public void Explode(float duration)
    {
        StartCoroutine(ChangeScale(0f, 32f, duration));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.Damage(3);
        }
    }

    IEnumerator ChangeScale(float beg, float end, float duration)
    {
        SetScale(beg);
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float current = Mathf.Lerp(beg, end, rate);
            SetScale(current);
            yield return null;
            time += Time.deltaTime;
        }
        SetScale(end);
        Destroy(gameObject);
    }

    private void SetScale(float value)
    {
        transform.localScale = new Vector3(1f, 0.5f, 1f) * value;
    }
}
