using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedLight : MonoBehaviour
{
    public Transform target;
    public Vector3 m_offset = new Vector3(0f, 8f, 0f);

    void Start()
    {
        target = Player.instance.transform;
    }

    void LateUpdate()
    {
        this.transform.position = target.position + m_offset;
        transform.LookAt(target);
    }
}