using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossEx3 : Enemy
{
    private const int MAX_COL_OFFSET = 12;
    private const int MAX_ROW_OFFSET = 10;
    private const int FIRE_COL_OFFSET = 5;
    private const int FIRE_ROW_OFFSET = 4;

    public int id = 0;
    
    [SerializeField] private GameObject body;
    [SerializeField] private BossShield shield;
    [SerializeField] private EmilShield dashShield;

    [SerializeField] private FanWeapon fanWeapon;
    [SerializeField] private SideWeapon sideWeapon;
    [SerializeField] private TripleRingWeapon ringWeapon;
    [SerializeField] private ConvergedWeapon convergedWeapon;
    [SerializeField] private ForkedWeapon forkedWeapon;
    private BaseWeapon currentWeapon;
    
    public override bool IsInBattle
    {
        get { return isInBattle; }
        set
        {
            isInBattle = value;
            shield.gameObject.SetActive(!value);
            dashShield.gameObject.SetActive(value);
            controller.radius = value ? 0.4f : 0.5f;
        }
    }

    private bool isOrbiting = false;
    private bool isOrbitOver = false;
    public bool IsOrbitOver
    {
        get { return isOrbitOver; }
    }

    public bool IsAbleToBreak
    {
        get
        {
            if (!IsInBattle || dashShield == null)
                return false;
            else
                return dashShield.IsCharged;
        }
    }

    private float speed = 0f;
    private Material material;

    private void Awake()
    {
        material = body.GetComponent<MeshRenderer>().material;
    }
    
    void Start ()
    {
        IsInBattle = true;
	}
	
    public override void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    //Functions for movement

    private const float MAX_DASH_SPEED = 15f;
    private const float ORBIT_SPEED = 5f;
    private const float GRID_ORBIT_TIME = MapEx3.GRID_SIZE / ORBIT_SPEED;

    //Orbit
    public void StartOrbit(int colOffset, int rowOffset, int roundCount, bool moveOut)
    {
        isOrbiting = true;
        isOrbitOver = false;
        StartCoroutine(Orbit(colOffset, rowOffset, roundCount, moveOut));
    }

    public void StopOrbit()
    {
        isOrbiting = false;
        fanWeapon.StopWeapon();
    }

    IEnumerator Orbit(int colOffset, int rowOffset, int roundCount, bool moveOut)
    {
        if (IsInBattle)
        {
            currentWeapon = fanWeapon;
            fanWeapon.SetRotation(90f, 0f);
            fanWeapon.SetFloatAngle(true, -20f, 30f);
            fanWeapon.StartWeapon(4, 30f, 1.44f, 5f + 0.64f * id);
        }

        float time = -id * 0.2f;
        if (speed <= Mathf.Epsilon) time -= id * 0.5f;
        while (time < 0f)
        {
            yield return null;
            time += Time.deltaTime;
        }
        if (speed <= Mathf.Epsilon) time += id * 0.5f;

        if (IsInBattle)
        {
            if (moveOut)
                dashShield.Expand(GetOrbitPeriod(colOffset, rowOffset) * 2f);
            else
                dashShield.Shrink(GetOrbitPeriod(colOffset, rowOffset) * 2f);
        }

        while (isOrbiting)
        {
            if (time >= (roundCount + (9f - id) / 9f) * GetOrbitPeriod(colOffset, rowOffset))
            {
                if (!isOrbitOver)
                {
                    isOrbitOver = true;
                    fanWeapon.StopWeapon();
                }
                if (!moveOut)
                {
                    StartCoroutine(SmoothStop(2f));
                    yield break;
                }
            }

            float rate = time / GetOrbitPeriod(colOffset, rowOffset);
            float accel = moveOut ?
                Mathf.Lerp(1f, 1.6f, Mathf.Clamp(rate / 2f, 0f, 1f)) :
                Mathf.Lerp(1.6f, 1f, Mathf.Clamp(rate / 2f, 0f, 1f));

            Vector3 target = GetOrbitPosition(colOffset, rowOffset, time, roundCount, moveOut);
            Vector3 offset = target - transform.position;
            speed = Mathf.Min(speed + 8f * Time.deltaTime, ORBIT_SPEED * accel);
            float maxMovement = (2f - id * 0.05f) * speed * Time.deltaTime;
            if (offset.magnitude > maxMovement)
                offset = offset.normalized * maxMovement;
            controller.Move(offset);
            if (offset.magnitude > Mathf.Epsilon)
                transform.rotation = Quaternion.LookRotation(offset);
            yield return null;
            time += Time.deltaTime * accel;
        }
    }

    private float GetOrbitPhaseTime(int colOffset, int rowOffset, int phase)
    {
        float grids = 0f;
        switch (phase)
        {
            case 0: grids = 2f * colOffset; break;
            case 1: grids = 0.5f * Mathf.PI; break;
            case 2: grids = 2f * rowOffset; break;
            case 3: grids = 0.5f * Mathf.PI; break;
            case 4: grids = 2f * colOffset; break;
            case 5: grids = 0.5f * Mathf.PI; break;
            case 6: grids = 2f * rowOffset; break;
            case 7: grids = 0.5f * Mathf.PI; break;
        }        
        return grids * MapEx3.GRID_SIZE / ORBIT_SPEED;
    }

    private float GetOrbitPhaseEndTime(int colOffset, int rowOffset, int phase)
    {
        float time = 0f;
        for (int i = 0; i <= phase; i++)
        {
            time += GetOrbitPhaseTime(colOffset, rowOffset, i);
        }
        return time;
    }

    private float GetOrbitPeriod(int colOffset, int rowOffset)
    {
        return GetOrbitPhaseEndTime(colOffset, rowOffset, 7);
    }

    private Vector3 GetOrbitPosition(int colOffset, int rowOffset, float time, int maxRound, bool moveOut)
    {
        int phase = 0;
        int round = 0;
        float phaseTime = Mathf.Max(0f, time);
        Vector3 position = Vector3.zero;

        while (phaseTime >= GetOrbitPhaseTime(colOffset, rowOffset, phase))
        {
            phaseTime -= GetOrbitPhaseTime(colOffset, rowOffset, phase);
            phase++;
            if (phase >= 8)
            {
                phase -= 8;
                round++;
            }
            if (moveOut && round >= maxRound && phase >= 4)
                break;
        }

        float rate = Mathf.Clamp(time / GetOrbitPeriod(colOffset, rowOffset) / maxRound, 0f, 1f);
        if (!moveOut) rate = 1f - rate;

        float offset = 1f + (0.5f + Mathf.Sin(Mathf.PI * time)) / 4f * (1f - rate);
        float rateCol = 1f + Mathf.Lerp(0f, 1f / (colOffset + 1f), rate);
        float rateRow = 1f + Mathf.Lerp(0f, 1f / (rowOffset + 1f), rate);

        switch (phase)
        {
            case 0:
                position.x = ORBIT_SPEED * phaseTime - colOffset * MapEx3.GRID_SIZE;
                position.z = (rowOffset + offset) * MapEx3.GRID_SIZE;
                break;
            case 1:
                position.x = (colOffset + offset * Mathf.Sin(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                position.z = (rowOffset + offset * Mathf.Cos(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                break;
            case 2:
                position.x = (colOffset + offset) * MapEx3.GRID_SIZE;
                position.z = rowOffset * MapEx3.GRID_SIZE - ORBIT_SPEED * phaseTime;
                break;
            case 3:
                position.x = (colOffset + offset * Mathf.Cos(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                position.z = -(rowOffset + offset * Mathf.Sin(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                break;
            case 4:
                position.x = colOffset * MapEx3.GRID_SIZE - ORBIT_SPEED * phaseTime;
                position.z = -(rowOffset + offset) * MapEx3.GRID_SIZE;
                break;
            case 5:
                position.x = -(colOffset + offset * Mathf.Sin(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                position.z = -(rowOffset + offset * Mathf.Cos(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                break;
            case 6:
                position.x = -(colOffset + offset) * MapEx3.GRID_SIZE;
                position.z = ORBIT_SPEED * phaseTime - rowOffset * MapEx3.GRID_SIZE;
                break;
            case 7:
                position.x = -(colOffset + offset * Mathf.Cos(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                position.z = (rowOffset + offset * Mathf.Sin(phaseTime / GRID_ORBIT_TIME)) * MapEx3.GRID_SIZE;
                break;
        }
        position.x *= rateCol;
        position.z *= rateRow;

        return position;
    }

    //Dash

    public void StartGridColDash(int col, bool positive, float wait, float duration)
    {
        Vector3 beg = new Vector3(col, 0f, (positive ? -1 : 1) * MAX_ROW_OFFSET) * MapEx3.GRID_SIZE;
        Vector3 end = new Vector3(col, 0f, (positive ? 1 : -1) * MAX_ROW_OFFSET) * MapEx3.GRID_SIZE;
        StartCoroutine(NodeDash(beg, end, wait, duration));
    }

    public void StartGridRowDash(int row, bool positive, float wait, float duration)
    {
        Vector3 beg = new Vector3((positive ? -1 : 1) * MAX_COL_OFFSET, 0f, row) * MapEx3.GRID_SIZE;
        Vector3 end = new Vector3((positive ? 1 : -1) * MAX_COL_OFFSET, 0f, row) * MapEx3.GRID_SIZE;
        StartCoroutine(NodeDash(beg, end, wait, duration));
    }

    IEnumerator NodeDash(Vector3 beg, Vector3 end, float wait, float duration)
    {
        float time = -0.1f * id;
        while (time < wait)
        {
            controller.Move(transform.forward * speed * Time.deltaTime);
            yield return null;
            time += Time.deltaTime;
        }
        time -= wait;
        
        transform.position = beg;
        Vector3 direction = end - beg;
        speed = direction.magnitude / duration;
        transform.rotation = Quaternion.LookRotation(direction);
        while (time < duration)
        {
            if (time > 0.2f * duration && !sideWeapon.IsExcuting)
            {
                if (IsInBattle)
                {
                    currentWeapon = sideWeapon;
                    sideWeapon.SetFloatGap(true, 0.5f, 1.5f);
                    sideWeapon.StartWeapon(0.15f, Random.Range(0f, 0.3f));
                }      
            }
            if (time > 0.8f * duration && sideWeapon.IsExcuting)
            {
                sideWeapon.StopWeapon();
            }
            Vector3 target = beg + direction.normalized * speed * time;
            Vector3 offset = target - transform.position;
            controller.Move(offset);
            yield return null;
            time += Time.deltaTime;
        }
        transform.position = end;
    }

    /* Unused codes for movement
     * 
    IEnumerator GridColDash(int col, bool positive, float wait, float duration)
    {
        yield return new WaitForSeconds(0.1f * id);

        Vector3 begin = new Vector3(col, 0f, (positive ? -1 : 1) * MAX_ROW_OFFSET) * MapEx3.GRID_SIZE;
        Vector3 end = new Vector3(col, 0f, (positive ? 1 : -1) * MAX_ROW_OFFSET) * MapEx3.GRID_SIZE;
        Vector3 relay = new Vector3(transform.position.x, 0f, begin.z);

        float lengthToRelay = (relay - transform.position).magnitude;
        float lengthToBegin = (begin - relay).magnitude;
        float timeToRelay = wait * lengthToRelay / (lengthToRelay + lengthToBegin);
        float timeToBegin = wait * lengthToBegin / (lengthToRelay + lengthToBegin);

        MoveTo(relay, timeToRelay);
        yield return new WaitForSeconds(timeToRelay);
        MoveTo(begin, timeToBegin);
        yield return new WaitForSeconds(timeToBegin);
        MoveTo(end, duration);
    }

    IEnumerator GridRowDash(int row, bool positive, float wait, float duration)
    {
        yield return new WaitForSeconds(0.1f * id);

        Vector3 begin = new Vector3((positive ? -1 : 1) * MAX_COL_OFFSET, 0f, row) * MapEx3.GRID_SIZE;
        Vector3 end = new Vector3((positive ? 1 : -1) * MAX_COL_OFFSET, 0f, row) * MapEx3.GRID_SIZE;
        Vector3 relay = new Vector3(begin.x, 0f, transform.position.z);

        float lengthToRelay = (relay - transform.position).magnitude;
        float lengthToBegin = (begin - relay).magnitude;
        float timeToRelay = wait * lengthToRelay / (lengthToRelay + lengthToBegin);
        float timeToBegin = wait * lengthToBegin / (lengthToRelay + lengthToBegin);

        MoveTo(relay, timeToRelay);
        yield return new WaitForSeconds(timeToRelay);
        MoveTo(begin, timeToBegin);
        yield return new WaitForSeconds(timeToBegin);
        MoveTo(end, duration);
    }
    
    IEnumerator MoveByNodes(List<Vector2Int> nodes)
    {
        foreach (Vector2Int node in nodes)
        {
            Vector3 target = new Vector3(node.x, 0f, node.y) * MapEx3.GRID_SIZE;
            Vector3 offset = target - transform.position;
            float duration = offset.magnitude / MAX_DASH_SPEED;
            MoveTo(target, duration);
            yield return new WaitForSeconds(duration);
        }
    }
    */

    //Land

    private const float LAND_SPEED = 4f;

    public void StartLand(float duration, int roundCount)
    {
        StartCoroutine(Land(duration, roundCount));
    }

    IEnumerator Land(float duration, int roundCount)
    {
        float period = duration / roundCount;
        float time = 0f;
        
        while (time < duration)
        {
            float progress = time / period;
            Vector3 target = GetLandPosition(progress + id / 9f);
            Vector3 offset = target - transform.position;
            speed = Mathf.Min(speed + 8f * Time.deltaTime, LAND_SPEED);
            float maxMovement = 1.2f * speed * Time.deltaTime;
            if (offset.magnitude > maxMovement)
                offset = offset.normalized * maxMovement;
            controller.Move(offset);
            if (offset.magnitude > Mathf.Epsilon)
                transform.rotation = Quaternion.LookRotation(offset);
            yield return null;
            time += Time.deltaTime;
        }
        controller.Move(GetLandPosition(id / 9f) - transform.position);
    }
    
    private Vector3 GetLandPosition(float progress)
    {
        Vector3 position = Vector3.zero;
        float phaseProgress = progress % 1 * 8f;
        int phase = (int)Mathf.Floor(phaseProgress);
        phaseProgress -= phase;

        switch (phase)
        {
            case 0:
                position.x = 1.5f * phaseProgress;
                position.z = 0.5f * Mathf.Sqrt(3f) * phaseProgress;
                break;
            case 1:
                position.x = 2f + Mathf.Cos(Mathf.PI * 2f / 3f * (1f - phaseProgress));
                position.z = Mathf.Sin(Mathf.PI * 2f / 3f * (1f - phaseProgress));
                break;
            case 2:
                position.x = 2f + Mathf.Cos(Mathf.PI * 2f / 3f * phaseProgress);
                position.z = -Mathf.Sin(Mathf.PI * 2f / 3f * phaseProgress);
                break;
            case 3:
                position.x = 1.5f * (1f - phaseProgress);
                position.z = -0.5f * Mathf.Sqrt(3f) * (1f - phaseProgress);
                break;
            case 4:
                position.x = -1.5f * phaseProgress;
                position.z = 0.5f * Mathf.Sqrt(3f) * phaseProgress;
                break;
            case 5:
                position.x = -2f - Mathf.Cos(Mathf.PI * 2f / 3f * (1f - phaseProgress));
                position.z = Mathf.Sin(Mathf.PI * 2f / 3f * (1f - phaseProgress));
                break;
            case 6:
                position.x = -2f - Mathf.Cos(Mathf.PI * 2f / 3f * phaseProgress);
                position.z = -Mathf.Sin(Mathf.PI * 2f / 3f * phaseProgress);
                break;
            case 7:
                position.x = -1.5f * (1f - phaseProgress);
                position.z = -0.5f * Mathf.Sqrt(3f) * (1f - phaseProgress);
                break;
        }
        position.z += 1f;
        position *= MapEx3.GRID_SIZE;

        return position;
    }

    //RandomMove

    public void Stop()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothStop(3f));
    }

    public void StartRandomMove(float duration, int count = 1)
    {
        StartCoroutine(RandomMove(duration, count));
    }

    IEnumerator RandomMove(float duration, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(-1f, 1f) * Mathf.PI;
            Vector3 delta = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * 6f;
            Vector3 beg = transform.position;
            Vector3 end = beg + delta;
            end.x = Mathf.Clamp(end.x, -12f, 12f);
            end.z = Mathf.Clamp(end.z, -9f, 9f);
            delta = end - beg;

            StartCoroutine(SmoothMove(beg, end, 0.6f * duration / count));
            yield return new WaitForSeconds(duration / count);
        }       
    }

    IEnumerator SmoothMove(Vector3 beg, Vector3 end, float duration)
    {
        speed = 0f;
        float time = 0f;
        Vector3 delta = end - beg;
        float maxSpeed = delta.magnitude / duration * Mathf.PI / 2f;
        while (time < duration)
        {
            float phase = Mathf.PI * time / duration;
            Vector3 target = beg + delta * (1f - Mathf.Cos(phase)) / 2f;
            Vector3 offset = target - transform.position;
            float movement = Mathf.Min(offset.magnitude, maxSpeed * Time.deltaTime);
            offset = offset.normalized * movement;
            speed = movement / Time.deltaTime;
            controller.Move(offset);
            yield return null;
            time += Time.deltaTime;
        }
        controller.Move(end - transform.position);
        speed = 0f;
    }

    IEnumerator SmoothStop(float duration)
    {
        float time = duration;
        while (time  > 0f)
        {
            float rate = time / duration;
            controller.Move(transform.forward * speed * rate * Time.deltaTime);
            yield return null;
            time -= Time.deltaTime;
        }
        speed = 0f;
    }

    //Functions for attack

    public void StartRingWeapon(float duration)
    {
        if (!IsInBattle) return;
        transform.LookAt(Player.instance.transform);

        currentWeapon = ringWeapon;
        ringWeapon.SetType(BulletType.HARD, true);
        ringWeapon.SetRotation(0f, 20f);
        ringWeapon.StartWeapon(18, 0.5f);
    }

    public void StartForkedWeapon(float duration)
    {
        if (!IsInBattle) return;
        transform.LookAt(Player.instance.transform);

        currentWeapon = forkedWeapon;
        forkedWeapon.SetType(BulletType.HARD, false);
        forkedWeapon.StartWeapon(duration, 0.15f);
    }

    public void StartConvergedWeapon(float duration)
    {
        if (!IsInBattle) return;
        transform.LookAt(Player.instance.transform);

        currentWeapon = convergedWeapon;
        convergedWeapon.SetType(BulletType.HARD, false);
        convergedWeapon.StartWeapon(duration, 0.15f);
    }

    /* Unused codes for attack
     * 
    private bool isMessWeaponOn = false;
    IEnumerator MessWeapon(float gap, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);
        isMessWeaponOn = true;
        while (isMessWeaponOn)
        {
            if (IsInBattle)
            {
                Vector3 playerDir = Player.instance.transform.position - transform.position;
                float angle = Vector3.SignedAngle(transform.forward, playerDir, Vector3.up);
                BulletType type = Random.Range(0, 2) == 0 ? BulletType.HARD : BulletType.SOFT;
                Fire(angle - 30f + Random.Range(-45f, 45f), 30f, 3, type, true);
                Fire(angle - 180f, 0f, 1, type);
                SoundManager.instance.PlayEfx(9, transform.position);
            }
            yield return new WaitForSeconds(gap);
        }
    }
    private bool isSideWeaponOn = false;
    IEnumerator SidedWeapon(float gap, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);
        isSideWeaponOn = true;
        while (isSideWeaponOn)
        {
            if (IsInBattle)
            {
                if (PositionX >= -FIRE_COL_OFFSET * MapEx3.GRID_SIZE && PositionX <= FIRE_COL_OFFSET * MapEx3.GRID_SIZE &&
                    PositionZ >= -FIRE_ROW_OFFSET * MapEx3.GRID_SIZE && PositionZ <= FIRE_ROW_OFFSET * MapEx3.GRID_SIZE)
                {
                    BulletType type = Random.Range(0, 2) == 0 ? BulletType.HARD : BulletType.SOFT;
                    Fire(90f, 180f, 2, type, true);
                    SoundManager.instance.PlayEfx(9, transform.position);
                }
            }
            yield return new WaitForSeconds(gap * Random.Range(0.8f, 1.5f));
        }
    }    
    */

    //For damage and death

    public void Burst()
    {
        EffectManager.instance.Explode(transform.position, ExplosionType.LARGE);
        Destroy(gameObject);
    }

    public override void Damage(int damagePoint = 1)
    {
        if (IsInBattle)
            base.Damage(damagePoint);
        else
            SoundManager.instance.PlayEfx(Efx.HIT_HARD_ENEMY, transform.position);
    }

    protected override void Explode()
    {
        EffectManager.instance.Explode(transform.position, ExplosionType.LARGE);
        material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1f));
        if (currentWeapon != null) currentWeapon.StopWeapon();
    }
}
