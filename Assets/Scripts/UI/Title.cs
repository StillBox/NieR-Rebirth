using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
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
        "_Gray3"
    };
    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    
    void Start()
    {
        InvokeRepeating("StartEffects", 0f, 2.5f);
    }

    void StartEffects()
    {
        StartCoroutine(SetRandomRanges(0));
        StartCoroutine(SetRandomRanges(1));
        StartCoroutine(SetRandomRanges(2));
        StartCoroutine(SetRandomRanges(4));
        StartCoroutine(SetRandomRanges(5));
        StartCoroutine(SetRandomRanges(6));
        StartCoroutine(SetRandomRanges(8));
        StartCoroutine(SetRandomRanges(9));
    }

    IEnumerator SetRandomRanges(int index)
    {
        float time = 0f;
        float[] nodes = new float[12];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = 0.2f * i + Random.Range(-0.08f, 0.08f);
        }
        int currentNode = 0;
        while (currentNode < nodes.Length)
        {
            while (time < nodes[currentNode])
            {
                yield return null;
                time += Time.deltaTime;
            }
            material.SetVector(valueNames[index], GetRandomRange());
            while (time < nodes[currentNode] + 0.1f)
            {
                yield return null;
                time += Time.deltaTime;
            }
            material.SetVector(valueNames[index], Vector4.zero);
            currentNode++;
        }
        while (time < 2.4f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        material.SetVector(valueNames[index], GetRandomRange());
        yield return new WaitForSeconds(0.05f);
        material.SetVector(valueNames[index], Vector4.zero);
    }

    Vector4 GetRandomRange()
    {
        Vector4 range = new Vector4();
        range.x = Random.Range(0.25f, 0.71f);
        range.y = Random.Range(0f, 0.9f);
        range.z = Random.Range(0.005f, 0.025f);
        range.w = Random.Range(0.12f, 0.18f);
        return range;
    }
}