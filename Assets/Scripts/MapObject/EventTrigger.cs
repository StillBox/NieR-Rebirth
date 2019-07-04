using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public bool isRepeated;
    public UnityEvent onTrigger;

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger.Invoke();
            if (!isRepeated)
                Destroy(gameObject);
        }
    }
}