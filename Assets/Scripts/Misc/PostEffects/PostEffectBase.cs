using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class PostEffectBase : MonoBehaviour
{
    protected void Start()
    {
        CheckResources();
    }

    protected void CheckResources()
    {
        bool isSupported = CheckSupport();

        if (!isSupported)
        {
            NotSupported();
        }
    }

    protected bool CheckSupport()
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            Debug.LogWarning("This platform does not support image effects or render textures.");
            return false;
        }

        return true;
    }

    protected void NotSupported()
    {
        enabled = false;
    }

    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null) return null;

        if (shader.isSupported && material && material.shader == shader)
            return material;

        if (!shader.isSupported)
        {
            return null;
        }
        else
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            else
                return null;
        }
    }

    //For Effect Scale

    [Range(0f, 1f)]
    public float effectScale = 1f;
    private Coroutine coroutine = null;

    public void SetEffectScale(float value, float duration)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ChangeEffectScale(effectScale, value, duration));
    }

    IEnumerator ChangeEffectScale(float beg, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            effectScale = Mathf.Lerp(beg, end, t);
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        effectScale = end;
    }
}