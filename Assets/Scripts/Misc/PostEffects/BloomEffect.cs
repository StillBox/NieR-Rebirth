using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomEffect : PostEffectBase
{
    public Shader bloomShader;
    private Material bloomMaterial = null;
    public Material material
    {
        get
        {
            bloomMaterial = CheckShaderAndCreateMaterial(bloomShader, bloomMaterial);
            return bloomMaterial;
        }
    }

    [Range(0f, 4f)]
    public float luminanceThreshold = 0.4f;

    [Range(0, 20)]
    public int radius = 10;

    [Range(1, 5)]
    public int sampleDown = 2;
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GameManager.IsMiscEffectOn && effectScale > 0f && material != null)
        {
            int rtW = source.width;
            int rtH = source.height;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            material.SetFloat("_LuminanceThreshold", luminanceThreshold);

            Graphics.Blit(source, buffer0, material, 0);

            material.SetFloat("_BlurRadius", radius * effectScale);
            material.SetFloat("_SampleDown", sampleDown);
            RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

            Graphics.Blit(buffer0, buffer1, material, 1);

            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            material.SetTexture("_Bloom", buffer0);
            Graphics.Blit(source, destination, material, 2);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
