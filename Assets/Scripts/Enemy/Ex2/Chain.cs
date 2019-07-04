using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public float armorTime = 0.12f;
    public float rollSpeed = 1f;

    [SerializeField] private Transform[] chains;
    private Material[] materials;

    public bool IsArmored
    {
        get; set;
    }

    private void Awake()
    {
        materials = new Material[chains.Length];
        for (int i = 0; i < chains.Length; i++)
        {
            materials[i] = chains[i] == null ? null : chains[i].GetComponent<MeshRenderer>().material;
        }
    }

    private void Start()
    {
        StartCoroutine(RollTexture(rollSpeed));
    }

    public void SetArmor()
    {
        IsArmored = true;
        StartCoroutine(ShowArmor(armorTime));
    }

    IEnumerator ShowArmor(float duration)
    {
        float time = 0f;
        while (time < 0.2f * duration)
        {
            float light = time / duration * 5f;
            SetChainLight(light);
            yield return null;
            time += Time.deltaTime;
        }
        SetChainLight(1f);
        while (time < 0.5f * duration)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time < duration)
        {
            float light = (1f - time / duration) * 2f;
            SetChainLight(light);
            yield return null;
            time += Time.deltaTime;
        }
        SetChainLight(0f);
        IsArmored = false;
    }

    void SetChainLight(float value)
    {
        foreach (Material material in materials)
        {
            if (material != null)
                material.SetColor("_Color", new Color(value, value, value, 1f));
        }
    }

    IEnumerator RollTexture(float speed)
    {
        float offset = 0f;
        while (true)
        {
            SetTextureOffset(offset);
            yield return null;
            offset += Time.deltaTime * speed;
            if (offset >= 1f) offset -= 1f;
        }
    }

    void SetTextureOffset(float value)
    {
        foreach (Material material in materials)
        {
            if (material != null)
                material.SetTextureOffset("_MainTex", new Vector2(value, 0));
        }
    }
}