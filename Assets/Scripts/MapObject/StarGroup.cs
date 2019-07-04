using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGroup : MonoBehaviour
{
    [SerializeField] private Star starPrefab;
    public int maxCount;

    private List<Star> stars;
    
    private void Awake()
    {
        stars = new List<Star>();   
    }

    void Start()
    {
        for (int i = 0; i < maxCount; i++)
        {
            Star star = Instantiate(starPrefab, transform);
            stars.Add(star);
        }
    }
}