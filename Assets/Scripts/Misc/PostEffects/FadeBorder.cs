using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBorder : PostEffectBase
{
    public Shader fadeBorderShader;
    private Material fadeBorderMaterial = null;
    public Material material
    {
        get
        {
            fadeBorderMaterial = CheckShaderAndCreateMaterial(fadeBorderShader, fadeBorderMaterial);
            return fadeBorderMaterial;
        }
    }

    [Range(0f, 1f)]
    public float fixedFade = 0.04f;

    [Range(0f, 1f)]
    public float ratioFade = 0.18f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GameManager.IsFadeBorderOn && effectScale > 0f && material != null)
        {
            int rtW = source.width;
            int rtH = source.height;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(source, buffer0);

            material.SetFloat("_FixedFade", fixedFade * effectScale);
            material.SetFloat("_RatioFade", ratioFade * effectScale);
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
