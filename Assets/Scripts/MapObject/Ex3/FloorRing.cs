using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorRing : MonoBehaviour
{
    public FloatingFloor floorPrefab;
    public float width = 3f;
    public int colOffset = 2;
    public int rowOffset = 1;
    private Dictionary<int, FloatingFloor> floors;

    private void OnDrawGizmos()
    {
        for (int c = -colOffset; c <= colOffset; c++)
        {
            for (int r = -rowOffset; r <= rowOffset; r++)
            {
                if (Mathf.Abs(c) == colOffset || Mathf.Abs(r) == rowOffset)
                {
                    float x = c * width;
                    float z = r * width;
                    Gizmos.color = new Color(0.9f, 0.9f, 0.8f, 1f);
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(new Vector3(x, -0.25f, z), new Vector3(width, 0.5f, width));
                }
            }
        }
    }

    private void Awake()
    {
        floors = new Dictionary<int, FloatingFloor>();
    }

    void Start()
    {
        for (int c = -colOffset; c <= colOffset; c++)
        {
            for (int r = -rowOffset; r <= rowOffset; r++)
            {
                if (Mathf.Abs(c) == colOffset || Mathf.Abs(r) == rowOffset)
                {
                    Vector3 position = new Vector3(c * width, 0f, r * width);
                    FloatingFloor floor = Instantiate(floorPrefab, position, Quaternion.identity, transform);
                    floor.SetWidth(width);
                    int key = c * 100 + r;
                    floors.Add(key, floor);
                }
            }
        }
    }

    //For Stable

    public void SetStable(int col, int row, bool value)
    {
        int key = col * 100 + row;
        if (floors.ContainsKey(key))
        {
            floors[key].IsStable = value;
        }
    }

    public void SetAllStable(bool value)
    {
        foreach (int key in floors.Keys)
        {
            floors[key].IsStable = value;
        }
    }

    public void SetColStable(int col, bool value)
    {
        SetStable(col, -rowOffset, value);
        SetStable(col, rowOffset, value);
    }

    public void SetRowStable(int row, bool value)
    {
        SetStable(-colOffset, row, value);
        SetStable(colOffset, row, value);
    }

    //For floors Moving in and out

    public void MoveUp(int col, int row)
    {
        int key = col * 100 + row;
        if (floors.ContainsKey(key))
        {
            floors[key].MoveUp();
        }
    }

    public void MoveIn(int col, int row)
    {
        int key = col * 100 + row;
        if (floors.ContainsKey(key))
        {
            floors[key].MoveIn();
        }
    }

    public void MoveOutRow(int row, float gap, bool positive = true)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        list.Add(new Vector2Int((positive ? -1 : 1) * colOffset, row));
        list.Add(new Vector2Int((positive ? 1 : -1) * colOffset, row));
        StartCoroutine(MoveOutList(list, gap));
    }

    public void MoveOutCol(int col, float gap, bool positive = true)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        list.Add(new Vector2Int(col, (positive ? -1 : 1) * rowOffset));
        list.Add(new Vector2Int(col, (positive ? 1 : -1) * rowOffset));
        StartCoroutine(MoveOutList(list, gap));
    }

    public void MoveInRow(int row, float gap, bool positive = true)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        list.Add(new Vector2Int((positive ? -1 : 1) * colOffset, row));
        list.Add(new Vector2Int((positive ? 1 : -1) * colOffset, row));
        StartCoroutine(MoveInList(list, gap));
    }

    public void MoveInCol(int col, float gap, bool positive = true)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        list.Add(new Vector2Int(col, (positive ? -1 : 1) * rowOffset));
        list.Add(new Vector2Int(col, (positive ? 1 : -1) * rowOffset));
        StartCoroutine(MoveInList(list, gap));
    }

    public void AllMoveOut()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int col = -colOffset + 1; col <= colOffset; col++) list.Add(new Vector2Int(col, rowOffset));
        for (int row = rowOffset - 1; row >= -rowOffset; row--) list.Add(new Vector2Int(colOffset, row));
        for (int col = colOffset - 1; col >= -colOffset; col--) list.Add(new Vector2Int(col, -rowOffset));
        for (int row = -rowOffset + 1; row <= rowOffset; row++) list.Add(new Vector2Int(-colOffset, row));
        StartCoroutine(MoveOutList(list, 0.1f));
    }

    public void AllMoveIn()
    {
        SetAllStable(true);
        List<Vector2Int> list = new List<Vector2Int>();
        for (int col = -colOffset + 1; col <= colOffset; col++) list.Add(new Vector2Int(col, rowOffset));
        for (int row = rowOffset - 1; row >= -rowOffset; row--) list.Add(new Vector2Int(colOffset, row));
        for (int col = colOffset - 1; col >= -colOffset; col--) list.Add(new Vector2Int(col, -rowOffset));
        for (int row = -rowOffset + 1; row <= rowOffset; row++) list.Add(new Vector2Int(-colOffset, row));
        StartCoroutine(MoveInList(list, 0.1f));
    }

    IEnumerator MoveOutList(List<Vector2Int> list, float gap)
    {
        foreach (Vector2Int v in list)
        {
            MoveUp(v.x, v.y);
            yield return new WaitForSeconds(gap);
        }
    }

    IEnumerator MoveInList(List<Vector2Int> list, float gap)
    {
        foreach (Vector2Int v in list)
        {
            MoveIn(v.x, v.y);
            yield return new WaitForSeconds(gap);
        }
    }
}
