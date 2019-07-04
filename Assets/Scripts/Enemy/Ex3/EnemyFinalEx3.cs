using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFinalEx3 : Enemy
{
    static Vector2[] cruiseNodes =
    {
        new Vector2(0, 1f), new Vector2(0, 3), new Vector2(4, 3), new Vector2(4, -3), new Vector2(-4, -3), new Vector2(-4, 3),
        new Vector2(-1, 3), new Vector2(-1, 2), new Vector2(-3, 2), new Vector2(-3, -2), new Vector2(3, -2), new Vector2(3, 2),
        new Vector2(1, 2), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, -1), new Vector2(-2, -1), new Vector2(-2, 1),
        new Vector2(0, 1), new Vector2(0, 0)
    };
    
    [SerializeField] private GameObject body;
    [SerializeField] private BossShield shield;
    [SerializeField] private EmilBomb bomb;
    
    private Material material;

    void Awake()
    {
        material = body.GetComponent<MeshRenderer>().material;
    }

    public override void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void StartCountDown()
    {
        IsInBattle = true;

        controller.radius = 0.4f;
        Destroy(shield.gameObject);
        SoundManager.instance.PlayEfx(Efx.BREAK, transform.position);

        StartCoroutine(CountDown(20f));
        StartCoroutine(Cruise(1f));
    }

    //For movement

    IEnumerator Cruise(float wait)
    {
        yield return new WaitForSeconds(wait);

        int node = 1;
        int count = 1;
        while (count < cruiseNodes.Length)
        {
            if (!IsInBattle) count = cruiseNodes.Length - 1;
            float time = 0f;
            Vector3 beg = new Vector3(cruiseNodes[node - 1].x, 0f, cruiseNodes[node - 1].y) * MapEx3.GRID_SIZE;
            Vector3 end = new Vector3(cruiseNodes[node].x, 0f, cruiseNodes[node].y) * MapEx3.GRID_SIZE;
            Vector3 delta = end - beg;
            float duration = delta.magnitude / 10f;
            while (time < duration)
            {
                float rate = time / duration;
                Vector3 target = beg + delta * 0.5f * (1f - Mathf.Cos(Mathf.PI * rate));
                controller.Move(target - transform.position);
                yield return null;
                time += Time.deltaTime;
            }
            node++;
            count++;
        }
    }
    
    //For damage and death

    public override void Damage(int damagePoint = 1)
    {
        if (shield != null)
            SoundManager.instance.PlayEfx(Efx.HIT_HARD_ENEMY, transform.position);
        else
            base.Damage(damagePoint);
    }

    protected override void Explode()
    {
        EffectManager.instance.Explode(transform.position, ExplosionType.LARGE);
        material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1f));
        HUDManager.instance.HideTimer();
    }

    IEnumerator CountDown(float duration)
    {
        HUDManager.instance.SetTimer(duration, null, transform);
        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            SetColor(rate);
            yield return null;
            time += Time.deltaTime;
            if (!IsInBattle) yield break;
        }
        HUDManager.instance.HideTimer();
        bomb.Explode(0.8f);
        Damage(999);
        body.SetActive(false);
    }

    private void SetColor(float rate)
    {
        Color beg = new Color(0.25f, 0.25f, 0.25f, 1f);
        Color end = new Color(0.8f, 0.25f, 0f, 1f);
        Color color = beg + rate * (end - beg);
        material.SetColor("_Color", color);
    }
}
