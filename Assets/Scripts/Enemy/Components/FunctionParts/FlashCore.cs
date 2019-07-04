using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashCore : MonoBehaviour
{
    public Color colorBeg = new Color(0.555f, 0.324f, 0.277f);
    public Color colorEnd = new Color(1f, 0.621f, 0.543f);
    public float period = 1f;

    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    
    void Start ()
    {
        StartCoroutine(Flash());
	}
	
    IEnumerator Flash()
    {
        float time = 0f;
        while (true)
        {
            if (time >= period)
                time -= period;
            float rate = 2f * Mathf.Abs(time / period - 0.5f);
            Color color = Color.Lerp(colorBeg, colorEnd, rate);
            material.SetColor("_Color", color);
            yield return null;
            time += Time.deltaTime;
        }
    }
}
