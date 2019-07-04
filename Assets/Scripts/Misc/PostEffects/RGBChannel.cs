using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBChannel : PostEffectBase
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
    public float scale = 0.006f;
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GameManager.IsRGBChannelOn && effectScale > 0f && material != null)
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
