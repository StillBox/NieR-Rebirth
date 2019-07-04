using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEx1 : MonoBehaviour
{
    public Transform attachedFloor;
    public Transform attachmentPrefab;
    public AirWall topWall;
    public AirWall btmWall;
    public AirWall topWall_L;
    public AirWall btmWall_L;
    public AirWall topWall_R;
    public AirWall btmWall_R;
    public Ladder regionForward = new Ladder(5.5f, 10.5f, 10.5f, -7.5f, -7.5f);
    public Ladder regionLeft = new Ladder(5.5f, 16.5f, 12f, -16.5f, -12f);
    public Ladder regionRight = new Ladder(5.5f, 12f, 16.5f, -12f, -16.5f);
    public float globalVelocity;

    private List<Transform> attachments;
    private Transform attachmentsHolder;

    void Start()
    {
        attachments = new List<Transform>();
        attachmentsHolder = new GameObject("Attachments").transform;

        for (int i = -1; i <= 1; i += 2)
        {
            Transform preset = CreateRandomAttachment(i);
            preset.SetLocalPositionZ(5f);
            attachments.Add(preset);
        }

        StartCoroutine(AddSideAttachment(-1f));
        StartCoroutine(AddSideAttachment(1f));
    }
    
    void Update()
    {
        for (int i = attachments.Count - 1; i >= 0; i--)
        {
            attachments[i].Translate(0f, 0f, globalVelocity * Time.deltaTime);
            if (attachments[i].position.z <= -attachedFloor.localScale.z / 2f)
            {
                Destroy(attachments[i].gameObject);
                attachments.RemoveAt(i);
            }
        }
    }

    public void SetWalls(int type)
    {
        topWall.gameObject.SetActive(type == 0);
        btmWall.gameObject.SetActive(type == 0);
        topWall_L.gameObject.SetActive(type == 1);
        btmWall_L.gameObject.SetActive(type == 1);
        topWall_R.gameObject.SetActive(type == 2);
        btmWall_R.gameObject.SetActive(type == 2);
    }

    IEnumerator AddSideAttachment(float offset)
    {
        float time = 0f;
        float gap = 0f;

        while (true)
        {
            if (time  >= gap)
            {
                time -= gap;
                gap = Random.Range(4f, 12f) / Mathf.Abs(globalVelocity);
                attachments.Add(CreateRandomAttachment(offset));
            }
            yield return null;
            time += Time.deltaTime;
        }
    }

    private Transform CreateRandomAttachment(float offset)
    {
        Transform attachment = Instantiate(attachmentPrefab, attachmentsHolder);
        Vector3 scale = new Vector3(Random.Range(0.2f, 0.4f), 0.5f, Random.Range(0.8f, 1.6f));
        Vector3 position = new Vector3()
        {
            x = offset * (attachedFloor.localScale.x + scale.x) / 2f,
            y = -0.25f,
            z = attachedFloor.localScale.z / 2f
        };
        attachment.position = position;
        attachment.localScale = scale;
        return attachment;
    }
}