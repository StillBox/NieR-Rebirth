using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlame : MonoBehaviour
{
    [SerializeField] private PlayerFlameParticle particle_solid;
    [SerializeField] private PlayerFlameParticle particle_wire;
    public Vector3 globalVelocity = new Vector3(0f, 0f, 0f);

    //Threshold for playing and stopping

    private float stopThreshold = 0f;
    private float playThreshold = 0f;

    public void SetThreshold(float stop, float play)
    {
        stopThreshold = stop;
        playThreshold = play;
    }

    private bool IsPlaying
    {
        get; set;
    }

    private void Awake()
    {
        IsPlaying = false;
    }

    public void CheckSpeed(Vector3 value)
    {
        Vector3 actSpeed = value + globalVelocity;
        if (!IsPlaying && actSpeed.magnitude > playThreshold) Play();
        else if (IsPlaying && actSpeed.magnitude < stopThreshold) Stop();
    }

    void Play()
    {
        if (IsPlaying) return;
        IsPlaying = true;
        StartCoroutine(Jet());
    }

    void Stop()
    {
        if (IsPlaying)
            IsPlaying = false;
    }

    IEnumerator Jet()
    {
        float time = 0f;
        float gap = 0f;
        while (IsPlaying)
        {
            if (time > gap)
            {
                time -= gap;
                AddParticle();
                gap = Random.Range(0.01f, 0.1f);
            }
            yield return null;
            time += Time.deltaTime;
        }
    }

    private void AddParticle()
    {
        PlayerFlameParticle particle = Random.Range(0f, 1f) <= 0.6f ? 
            Instantiate(particle_solid, transform) : Instantiate(particle_wire, transform);
        float offset_x = Random.Range(-0.25f, 0.25f);
        float offset_z = Random.Range(-0.1f, 0f);
        particle.transform.localPosition = new Vector3(offset_x, 0f, offset_z);
        particle.transform.SetParent(EffectManager.instance.transform, true);
        particle.flameJet = this;
    }
}
