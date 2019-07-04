using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager instance = null;

    public static bool IsBossMode;
    public static Ladder Region;

    public Ghost ghostPrefab;
    public EnemyJunior enemyJuniorPrefab;
    public EnemySenior enemySeniorPrefab;

    private Dictionary<int, Ghost> fixedGhosts;
    private Dictionary<int, Ghost> moveInGhosts;
    private Dictionary<int, Ghost> moveOutGhosts;
    private Transform ghostsHolder;
    private float timer;

    public bool IsFixedEmpty
    {
        get { return fixedGhosts.Count == 0; }
    }

    public bool IsMoveInEmpty
    {
        get { return moveInGhosts.Count == 0; }
    }

    public bool IsMoveOutEmpty
    {
        get { return moveOutGhosts.Count == 0; }
    }

    public bool ExistGhostReadyToRebirth
    {
        get
        {
            foreach (Ghost ghost in moveInGhosts.Values)
            {
                if (ghost.ReadyToRebitrh) return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        fixedGhosts = new Dictionary<int, Ghost>();
        moveInGhosts = new Dictionary<int, Ghost>();
        moveOutGhosts = new Dictionary<int, Ghost>();
    }

    void Start()
    {
        ghostsHolder = new GameObject("Ghosts").transform;
    }

    void Update()
    {
        if (moveOutGhosts.Count > 0)
            timer += Time.deltaTime;
    }

    public void SetGhost(Vector3 position, GhostType type)
    {
        Ghost ghost = Instantiate(ghostPrefab, ghostsHolder);
        Vector3 pos = position;
        Vector2 pos2D = Region.ClosestPoint(position.XZ());
        pos.x = pos2D.x;
        pos.z = pos2D.y;
        ghost.transform.position = pos;
        ghost.type = type;
        if (IsBossMode)
        {
            ghost.Rebirth(3f);
            Add(ghost, fixedGhosts);
        }
        else
        {
            ghost.MoveOut();
            Add(ghost, moveOutGhosts);
        }
    }

    public void SetRebirthEnemy(Ghost ghost)
    {
        if (ghost.type == GhostType.JUNIOR)
        {
            EnemyJunior enemy = EnemyManager.instance.SetEnemy(enemyJuniorPrefab, ghost.rebirthPosition);
            enemy.IsBossMode = IsBossMode;
        }
        else if (ghost.type == GhostType.SENIOR)
        {
            EnemySenior enemy = EnemyManager.instance.SetEnemy(enemySeniorPrefab, ghost.rebirthPosition);
            enemy.IsBossMode = IsBossMode;
        }
    }

    public void MoveIn()
    {
        float period = 0f;
        List<Ghost> ghosts = new List<Ghost>();
        foreach (Ghost ghost in moveOutGhosts.Values)
        {
            ghosts.Add(ghost);
            if (ghost.Timer < period)
                period = ghost.Timer;
        }
        foreach (Ghost ghost in ghosts)
        {
            ghost.Timer -= period;
            ghost.MoveIn();
            Remove(ghost);
            Add(ghost, moveInGhosts);
        }
        MinimizeWaitTime();
    }

    public void MinimizeWaitTime()
    {
        if (moveInGhosts.Count == 0) return;

        float minWait = 999f;
        foreach (Ghost ghost in moveInGhosts.Values)
        {
            if (ghost.Timer < minWait)
                minWait = ghost.Timer;
        }
        if (minWait < 999f)
        {
            foreach (Ghost ghost in moveInGhosts.Values)
            {
                ghost.Timer -= minWait;
            }
        }
    }

    //Add and remove

    public void Add(Ghost ghost, Dictionary<int, Ghost> ghosts)
    {
        ghosts.Add(ghost.GetInstanceID(), ghost);
    }

    public void Remove(int id)
    {
        if (fixedGhosts.ContainsKey(id)) fixedGhosts.Remove(id);
        if (moveInGhosts.ContainsKey(id)) moveInGhosts.Remove(id);
        if (moveOutGhosts.ContainsKey(id)) moveOutGhosts.Remove(id);
    }

    public void Remove(Ghost ghost)
    {
        Remove(ghost.GetInstanceID());
    }

    public void DestroyGhosts(Dictionary<int, Ghost> ghosts)
    {
        List<int> ids = new List<int>();
        foreach (int id in ghosts.Keys)
            ids.Add(id);
        foreach (int id in ids)
            ghosts[id].Remove();
    }

    public void DestroyAll()
    {
        DestroyGhosts(fixedGhosts);
        DestroyGhosts(moveInGhosts);
        DestroyGhosts(moveOutGhosts);
    }
}