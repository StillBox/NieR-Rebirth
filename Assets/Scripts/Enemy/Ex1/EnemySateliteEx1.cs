using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySateliteEx1 : Enemy
{
    [SerializeField] private Transform core;
    [SerializeField] private Transform halo;
    
    protected float orbitDir = 0f;
    protected bool isOrbiting = false;

    public virtual void Launch(float dir)
    {
        orbitDir = dir;
        armor.ResetArmor();
        StartCoroutine(Launch());
        StartCoroutine(Twinkle());
    }

    public virtual void Land(float dir)
    {
        orbitDir = dir;
        isOrbiting = false;
        StartCoroutine(Land());
    }

    protected virtual IEnumerator Launch() { yield break; }
    protected virtual IEnumerator Orbit() { yield break; }
    protected virtual IEnumerator Land() { yield break; }

    protected IEnumerator ChangeCoreSscale(float beg, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float scale = Mathf.Lerp(beg, end, rate);
            SetCoreScale(scale);
            yield return null;
            time += Time.deltaTime;
        }
        SetCoreScale(end);
    }

    protected IEnumerator Twinkle()
    {
        float time = 0f;
        while (IsInBattle)
        {
            float rate = time / 2f;
            if (rate < 0.5f)
            {
                float scale = Mathf.Lerp(0.9f, 1.3f, rate);
                halo.localScale = Vector3.one * scale;
                core.localScale = Vector3.one * scale;
            }
            else
            {
                float scale = Mathf.Lerp(1.3f, 0.9f, rate);
                halo.localScale = Vector3.one * scale;
                core.localScale = Vector3.one * scale;
            }
            yield return null;
            time += Time.deltaTime;
            if (time >= 2f) time -= 2f;
        }
    }

    protected void SetCoreScale(float value)
    {
        core.GetComponent<MeshRenderer>().material.SetFloat("_CoreScale", value);
    }
}