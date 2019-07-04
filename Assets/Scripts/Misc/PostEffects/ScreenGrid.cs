using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenGrid : PostEffectBase
{
    public Shader screenGridShader;
    private Material screenGridMaterial = null;
    public Material material
    {
        get
        {
            screenGridMaterial = CheckShaderAndCreateMaterial(screenGridShader, screenGridMaterial);
            return screenGridMaterial;
        }
    }

    public float gridSize = 6f;
    public float borderWidth = 1f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GameManager.IsScreenGridOn && effectScale > 0f && material != null)
        {
            int rtW = source.width;
            int rtH = source.height;

            float countW = rtW / gridSize;
            float countH = rtH / gridSize;
            float borderW = borderWidth / gridSize;
            float borderH = borderWidth / gridSize;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(source, buffer0);

            material.SetFloat("_CountW", countW);
            material.SetFloat("_CountH", countH);
            material.SetFloat("_BorderW", borderW);
            material.SetFloat("_BorderH", borderH);
            material.SetFloat("_ShadeScale", effectScale);
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
