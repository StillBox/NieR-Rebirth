using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distort : MonoBehaviour
{
    private const int NODE_COUNT = 9;
    private const float MAX_OFFSET = 0.1f;
    private Material distortMaterial = null;

    private bool isOn = false;
    public void TurnOn()
    {
        if (isOn) return;
        isOn = true;
        for (int i = 0; i < NODE_COUNT; i++)
        {
            StartCoroutine(SetRandomOffset(i, 0.04f));
        }
    }

    public void TurnOff()
    {
        isOn = false;
        distortMaterial.SetFloat("_Alpha", 0f);
    }

    private void Awake()
    {
        distortMaterial = GetComponent<Image>().material;
    }
    
    IEnumerator SetRandomOffset(int index, float gap)
    {
        distortMaterial.SetFloat("_Alpha", 0.8f);
        float time = 0f;
        while (isOn)
        {
            time += Time.unscaledDeltaTime;
            if (time >= gap)
            {
                time -= gap;
                distortMaterial.SetFloat("_Offset" + index, Random.Range(-MAX_OFFSET, MAX_OFFSET));
            }
            yield return null;
        }
    }
}