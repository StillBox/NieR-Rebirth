using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEx3 : MonoBehaviour
{
    public static MapEx3 instance = null;

    public const float GRID_SIZE = 3f;
    public const int OUTER_COUNT = 3;
    public const int MIDDLE_COUNT = 2;
    public const int INNER_COUNT = 1;
    
    public bool IsMoving { get; set; }

    [SerializeField] private FloorRing[] floorRings;
    [SerializeField] private Transform floorCenter;
    [SerializeField] private Transform house;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    public void SetColUnstable(int ringIndex, int col)
    {
        floorRings[ringIndex].SetColStable(col, false);
    }

    public void SetRowUnstable(int ringIndex, int row)
    {
        floorRings[ringIndex].SetRowStable(row, false);
    }

    public void SetRingUnstable(int ringIndex)
    {
        floorRings[ringIndex].SetAllStable(false);
    }

    public void SetRingStable(int ringIndex)
    {
        floorRings[ringIndex].SetAllStable(true);
    }

    public void SetAllStable()
    {
        foreach (FloorRing ring in floorRings)
        {
            ring.SetAllStable(true);
        }
    }

    public void SetAllUnstable()
    {
        foreach (FloorRing ring in floorRings)
        {
            ring.SetAllStable(false);
        }
    }

    public void MoveIn(int ringIndex)
    {
        floorRings[ringIndex].AllMoveIn();
    }

    public void DestroyFloors()
    {
        EffectManager.instance.Explode(Vector3.zero, ExplosionType.LARGE);
        SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
        Destroy(floorCenter.gameObject);
        foreach (FloorRing ring in floorRings)
        {
            Destroy(ring.gameObject);
        }
    }

    public void MoveInHouse(float duration)
    {
        StartCoroutine(MoveHouse(duration));
    }

    IEnumerator MoveHouse(float duration)
    {
        house.gameObject.SetActive(true);

        float time = 0f;
        while (time < duration)
        {
            float rate = 1f - Mathf.Cos(0.5f * Mathf.PI * time / duration);
            float height = Mathf.Lerp(-600f, -0.25f, rate);
            house.SetLocalScaleX(rate);
            house.SetLocalScaleZ(rate);
            house.SetLocalPositionY(height);
            yield return null;
            time += Time.deltaTime;
        }
        house.SetLocalScaleX(1f);
        house.SetLocalScaleZ(1f);
        house.SetPositionY(0f);
    }
}