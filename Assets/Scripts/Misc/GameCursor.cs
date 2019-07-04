using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor : MonoBehaviour
{
    private const float MAX_SPEED = 720f;
    private const float MIN_SPEED = -60f;

    private const float MAX_SCALE = 0.2f;
    private const float MIN_SCALE = 0.15f;

    private const float MAX_RADIUS = 0.36f;
    private const float MIN_RADIUS = 0.2f;

    private const float SWITCH_TIME = 0.2f;

    public static GameCursor instance = null;
    public static bool isActivated = false;

    [SerializeField] private Transform ring;
    [SerializeField] private Transform center;
    
    private Material ringMaterial = null;

    private float speed = MIN_SPEED;
    private float scale = MAX_SCALE;
    private float radius = MAX_RADIUS;
    private bool isFocusing = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    private void Start()
    {
        isActivated = true;
        ringMaterial = ring.GetComponent<MeshRenderer>().material;
        SetRadius(radius);
        SetScale(scale);
    }

    private void Update()
    {
        if (isFocusing)
        {
            if (speed < MAX_SPEED)
            {
                speed += (MAX_SPEED - MIN_SPEED) * Time.deltaTime / SWITCH_TIME;
                speed = Mathf.Min(MAX_SPEED, speed);
            }
            if (scale < MAX_SCALE)
            {
                scale += (MAX_SCALE - MIN_SCALE) * Time.deltaTime / SWITCH_TIME;
                scale = Mathf.Min(MAX_SCALE, scale);
                SetScale(scale);
            }
            if (radius > MIN_RADIUS)
            {
                radius -= (MAX_RADIUS - MIN_RADIUS) * Time.deltaTime / SWITCH_TIME;
                radius = Mathf.Max(MIN_RADIUS, radius);
                SetRadius(radius);
            }
        }
        else
        {
            if (speed > MIN_SPEED)
            {
                speed -= (MAX_SPEED - MIN_SPEED) * Time.deltaTime / 0.5f;
                speed = Mathf.Max(MIN_SPEED, speed);
            }
            if (scale > MIN_SCALE)
            {
                scale -= (MAX_SCALE - MIN_SCALE) * Time.deltaTime / SWITCH_TIME;
                scale = Mathf.Max(MIN_SCALE, scale);
                SetScale(scale);
            }
            if (radius < MAX_RADIUS)
            {
                radius += (MAX_RADIUS - MIN_RADIUS) * Time.deltaTime / 0.5f;
                radius = Mathf.Min(MAX_RADIUS, radius);
                SetRadius(radius);
            }
        }

        transform.Rotate(0f, speed * Time.deltaTime, 0f);
    }

    private void OnDestroy()
    {
        isActivated = false;
    }

    private void SetRadius(float value)
    {
        ringMaterial.SetFloat("_Radius", value);
    }

    private void SetScale(float value)
    {
        center.transform.localScale = Vector3.one * value;
    }

    public static Vector3 GetPosition()
    {
        Vector3 pos = instance.transform.position;
        pos.y = 0f;
        return pos;
    }

    public void SetFocus(bool value)
    {
        isFocusing = value;
    }
}