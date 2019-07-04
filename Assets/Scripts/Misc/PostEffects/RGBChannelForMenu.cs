using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBChannelForMenu : PostEffectBase
{
    public Shader rgbChannelShader;
    private Material rgbChannelMaterial = null;
    public Material material
    {
        get
        {
            rgbChannelMaterial = CheckShaderAndCreateMaterial(rgbChannelShader, rgbChannelMaterial);
            return rgbChannelMaterial;
        }
    }

    [Range(-1f, 1f)]
    public float scale = 0f;

    public void Shake(float pitch, float duration)
    {
        StartCoroutine(ScaleShake(pitch, duration));
    }

    IEnumerator ScaleShake(float pitch, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float rate = Mathf.Cos(0.5f * Mathf.PI * time / duration);
            scale = Random.Range(0f, pitch * rate);
            yield return null;
            time += Time.deltaTime;
        }
        scale = 0f;
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GameManager.IsMiscEffectOn && effectScale > 0f && material != null)
        {
            int rtW = source.width;
            int rtH = source.height;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(source, buffer0);

            material.SetFloat("_Scale", scale * effectScale);
            RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

            Graphics.Blit(buffer0, buffer1, material);

            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            Graphics.Blit(buffer0, destination);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
