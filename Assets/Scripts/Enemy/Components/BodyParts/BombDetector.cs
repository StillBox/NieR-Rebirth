using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDetector : MonoBehaviour
{
    private Enemy parent = null;

    private void Awake()
    {
        parent = GetComponentInParent<Enemy>();
    }

    private void Start()
    {
        SetScale(0f);
    }
    
    public void Expand(float scale, float duration, float wait = 0f)
    {
        StartCoroutine(ChangeScale(scale, duration, wait));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.Damage();
            Player.instance.PushAway(transform.position);
            parent.Damage(999);
        }
    }

    IEnumerator ChangeScale(float scale, float duration, float wait)
    {
        yield return new WaitForSeconds(wait);
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float current = Mathf.Lerp(0f, scale, rate);
            SetScale(current);
            yield return null;
            time += Time.deltaTime;
        }
        SetScale(scale);
    }

    private void SetScale(float value)
    {
        transform.localScale = new Vector3(value, 0.1f, value);
    }
}