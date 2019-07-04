using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostType
{
    JUNIOR,
    SENIOR
}

public class Ghost : MonoBehaviour
{
    //Auxiliary functions and values 

    public float PositionZ
    {
        get
        {
            return transform.position.z;
        }
        set
        {
            Vector3 position = transform.position;
            position.z = value;
            transform.position = position;
        }
    }

    //main

    private const float SPEED = 6f;
    private const float BOUND_TOP = 15f;
    private const float BOUND_BTM = -20f;

    public GhostParticle[] prefabs;
    public GhostType type;
    public Vector3 rebirthPosition;

    private bool isActivated = true;
    private bool isMovable = false;

    private bool readyToMoveIn = false;
    private bool readyToRebirth = false;
    public bool ReadyToRebitrh
    {
        get { return readyToRebirth; }
    }

    public float Timer
    {
        set; get;
    }

    void Start()
    {
        rebirthPosition = transform.position;
        StartCoroutine(CreateGhost(GhostParticle.lifeTime / 10f));
    }
    
    void Update()
    {
        if (!isActivated) return;

        Timer -= Time.deltaTime;

        if (isMovable)
        {
            transform.Translate(0f, 0f, -SPEED * Time.deltaTime);
            if (PositionZ < BOUND_BTM) isMovable = false;
        }

        if (readyToMoveIn && Timer <= 0f)
        {
            isMovable = true;
            readyToMoveIn = false;
            Rebirth((BOUND_TOP - rebirthPosition.z) / SPEED);
        }

        if (readyToRebirth)
        {
            if (Timer <= 0)
            {
                GhostManager.instance.SetRebirthEnemy(this);
                Remove();
            }
        }
    }

    public void MoveOut()
    {
        isMovable = true;
    }

    public void MoveIn()
    {
        readyToMoveIn = true;
        Timer += (rebirthPosition.z - BOUND_BTM) / SPEED;
        PositionZ = BOUND_TOP;
    }

    public void Rebirth(float time)
    {
        readyToRebirth = true;
        Timer = time;
    }

    public void Remove()
    {
        isActivated = false;
        StartCoroutine(RemoveGhost(GhostParticle.lifeTime));
        GhostManager.instance.Remove(this);
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