using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussBlur : PostEffectBase
{
    public Shader gaussBlurShader;
    private Material gaussBlurMaterial = null;
    public Material material
    {
        get
        {
            gaussBlurMaterial = CheckShaderAndCreateMaterial(gaussBlurShader, gaussBlurMaterial);
            return gaussBlurMaterial;
        }
    }

    [Range(0, 20)]
    public int radius = 18;

    [Range(1, 5)]
    public int sampleDown = 3;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GameManager.IsMiscEffectOn && effectScale > 0f && material != null)
        {
            int rtW = source.width;
            int rtH = source.height;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(source, buffer0);

            material.SetFloat("_BlurRadius", radius * effectScale);
            material.SetFloat("_SampleDown", sampleDown);
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
