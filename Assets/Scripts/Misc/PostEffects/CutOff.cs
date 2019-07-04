using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutOff : PostEffectBase
{
    private static string[] valueNames =
    {
        "_Offset0",
        "_Offset1",
        "_Offset2",
        "_Offset3"
    };
    
    public Shader cutOffShader;
    private Material cutOffMaterial = null;
    public Material material
    {
        get
        {
            cutOffMaterial = CheckShaderAndCreateMaterial(cutOffShader, cutOffMaterial);
            return cutOffMaterial;
        }
    }

    public void TurnOn()
    {
        if (effectScale > 0f) return;
        effectScale = 1f;
        for (int i = 0; i < valueNames.Length; i++)
        {
            StartCoroutine(SetRandomRanges(i));
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectScale > 0f && material != null)
        {
            int rtW = source.width;
            int rtH = source.height;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(source, buffer0);
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

    IEnumerator SetRandomRanges(int index)
    {
        while (effectScale > 0f)
        {
            float gap = Random.Range(0f, 0.2f);
            yield return new WaitForSeconds(gap);

            float duration = Random.Range(0.02f, 0.08f);
            material.SetVector(valueNames[index], GetRandomRange());
            yield return new WaitForSeconds(duration);
        }
    }

    Vector3 GetRandomRange()
    {
        Vector3 range = new Vector3();
        float height = Random.Range(0.008f, 0.02f);
        range.x = Random.Range(0f, 1f - height);
        range.y = range.x + height;
        range.z = 0.002f * Mathf.Sign(Random.Range(-1f, 1f));
        return range;
    }
}
