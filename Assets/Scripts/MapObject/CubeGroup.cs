using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGroup : MonoBehaviour
{
    public bool isHidden = false;
    public Vector3[] positions;
    [SerializeField] private GameObject cubePrefab;

    public Color gizmosColor = Color.gray;

    private Material[] materials;

    private void Awake()
    {
        materials = new Material[positions.Length];
    }

    void Start()
    {
        if (positions == null) return;

        for (int i = 0; i < positions.Length; i++)
        {
            Transform cube = Instantiate(cubePrefab).transform;
            cube.SetParent(transform);
            cube.localPosition = positions[i];
            materials[i] = cube.GetComponentInChildren<MeshRenderer>().material;
        }

        if (isHidden)
        {
            SetCoreShown(false);
            SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        if (positions == null) return;

        Gizmos.color = gizmosColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        foreach (Vector3 pos in positions)
        {
            Gizmos.DrawCube(new Vector3(pos.x, pos.y + 0.5f, pos.z), Vector3.one * 0.9f);
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void BlinkIn()
    {
        SetActive(true);
        StartCoroutine(Blink(true));
    }

    public void BlinkOut()
    {
        StartCoroutine(Blink(false));
    }

    IEnumerator Blink(bool IsIn)
    {
        yield return null;
        SoundManager.instance.PlayEfx(Efx.SHOW_UP, transform.position);
        SetCoreShown(!IsIn);
        float time = 0f;

        while (time < 0.2f)
        {
            SetArmorAlpha(5f * time);
            yield return null;
            time += Time.deltaTime;
        }
        SetArmorAlpha(1f);
        while (time < 0.4f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        SetCoreShown(IsIn);
        while (time < 0.5f)
        {
            SetArmorAlpha(10f * (0.5f - time));
            yield return null;
            time += Time.deltaTime;
        }
        SetArmorAlpha(0f);
        if (!IsIn)
            SetActive(false);
    }

    void SetCoreShown(bool value)
    {
        foreach (Material material in materials)
        {
            material.SetFloat("_CoreScale", value ? 1f : 0f);
        }
    }

    void SetArmorAlpha(float value)
    {
        foreach (Material material in materials)
        {
            material.SetFloat("_ArmorAlpha", value);
        }
    }
}