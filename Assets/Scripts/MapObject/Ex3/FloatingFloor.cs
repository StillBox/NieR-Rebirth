using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingFloor : MonoBehaviour
{
    private const float MAX_AMPLITUDE = 0.05f;

    public float width = 3f;

    [SerializeField] private Transform floor;
    [SerializeField] private Transform wall;
    [SerializeField] private BoxCollider trigger;

    private bool isMoving = false;
    private bool isInPosition = true;
    private bool isStable = true;
    public bool IsStable
    {
        get { return isStable; }
        set { isStable = value; }
    }

    void Start()
    {
        floor.gameObject.SetActive(true);
        wall.gameObject.SetActive(false);
    }

    public void SetWidth(float value)
    {
        width = value;
        trigger.size = new Vector3(value, 2f, value);
        floor.localScale = new Vector3(value, 0.5f, value);
//        wall.localScale = new Vector3(value, 2f, value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStable && other.CompareTag("Enemy"))
        {
            bool broke = false;
            EnemyBossEx3 emil = other.GetComponent<EnemyBossEx3>();
            if (emil != null && emil.IsAbleToBreak) broke = true;
            EnemyFinalEx3 emilFinal = other.GetComponent<EnemyFinalEx3>();
            if (emilFinal != null) broke = true;

            if (broke)
            {
                if (MapEx3.instance.IsMoving)
                    MoveUp();
                else
                    MoveDown();
            }
        }
    }

    //Functions for floor movement

    public void MoveUp()
    {
        if (isInPosition)
            StartCoroutine(MoveUp(3f, 1f));
    }

    public void MoveDown()
    {
        if (isInPosition)
            StartCoroutine(MoveDown(2f, 8f));
    }

    public void MoveIn()
    {
        if (!isInPosition && !isMoving)
            StartCoroutine(MoveIn(0f, 3f));
    }

    IEnumerator MoveUp(float wait, float duration)
    {
        isMoving = true;
        isInPosition = false;

        EffectManager.instance.Explode(transform.position, ExplosionType.FLOOR);
        SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
//        wall.gameObject.SetActive(true);

        float time = 0f;
        while (time < wait)
        {
            floor.SetLocalPositionX(MAX_AMPLITUDE * Random.Range(-1f, 1f));
            floor.SetLocalPositionY(-0.25f + MAX_AMPLITUDE * Random.Range(-0.1f, 0.1f));
            floor.SetLocalPositionZ(MAX_AMPLITUDE * Random.Range(-1f, 1f));
            yield return null;
            time += Time.deltaTime;
        }
        floor.SetLocalPositionX(0f);
        floor.SetLocalPositionZ(0f);
        time -= wait;
        while (time < duration)
        {
            float rate = time / duration;
            float height = Mathf.Lerp(-0.25f, 20f, rate);
            floor.SetPositionY(height);
            yield return null;
            time += Time.deltaTime;
        }

        floor.gameObject.SetActive(false);

        isMoving = false;
    }

    IEnumerator MoveDown(float wait, float duration)
    {
        isMoving = true;
        isInPosition = false;

        EffectManager.instance.Explode(transform.position, ExplosionType.FLOOR);
        SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
        //        wall.gameObject.SetActive(true);

        float time = 0f;
        while (time < wait)
        {
            floor.SetLocalPositionX(MAX_AMPLITUDE * Random.Range(-1f, 1f));
            floor.SetLocalPositionY(-0.25f + MAX_AMPLITUDE * Random.Range(-0.1f, 0.1f));
            floor.SetLocalPositionZ(MAX_AMPLITUDE * Random.Range(-1f, 1f));
            yield return null;
            time += Time.deltaTime;
        }
        floor.SetLocalPositionX(0f);
        floor.SetLocalPositionZ(0f);
        time -= wait;
        while (time < duration)
        {
            float rate = time / duration;
            float height = Mathf.Lerp(-0.25f, -400f, 1f - Mathf.Cos(0.5f * Mathf.PI * rate));
            floor.SetLocalScaleX(width * (1f - rate));
            floor.SetLocalScaleZ(width * (1f - rate));
            floor.SetPositionY(height);
            yield return null;
            time += Time.deltaTime;
        }

        floor.gameObject.SetActive(false);

        isMoving = false;
    }

    IEnumerator MoveIn(float wait, float duration)
    {
        isMoving = true;

        yield return new WaitForSeconds(wait);

        floor.gameObject.SetActive(true);

        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float height = Mathf.Lerp(-400f, -0.25f, Mathf.Sin(0.5f * Mathf.PI * rate));
            floor.SetLocalScaleX(width * rate);
            floor.SetLocalScaleZ(width * rate);
            floor.SetLocalPositionY(height);
            yield return null;
            time += Time.deltaTime;
        }
        floor.SetLocalScaleX(width);
        floor.SetLocalScaleZ(width);
        floor.SetPositionY(-0.25f);
//        wall.gameObject.SetActive(false);

        isMoving = false;
        isInPosition = true;
    }
}