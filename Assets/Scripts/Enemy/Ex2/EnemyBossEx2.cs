using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossEx2 : Enemy
{
    private const float MIN_RADIUS = 2f;
    private const float MAX_RADIUS = 11f;
    private const float ROTATE_RATE = 3f;

    [SerializeField] private Chain chain;

    [SerializeField] private FanWeapon fanWeapon;
    [SerializeField] private RingWeapon ringWeapon;
    [SerializeField] private MessWeaponEx2 messWeapon;
    [SerializeField] private BombWeaponEx2 bombWeapon;
    [SerializeField] private ArrowWeaponEx2 arrowWeapon;
    [SerializeField] private CrossWeaponEx2 crossWeapon;
    [SerializeField] private SpreadWeaponEx2 spreadWeapon;

    public int direction = 1;
    public int Phase { get; set; }
    
    private int burstLevel = 0;
    public int BurstLevel
    {
        get { return burstLevel; }
        set
        {
            if (burstLevel != value)
            {
                burstLevel = value;
                float gapRate = 1f - burstLevel * 0.2f;
                fanWeapon.SetFloatGap(false, gapRate, 1f);
                ringWeapon.SetFloatGap(false, gapRate, 1f);
                messWeapon.SetFloatGap(false, gapRate, 1f);
                bombWeapon.SetFloatGap(false, gapRate, 1f);
                arrowWeapon.SetFloatGap(false, gapRate, 1f);
                crossWeapon.SetFloatGap(false, gapRate, 1f);
                spreadWeapon.SetFloatGap(false, gapRate, 1f);
            }
        }
    }
    
    //Fuctions for action

    Angle angle = 0f;
    bool isColliderHit = false;

    public void StartPrePhase(float period, float wait)
    {
        IsInBattle = true;
        StartCoroutine(Turning());
        StartCoroutine(PrePhase(period, period * wait));
    }

    public void StartOrbitClosePhase(float period, float duration, float wait, float end)
    {
        StopAllWeapons();
        StartCoroutine(OrbitClose(period * duration, period * wait, period * end));
        if (IsInBattle) crossWeapon.StartWeapon(period * duration * 0.5f, 0f, 90f, period * 0.8f, direction == -1, period * wait);
    }

    public void StartOrbitApartPhase(float period, float duration, float wait, float end)
    {
        StopAllWeapons();
        StartCoroutine(OrbitApart(period * duration, period * wait, period * end));
        if (IsInBattle) spreadWeapon.StartWeapon((int)((wait + duration + end) / 6f - 1), period * 18f, period * 0.4f, direction == -1);
    }

    public void StartSentryPhase(float period, float duration, float wait, float end)
    {
        StopAllWeapons();
        StartCoroutine(StaySentry(period * duration, period * wait, period * end));
        if (IsInBattle)
        {
            switch (direction)
            {
                case 1:
                    arrowWeapon.StartWeapon(period * 16f, period * wait);
                    break;
                case -1:
                    bombWeapon.StartWeapon(period * 16f, period * wait);
                    break;
            }
        }
    }

    public void StartShieldPhase(float period, float duration, float wait, float end)
    {
        StopAllWeapons();
        StartCoroutine(OrbitShield(period * (duration + wait), period * end));
        if (IsInBattle) fanWeapon.StartWeapon(5, 30f, period * 0.5f, period * wait);
    }

    public void StartDashPhase(float period, float duration, int count, float wait, float end)
    {
        StopAllWeapons();
        StartCoroutine(Dash(period * duration, count, period * wait, period * end));
    }

    private IEnumerator PrePhase(float period, float wait)
    {
        yield return new WaitForSeconds(wait);
        SetTurningType(TurningType.LOOK_AT_PLAYER);

        if (IsInBattle) ringWeapon.SingleFire(12);
        SmoothMove(transform.position, new Vector3(6f * direction, 0f, 6f), period * 2f);
        yield return new WaitForSeconds(period * 8f);

        if (IsInBattle) ringWeapon.SingleFire(12);
        SmoothMove(transform.position, new Vector3(9f * direction, 0f, 3f), period * 2f);
        yield return new WaitForSeconds(period * 8f);

        if (IsInBattle) ringWeapon.SingleFire(12);
        SmoothMove(transform.position, new Vector3(6f * direction, 0f, 0f), period * 2f);
        yield return new WaitForSeconds(period * 8f);

        if (IsInBattle) ringWeapon.SingleFire(12);
        SmoothMove(transform.position, new Vector3(11f * direction, 0f, 0f), period * 2f);
        yield return new WaitForSeconds(period * 8f);
    }

    private IEnumerator OrbitClose(float duration, float wait = 0f, float end = 0f)
    {
        if (IsInBattle) ringWeapon.SingleFire(12);

        angle = 90f * direction;
        SetTurningType(TurningType.OFFSET_TO_CENTER, 90f);
        SmoothMove(transform.position, new Vector3(MAX_RADIUS * direction, 0f, 0f), wait / 2f);
        yield return new WaitForSeconds(wait);
        
        float time = 0f;
        while (time < duration)
        {
            float radius = Mathf.Lerp(MAX_RADIUS, MIN_RADIUS, time / duration);
            float speed = Mathf.PI * 100f / radius;
            MoveToPolar(radius, angle);
            yield return null;
            time += Time.deltaTime;
            angle += Time.deltaTime * speed;
        }
        time -= duration;

        float begin = angle;
        float deltaAngle = (90f * direction + 360f - (float)angle) % 360f;
        while (time < end)
        {
            angle = begin + deltaAngle * Mathf.Sin(0.5f * Mathf.PI * time / end);
            MoveToPolar(MIN_RADIUS, angle);
            yield return null;
            time += Time.deltaTime;
        }
        MoveToPolar(MIN_RADIUS, 90f * direction);
    }

    private IEnumerator OrbitApart(float duration, float wait = 0f, float end = 0f)
    {
        angle = 90f * direction;
        MoveTo(new Vector3(MAX_RADIUS * direction, 0f, 0f), wait * 0.5f, true);
        SetTurningType(TurningType.OFFSET_TO_CENTER, 180f);
        yield return new WaitForSeconds(wait);
        SetTurningType(TurningType.OFFSET_TO_CENTER, 210f);

        float time = 0f;
        while (time < duration)
        {
            float rate = time / duration;
            float speed = Mathf.PI * 60f / MAX_RADIUS * (2f - 4f * Mathf.Pow(rate - 0.5f, 2f));
            MoveToPolar(MAX_RADIUS, angle);
            yield return null;
            time += Time.deltaTime;
            angle += Time.deltaTime * speed;
        }
        time -= duration;

        float begin = angle;
        float deltaAngle = (90f * direction + 360f - (float)angle) % 360f;
        while (time < end)
        {
            angle = begin + deltaAngle * Mathf.Sin(0.5f * Mathf.PI * time / end);
            SetTurningType(TurningType.OFFSET_TO_CENTER, 210f - 30f * time / end);
            MoveToPolar(MAX_RADIUS, angle);
            yield return null;
            time += Time.deltaTime;
        }
        MoveToPolar(MAX_RADIUS, 90f * direction);
    }

    private IEnumerator OrbitShield(float duration, float end = 0f)
    {
        SetTurningType(TurningType.OFFSET_TO_CENTER);

        float time = 0f;
        while (time < duration)
        {
            MoveToPolar(MIN_RADIUS, angle);
            yield return null;
            Vector3 playerDir = Player.instance.transform.position;
            Angle playerAngle = Vector3.SignedAngle(Vector3.forward, playerDir, Vector3.up);
            Angle delta = playerAngle - angle;
            angle += delta * Time.deltaTime * ROTATE_RATE;
            time += Time.deltaTime;
        }
        time -= duration;

        fanWeapon.StopWeapon();
        SetTurningType(TurningType.OFFSET_TO_CENTER, 180f);
        Angle begin = angle;
        Angle deltaAngle = 90f * direction - angle;
        while (time < end)
        {
            angle = begin + deltaAngle * Mathf.Sin(0.5f * Mathf.PI * time / end);
            MoveToPolar(MIN_RADIUS, angle);
            yield return null;
            time += Time.deltaTime;
        }
        MoveToPolar(MIN_RADIUS, 90f * direction);
    }

    private IEnumerator StaySentry(float duration, float wait = 0f, float end = 0f)
    {
        SetTurningType(TurningType.LOOK_AT_PLAYER);

        MoveTo(Vector3.zero, wait, true);
        yield return new WaitForSeconds(wait);
        yield return new WaitForSeconds(duration);

        bombWeapon.StopWeapon();
        arrowWeapon.StopWeapon();
        SetTurningType(TurningType.OFFSET_TO_CENTER, 180f);

        Vector3 position = new Vector3(MIN_RADIUS * direction, 0f, 0f);
        SmoothMove(transform.position, position, end);
        SetTurningType(TurningType.OFFSET_TO_CENTER, 0f);
    }

    private IEnumerator Dash(float duration, int dashCount, float wait = 0f, float end = 0f)
    {
        SetTurningType(TurningType.LOOK_AT_PLAYER);
        yield return new WaitForSeconds(wait);

        int count = 0;
        while (count < dashCount)
        {
            if (direction == -1)
            {
                SetTurningType(TurningType.FIXED);
                StartCoroutine(MoveForward());
                yield return new WaitForSeconds(duration);
            }
            else if (direction == 1)
            {
                yield return new WaitForSeconds(duration * 0.5f);
                SetTurningType(TurningType.FIXED);
                StartCoroutine(MoveForward());
                yield return new WaitForSeconds(duration * 0.5f);
            }
            count++;
        }

        Vector3 position = new Vector3(MAX_RADIUS * direction, 0f, 0f);
        if (IsInBattle) ringWeapon.StartWeapon(12, 1f);
        SmoothMove(transform.position, position, end);
    }

    private void MoveToPolar(float radius, float degree)
    {
        Vector3 position = new Vector3()
        {
            x = radius * Mathf.Sin(Mathf.Deg2Rad * degree),
            y = 0f,
            z = radius * Mathf.Cos(Mathf.Deg2Rad * degree)
        };
        controller.Move(position - transform.position);
    }

    private IEnumerator MoveForward()
    {
        isColliderHit = false;
        if (IsInBattle) messWeapon.StartWeapon(3 + burstLevel * 2, 0.05f);

        float speed = 5;
        while (!isColliderHit)
        {
            controller.Move(speed * Time.deltaTime * transform.forward);
            speed += Time.deltaTime * 10f;
            yield return null;
        }
        messWeapon.StopWeapon();
        if (IsInBattle) ringWeapon.SingleFire(24);

        SetTurningType(TurningType.LOOK_AT_PLAYER);
    }

    private void SmoothMove(Vector3 beg, Vector3 end, float duration, bool smoothStart = true)
    {
        if (smoothStart)
            StartCoroutine(SmoothStartAndStop(beg, end, duration));
        else
            StartCoroutine(SmoothStop(beg, end, duration));
    }

    IEnumerator SmoothStop(Vector3 beg, Vector3 end, float duration)
    {
        float time = 0f;
        Vector3 delta = end - beg;
        float maxSpeed = delta.magnitude / duration * Mathf.PI / 2f;
        while (time < duration)
        {
            float phase = 0.5f * Mathf.PI * time / duration;
            Vector3 target = beg + delta * Mathf.Sin(phase);
            Vector3 offset = target - transform.position;
            float movement = Mathf.Min(offset.magnitude, maxSpeed * Time.deltaTime);
            offset = offset.normalized * movement;
            controller.Move(offset);
            yield return null;
            time += Time.deltaTime;
        }
        controller.Move(end - transform.position);
    }

    IEnumerator SmoothStartAndStop(Vector3 beg, Vector3 end, float duration)
    {
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
            controller.Move(offset);
            yield return null;
            time += Time.deltaTime;
        }
        controller.Move(end - transform.position);
    }

    override protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Player"))
        {
            CharacterController controller = hit.collider.GetComponent<CharacterController>();
            if (controller == null) return;

            Vector3 pushDir = new Vector3(hit.normal.x, 0f, hit.normal.z);
            Vector3 moveDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            float pushLength = hit.moveLength * Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(moveDir, pushDir));
            controller.Move(pushDir * pushLength);
        }
        else if (!hit.collider.CompareTag("Floor"))
        {
            isColliderHit = true;
        }
    }

    //Values and Functions for euler angle
    
    private enum TurningType
    {
        LOOK_AT_PLAYER,
        OFFSET_TO_CENTER,
        FIXED
    }
    private TurningType turningType = TurningType.FIXED;
    private float addAngle;

    private void SetTurningType(TurningType type, float add = 0f)
    {
        turningType = type;
        addAngle = add;
    }

    private IEnumerator Turning()
    {
        while (IsInBattle)
        {
            Vector3 target = Vector3.zero;
            switch (turningType)
            {
                case TurningType.LOOK_AT_PLAYER:
                    target = Player.instance.transform.position - transform.position;
                    break;
                case TurningType.OFFSET_TO_CENTER:
                    target = transform.position;
                    break;
                case TurningType.FIXED:
                    target = transform.forward;
                    break;
            }
            Angle deltaAngle = Vector3.SignedAngle(transform.forward, target, Vector3.up) + addAngle;
            
            transform.Rotate(0f, deltaAngle * Time.deltaTime * ROTATE_RATE * 2f, 0f);
            yield return null;
        }
    }

    //For attack

    public void StopAllWeapons()
    {
        fanWeapon.StopWeapon();
        ringWeapon.StopWeapon();
        messWeapon.StopWeapon();
        bombWeapon.StopWeapon();
        arrowWeapon.StopWeapon();
        crossWeapon.StopWeapon();
        spreadWeapon.StopWeapon();
    }

    /* Unused codes for attack
     * 
     * 
    private float timeRate = 0f;
    private float gapRate = 1f;
    
    private bool isConvergedWeaponOn = false;
    private bool isTracerWeaponOn = false;
    private bool isBombWeaponOn = false;
    private bool isMessWeaponOn = false;
    private bool isFanWeaponOn = false;
    
    public void StopRotatedWeapon() { crossWeapon.StopWeapon(); }
    public void StopConvergedWeapon() { isConvergedWeaponOn = false; }
    public void StopTracerWeapon() { isTracerWeaponOn = false; }
    public void StopBombWeapon() { isBombWeaponOn = false; }
    public void StopMessWeapon() { isMessWeaponOn = false; }
    public void StopFanWeapon() { isFanWeaponOn = false; }

    private IEnumerator RotatedWeapon(float gap, float angleBeg, float angleEnd, float duration, float wait = 0f)
    {
        bool odd = true;
        float timeRotate = 0f;
        timeRate = (3f + direction) / 4f * gapRate;
        while (isRotatedWeaponOn)
        {
            if (timeRate >= gapRate)
            {
                if (IsInBattle)
                {
                    float fireAngle = angleBeg;
                    if (timeRotate >= wait)
                    {
                        fireAngle = Mathf.Lerp(angleBeg, angleEnd, (timeRotate - wait) / duration);
                    }
                    Fire(-fireAngle, 90f, 4, odd ? BulletType.HARD : BulletType.SOFT, true);
                    SoundManager.instance.PlayEfx(9, transform.position);
                }
                odd = !odd;
                timeRate -= gapRate;
            }
            yield return null;
            timeRotate += Time.deltaTime;
            timeRate += Time.deltaTime / gap;
        }
    }

    private IEnumerator ConvergedWeapon(float gap, float duration, float wait = 0f)
    {
        bool odd = true;
        timeRate = (3f + direction) / 4f * gapRate;
        float timeConverge = 0f;
        UAngle angle = 0f;
        while (isConvergedWeaponOn)
        {
            if (timeRate >= gapRate * 0.5f)
            {
                if (IsInBattle)
                {
                    Vector3 eulerBoss = transform.eulerAngles;
                    int count = timeConverge >= duration * 2f / 3f ? 3 : timeConverge >= duration / 3f ? 2 : 1;
                    for (int i = 0; i < count; i++)
                    {
                        BulletType type = (odd && i == 0) || (!odd && i == 1) ? BulletType.SOFT : BulletType.HARD;
                        Vector3 euler = -eulerBoss + new Vector3(0f, angle - 60f * i, 0f) * (odd ? 1f : -1f);
                        EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
                    }
                    if (odd) SoundManager.instance.PlayEfx(9, transform.position);
                }
                odd = !odd;
                timeRate -= gapRate * 0.5f;
            }
            yield return null;
            timeConverge += Time.deltaTime;
            timeRate += Time.deltaTime / gap;
            angle += Time.deltaTime * 180f / duration;
        }
    }

    private IEnumerator TracerWeapon(float gap, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);
        timeRate = gapRate;
        while (isTracerWeaponOn)
        {
            if (timeRate > gapRate)
            {
                if (IsInBattle)
                {
                    SoundManager.instance.PlayEfx(13, transform.position);
                    for (int i = 0; i < 4; i++)
                    {
                        Enemy arrow = EnemyManager.instance.SetEnemy(arrowPrefab, transform.position);
                        Vector3 euler = transform.eulerAngles;
                        euler.y += 90f + 60f * i;
                        arrow.transform.eulerAngles = euler;
                        arrow.MoveBy(arrow.transform.forward * 4f, 0.5f);
                        yield return new WaitForSeconds(0.1f);
                    }
                    timeRate -= gapRate - 0.4f / gap;
                }
                else
                    timeRate -= gapRate;
            }
            yield return null;
            timeRate += Time.deltaTime / gap;
        }
    }

    private IEnumerator BombWeapon(float gap, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);
        timeRate = gapRate;
        while (isBombWeaponOn)
        {
            if (timeRate > gapRate)
            {
                if (IsInBattle)
                {
                    SoundManager.instance.PlayEfx(13, transform.position);
                    for (int i = 0; i < 2; i++)
                    {
                        Enemy bomb = EnemyManager.instance.SetEnemy(bombPrefab, transform.position);
                        bomb.MoveTo(transform.right * (2f * i - 1f) * 7f, 0.5f);
                    }
                }
                timeRate -= gapRate;
            }
            yield return null;
            timeRate += Time.deltaTime / gap;
        }
    }

    private IEnumerator MessWeapon(float gap, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);

        bool odd = true;
        while (isMessWeaponOn)
        {
            if (IsInBattle)
            {
                float angle = Random.Range(0f, 360f);
                BulletType type = odd ? BulletType.HARD : BulletType.SOFT;
                Fire(angle, 360f / (3 + burstLevel * 2), 3 + burstLevel * 2, type);
                SoundManager.instance.PlayEfx(9, transform.position);
            }
            odd = !odd;
            yield return new WaitForSeconds(gap);
        }
    }

    private IEnumerator FanWeapon(float gap, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);
        timeRate = gapRate;
        bool odd = true;
        while (isFanWeaponOn)
        {
            if (timeRate > gapRate)
            {
                if (IsInBattle)
                {
                    BulletType type = odd ? BulletType.HARD : BulletType.SOFT;
                    Fire(-60f, 30f, 5, type, true);
                    SoundManager.instance.PlayEfx(9, transform.position);
                }
                odd = !odd;
                timeRate -= gapRate;
            }
            yield return null;
            timeRate += Time.deltaTime / gap;
        }
    }

    private void Fire(float angleBeg, float angleDelta, int count, BulletType typeProto, bool alter = false)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 euler = transform.eulerAngles + new Vector3(0f, angleBeg + angleDelta * i);
            BulletType type = typeProto;
            if (alter && i % 2 == 1)
            {
                type = typeProto == BulletType.HARD ? BulletType.SOFT : BulletType.HARD;
            }
            EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
        }
    }

    private void Fire(float angleBeg, float angleDelta, int count, BulletType typeProto, BulletType typeAlter)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 euler = transform.eulerAngles + new Vector3(0f, angleBeg + angleDelta * i);
            BulletType type = i % 2 == 0 ? typeProto : typeAlter;
            EnemyBulletManager.instance.SetBullet(type, transform.position, euler);
        }
    }    
    */

    //For damage and death

    public override void Damage(int damagePoint = 1)
    {
        if (IsInBattle)
        {
            if (damagePoint >= 999)
            {
                Explode();
                IsInBattle = false;
                SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
                return;
            }

            if (chain.IsArmored) return;

            chain.SetArmor();
            SoundManager.instance.PlayEfx(Efx.DAMAGE_CUBE, transform.position);

            if (healthPoint > 0)
                healthPoint -= damagePoint;
        }
    }

    protected override void Explode()
    {
        EffectManager.instance.Explode(transform.position, ExplosionType.LARGE);
        Destroy(chain.gameObject);
        StopAllWeapons();
    }
}