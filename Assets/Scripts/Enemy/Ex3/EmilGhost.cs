using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilGhost : MonoBehaviour
{
    public GhostParticle[] prefabs;
    public EnemyProtoEx3 emilPrefab;

    public float rebirthTime;
    public Vector3 rebirthPosition;

    private bool isActivated = true;
        
    void Start()
    {
        StartCoroutine(Rebirth(rebirthTime));
        StartCoroutine(CreateGhost(GhostParticle.lifeTime / 10f));
    }
        
    public void Remove()
    {
        isActivated = false;
        StartCoroutine(RemoveGhost(GhostParticle.lifeTime));
    }

    IEnumerator Rebirth(float duration)
    {
        float time = 0f;
        Vector3 origin = transform.position;
        Vector3 offset = rebirthPosition - origin;
        while (time < duration)
        {
            float rate = time / duration;
            Vector3 position = origin + offset * rate;
            transform.position = position;
            yield return null;
            time += Time.deltaTime;
        }
        transform.position = rebirthPosition;
        EnemyManager.instance.SetEnemy(emilPrefab, rebirthPosition);
        SoundManager.instance.PlayEfx(Efx.SHOW_UP, rebirthPosition);
        Remove();
    }

    IEnumerator CreateGhost(float gap)
    {
        float time = 0f;
        while (isActivated)
        {
            if (time >= gap)
            {
                time -= gap;
                Instantiate(prefabs[Random.Range(0, 2)], transform);
            }
            yield return null;
            time += Time.deltaTime;
        }
    }

    IEnumerator RemoveGhost(float wait)
    {
        isActivated = false;
        yield return new WaitForSeconds(wait);
        Destroy(gameObject);
    }
}
