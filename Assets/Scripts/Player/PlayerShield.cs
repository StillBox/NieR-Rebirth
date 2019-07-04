using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    private const float MAX_SCALE = 7f;
    private const float SPREAD_TIME = 0.4f;
    private const float PUSH_TIME = 1f;

    void Start()
    {
        SetScale(0f);
        StartCoroutine(Spread());
    }

    private void SetScale(float value)
    {
        transform.localScale = Vector3.one * value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            other.GetComponent<EnemyBullet>().Damage();
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;
            enemy.Damage();

            if (!enemy.isPushable) return;
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller == null) return;

            Vector3 pushDir = controller.transform.position - transform.position;
            pushDir.y = 0;
            pushDir.Normalize();

            StartCoroutine(PushAway(controller, pushDir));
        }
    }

    IEnumerator Spread()
    {
        float time = 0f;
        while (time < SPREAD_TIME)
        {
            yield return null;
            time += Time.deltaTime;
            float rate = time / SPREAD_TIME;
            float scale = MAX_SCALE * (1f - Mathf.Pow(1f - rate, 3f));
            SetScale(scale);
        }
        SetScale(0f);
        time -= SPREAD_TIME;
        while (time < PUSH_TIME)
        {
            yield return null;
            time += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    IEnumerator PushAway(CharacterController controller, Vector3 dir)
    {
        float time = 0f;
        while (time < PUSH_TIME)
        {
            yield return null;
            time += Time.deltaTime;
            float rate = time / PUSH_TIME;
            float speed = 10f * Mathf.Pow(1f - rate, 2f);
            if (controller == null) break;
            controller.Move(dir * speed * Time.deltaTime);
        }
    }
}