using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    [Range(0f, 1f)]
    public float speedRate = 1f;
    public Vector3 maxOffset = Vector3.zero;
    public Vector3 variationFactor = Vector3.one;
    public bool randomPhase = true;

    private RectTransform thisTransform = null;
    private Vector3 origin = Vector3.zero;
    private Vector3 phase = Vector3.zero;
    private Vector3 speed = Vector3.one;
    private float timer = 0f;

    private void Awake()
    {
        thisTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        origin = thisTransform != null ? (Vector3)thisTransform.anchoredPosition : transform.position;
        if (randomPhase) phase = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * 2f * Mathf.PI;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime * speedRate;
        speed.x = (Mathf.Cos(2f * Mathf.PI * timer / variationFactor.x) + 2f) / 3f;
        speed.y = (Mathf.Cos(2f * Mathf.PI * timer / variationFactor.y) + 2f) / 3f;
        speed.z = (Mathf.Cos(2f * Mathf.PI * timer / variationFactor.z) + 2f) / 3f;
        phase += speed * Time.unscaledDeltaTime * speedRate;

        Vector3 position = origin;
        position.x += maxOffset.x * Mathf.Sin(phase.x);
        position.y += maxOffset.y * Mathf.Sin(phase.y);
        position.z += maxOffset.z * Mathf.Sin(phase.z);

        if (thisTransform != null)
            thisTransform.anchoredPosition = position;
        else
            transform.position = position;
    }
}
