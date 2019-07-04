using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowflake : PostEffectBase
{
    private static string[] valueNames =
    {
        "_Inverse0",
        "_Inverse1",
        "_Inverse2",
        "_Inverse3",
        "_Offset0",
        "_Offset1",
        "_Offset2",
        "_Offset3",
        "_Gray0",
        "_Gray1",
        "_Gray2",
        "_Gray3",
        "_Gray4",
        "_Gray5",
        "_Gray6",
        "_Gray7"
    };
    
    public Shader snowflakeShader;
    private Material snowflakeMaterial = null;
    public Material material
    {
        get
        {
            snowflakeMaterial = CheckShaderAndCreateMaterial(snowflakeShader, snowflakeMaterial);
            return snowflakeMaterial;
        }
    }
    
    public void TurnOn(float duration)
    {
        if (effectScale > 0f) return;

        effectScale = 1f;
        SetEffectScale(0f, duration);

        for (int i = 4; i < valueNames.Length; i++)
        {
            StartCoroutine(SetRandomRanges(i, duration));
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

    IEnumerator SetRandomRanges(int index, float duration)
    {
        float time = 0f;
        int nodeCount = (int)(duration / 0.05f);
        float[] nodes = new float[nodeCount];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = duration / nodeCount * i + Random.Range(0f, duration / nodeCount);
        }
        int currentNode = 0;
        while (currentNode < nodeCount)
        {
            while (time < nodes[currentNode])
            {
                yield return null;
                time += Time.deltaTime;
            }
            material.SetVector(valueNames[index], GetRandomRange());
            currentNode++;
        }
        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;
        }
        material.SetVector(valueNames[index], Vector4.zero);
    }

    Vector4 GetRandomRange()
    {
        Vector4 range = new Vector4();
        range.x = Random.Range(-0.2f, 0.9f);
        range.y = Random.Range(-0.2f, 0.9f);
        range.z = Random.Range(0.1f, 0.7f);
        range.w = Random.Range(0.1f, 0.7f);
        return range;
    }
}