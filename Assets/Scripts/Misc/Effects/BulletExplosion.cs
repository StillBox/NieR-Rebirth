using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    [SerializeField] private BulletExplosionParticle particle_solid;
    [SerializeField] private BulletExplosionParticle particle_wire;

    private void Start()
    {
        for (int i = 0; i < 42; i++)
        {
            AddParticle();
        }
        Invoke("Destroy", 0.3f);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void AddParticle()
    {
        BulletExplosionParticle particle = Random.Range(0f, 1f) <= 0.4f ?
            Instantiate(particle_solid, transform) : Instantiate(particle_wire, transform);
        particle.transform.localPosition = new Vector3(0f, 0.4f, 0f);
    }
}