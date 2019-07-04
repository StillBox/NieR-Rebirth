using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residue : MonoBehaviour
{
    public float lag = 0.18f;
    public float period = 0.04f;
    public float gap = 0.02f;

    private const int HOR_COUNT = 4;
    private const int VER_COUNT = 2;
    private const int FRAME_COUNT = 8;
    private Material material;
    
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        StartCoroutine(FrameAnimation());
    }
    
    IEnumerator FrameAnimation()
    {
        float time = -lag;
        int index = 0;
        bool isInGap = true;
        material.SetFloat("_CurrentFrame", FRAME_COUNT);

        while (index < FRAME_COUNT)
        {
            if (time >= 0 && isInGap)
            {
                isInGap = false;
                material.SetFloat("_CurrentFrame", index);
            }
            if (time >= period)
            {
                time -= period + gap;
                index++;
                isInGap = true;
                material.SetFloat("_CurrentFrame", FRAME_COUNT);
            }
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}