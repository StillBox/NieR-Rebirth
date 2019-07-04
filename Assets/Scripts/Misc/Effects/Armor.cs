using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    public float armorTime = 0.12f;
    [SerializeField] private GameObject[] parts;
    private Material[] materials;

    public bool IsArmored
    {
        get; set;
    }

    public bool IsCoreShown
    {
        get; set;
    }
    
    private void Awake()
    {
        IsCoreShown = false;
        materials = new Material[parts.Length];
    }
    
    public void SetArmor()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            materials[i] = parts[i] == null ? null : parts[i].GetComponent<MeshRenderer>().material;
        }
        IsArmored = true;
        StartCoroutine(ShowArmor(armorTime));
    }

    public void ResetArmor()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            materials[i] = parts[i] == null ? null : parts[i].GetComponent<MeshRenderer>().material;
        }
        IsArmored = false;
        SetArmorAlpha(0f);
    }

    IEnumerator ShowArmor(float duration)
    {
        float time = 0f;
        while (time < 0.2f * duration)
        {
            float alpha = time / duration * 5f;
            SetArmorAlpha(alpha);
            yield return null;
            time += Time.deltaTime;
        }
        SetArmorAlpha(1f);
        while (time < 0.5f * duration)
        {
            yield return null;
            time += Time.deltaTime;
        }
        while (time < duration)
        {
            float alpha = (1f - time / duration) * 2f;
            SetArmorAlpha(alpha);
            yield return null;
            time += Time.deltaTime;
        }
        SetArmorAlpha(0f);
        IsArmored = false;
    }

    void SetArmorAlpha(float value)
    {
        foreach (Material material in materials)
        {
            if (material != null)
                material.SetFloat("_ArmorAlpha", value);
        }
    }
}