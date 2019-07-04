using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedCamera : MonoBehaviour
{
    private float damping = 0.93f;

    public Transform target;
    
    [SerializeField, SetProperty("Offset")]
    private Vector3 offset = new Vector3(0f, 12f, -5f);
    public Vector3 Offset
    {
        get
        {
            return offset;
        }
        set
        {
            offset = value;
            ImmediateSet();
        }
    }

    public void SetOffset(Vector3 value)
    {
        offset = value;
    }

    [SerializeField, SetProperty("AnchorOffset")]
    private Vector3 anchorOffset = new Vector3(0f, 0f, 0f);
    public Vector3 AnchorOffset
    {
        get
        {
            return anchorOffset;
        }
        set
        {
            anchorOffset = value;
            ImmediateSet();
        }
    }

    public void SetAnchorOffset(Vector3 value)
    {
        anchorOffset = value;
    }

    public bool xTrack = true, yTrack = true, zTrack = true;

    public void ImmediateSet()
    {
        if (target != null)
        {
            transform.position = target.position + anchorOffset + offset;
            transform.rotation = Quaternion.LookRotation(-offset);
        }
    }

    public Vector3 RemainDspl
    {
        get
        {
            Vector3 dspl = target.position + anchorOffset + offset - transform.position;
            dspl.x = xTrack ? dspl.x : 0f;
            dspl.y = yTrack ? dspl.y : 0f;
            dspl.z = zTrack ? dspl.z : 0f;
            return dspl;
        }
    }

    void Start()
    {
        target = Player.instance.transform;
        ImmediateSet();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            target = Player.instance.transform;
        }
        else
        {
            Vector3 movement = RemainDspl;

            if (RemainDspl.magnitude > Mathf.Epsilon)
            {
                float distance = movement.magnitude * (1 - damping);
                distance = Mathf.Clamp(distance, 0.1f * Time.deltaTime, 10f * Time.deltaTime);
                distance = Mathf.Min(movement.magnitude, distance);
                movement = movement.normalized * distance;

                Vector3 position = transform.position + movement;
                transform.position = position;

                Quaternion rotation = transform.rotation;
                Quaternion lookDir = Quaternion.LookRotation(-offset);
                transform.rotation = Quaternion.Lerp(rotation, lookDir, 0.6f * Time.deltaTime);
            }
        }
    }
}
