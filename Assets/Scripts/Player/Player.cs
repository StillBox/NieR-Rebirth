using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    //Auxiliary functions and values 

    public float PositionX
    {
        get
        {
            return transform.position.x;
        }
        set
        {
            Vector3 position = transform.position;
            position.x = value;
            transform.position = position;
        }
    }

    public float PositionY
    {
        get
        {
            return transform.position.y;
        }
        set
        {
            Vector3 position = transform.position;
            position.y = value;
            transform.position = position;
        }
    }

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

    //Keep only one instance for Player
    public static Player instance = null;

    //Character Controller
    private CharacterController charaController;

    //Life Points and Parts of Player
    private int life = 3;
    [SerializeField] private GameObject core;
    [SerializeField] private GameObject fore;
    [SerializeField] private GameObject port;
    [SerializeField] private GameObject stbd;
    [SerializeField] private Armor armor;
    [SerializeField] private PlayerFlame flame;
    [SerializeField] private Wingman wingmanPrefab;
    [SerializeField] private PlayerBullet bulletPrefab;
    [SerializeField] private PlayerShield shieldPrefab;
    private Queue<PlayerBullet> bulletPool;
    private List<Wingman> wingmen;

    //Base Factor for movement and attack
    public float baseSpeed = 10f;
    private float fireGap = 0.08f;

    //Actual Factors and timers for movement and attack
    private float moveSpeed;
    private float rotSpeed = 16f;
    private float timeFireRecharge = 0f;

    //States of Player
    public static bool IsMovable = false;
    public static bool IsArmed = false;
    public static bool IsDeath = false;
    public static bool IsSuper = false;

    //Other States
    [HideInInspector] public Enemy arrow = null;
    [HideInInspector] public InteractivePoint interactivePoint = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        charaController = GetComponent<CharacterController>();
        bulletPool = new Queue<PlayerBullet>();
        wingmen = new List<Wingman>();
    }

    void Start()
    {
        IsDeath = false;
        IsSuper = false;

        moveSpeed = baseSpeed;
        flame.SetThreshold(0.2f * moveSpeed, 0.5f * moveSpeed);

        Transform bulletHolder = new GameObject("PlayerBullets").transform;
        for (int i = 0; i <= 64; i++)
        {
            PlayerBullet bullet = Instantiate(bulletPrefab).GetComponent<PlayerBullet>();
            bulletPool.Enqueue(bullet);
            bullet.transform.SetParent(bulletHolder);
            bullet.SetActive(false);
        }
    }

    void LateUpdate()
    {
        //Movement

        Vector3 movement = Vector3.zero;
        float horInput = STBInput.GetAxis("HorizontalMove");
        float verInput = STBInput.GetAxis("VerticalMove");
        
        Matrix4x4 rotCamera = Matrix4x4.Rotate(Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f));

        if (horInput != 0 || verInput != 0)
        {
            movement.x = horInput * moveSpeed;
            movement.z = verInput * moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);
            if (GameManager.IsDirectionAdjustOn)
            {
                float angle = Camera.main.AdjustAngle(transform.position, movement);
                Matrix4x4 adjust = Matrix4x4.Rotate(Quaternion.Euler(0f, angle, 0f));
                movement = adjust * movement;
            }
            movement = rotCamera * movement;
        }

        if (GameCursor.isActivated)
        {
            Vector3 rotation = GameCursor.GetPosition() - transform.position;
            rotation.y = 0f;
            if (rotation != Vector3.zero)
            {
                Quaternion direction = Quaternion.LookRotation(rotation);
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
            }
        }
        else
        {
            float horRotInput = STBInput.GetAxis("HorizontalRotate");
            float verRotInput = STBInput.GetAxis("VerticalRotate");
            if (horRotInput != 0 || verRotInput != 0)
            {
                Vector3 lookDir = new Vector3(horRotInput, 0f, verRotInput);
                if (GameManager.IsDirectionAdjustOn)
                {
                    float angle = Camera.main.AdjustAngle(transform.position, lookDir);
                    Matrix4x4 adjust = Matrix4x4.Rotate(Quaternion.Euler(0f, angle, 0f));
                    lookDir = adjust * lookDir;
                }
                lookDir = rotCamera * lookDir;
                Quaternion direction = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
            }
            else if (movement.magnitude != 0f)
            {
                Quaternion direction = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
            }
        }

        flame.CheckSpeed(movement);

        if (!charaController.isGrounded)
        {
            charaController.Move(new Vector3(0f, -9.8f, 0f) * Time.deltaTime);
        }
        movement *= Time.deltaTime;
        if (IsMovable)
        {
            charaController.Move(movement);
        }

        //Wingmen

        if (wingmen.Count != 0)
        {
            for (int i = wingmen.Count - 1; i >= 0; i--)
            {
                if (wingmen[i] == null)
                    wingmen.RemoveAt(i);
            }
        }

        //Attack

        if (IsArmed)
        {
            timeFireRecharge += Time.deltaTime;
            if (STBInput.GetButton("Fire"))
            {
                if (timeFireRecharge > fireGap) Fire();
            }
            else
            {
                timeFireRecharge = Mathf.Clamp(timeFireRecharge, 0f, fireGap);
            }
        }
    }

    public void SetWingmen(int count)
    {
        float delta = 2f * Mathf.PI / count;
        for (int i = 0; i < count; i++)
        {
            Wingman wingman = Instantiate(wingmanPrefab, transform);
            wingman.SetStartPhase(delta * i);
            wingmen.Add(wingman);
        }
    }

    public void SetFlameSpeed(Vector3 value)
    {
        flame.globalVelocity = value;
    }

    public void Fire()
    {
        timeFireRecharge = 0f;

        PlayerBullet bullet = bulletPool.Dequeue();
        bullet.SetActive(true);
        bullet.transform.rotation = transform.rotation;
        bullet.transform.position = transform.position + transform.forward * 0.25f;
        bulletPool.Enqueue(bullet);

        if (wingmen.Count != 0)
        {
            foreach (Wingman wingman in wingmen)
            {
                PlayerBullet wingBullet = bulletPool.Dequeue();
                wingBullet.SetActive(true);
                wingBullet.transform.rotation = transform.rotation;
                wingBullet.transform.position = wingman.transform.position + transform.forward * 0.25f;
                bulletPool.Enqueue(wingBullet);
            }
        }

        SoundManager.instance.PlayEfx(Efx.FIRE_PLAYER, transform.position);
    }

    public void Damage(int damagePoint = 1)
    {
        if (IsDeath) return;

        if (damagePoint == 1)
        {
            if (armor.IsArmored) return;
            armor.SetArmor();
            Instantiate(shieldPrefab, transform);
        }            
        
        if (IsSuper)
        {
            damagePoint--;
        }

        life = Mathf.Max(0, life - damagePoint);
        switch (life)
        {
            case 3:
                SoundManager.instance.PlayEfx(Efx.DAMAGE_PLAYER, transform.position);
                break;
            case 2:
                if (port != null) Destroy(port.gameObject);
                SoundManager.instance.PlayEfx(Efx.DAMAGE_PLAYER, transform.position);
                break;
            case 1:
                if (port != null) Destroy(port.gameObject);
                if (stbd != null) Destroy(stbd.gameObject);
                SoundManager.instance.PlayEfx(Efx.DAMAGE_PLAYER, transform.position);
                break;
            case 0:
                if (port != null) Destroy(port.gameObject);
                if (stbd != null) Destroy(stbd.gameObject);
                Destroy(fore.gameObject);
                Destroy(core.gameObject);
                IsMovable = false;
                IsArmed = false;
                IsDeath = true;
                if (interactivePoint != null) interactivePoint.StopActive();
                SoundManager.instance.PlayEfx(Efx.DEATH_PLAYER, transform.position);
                EffectManager.instance.Explode(transform.position, ExplosionType.LARGE);
                break;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (arrow != null)
        {
            bool damaged = false;
            if (hit.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();
                if (enemy != arrow) damaged = true;
            }
            else if (hit.gameObject.CompareTag("AirWall") ||
                hit.gameObject.CompareTag("SoftObject") ||
                hit.gameObject.CompareTag("HardObject"))
            {
                damaged = true;
            }
            if (damaged)
            {
                Damage();
                arrow.Damage();
            }
        }
    }

    //Uncontrolled movement

    public void MoveBy(Vector3 offset, float duration)
    {
        StartCoroutine(Move(transform.position + offset, duration));
    }

    public void MoveTo(Vector3 target, float duration)
    {
        StartCoroutine(Move(target, duration));
    }

    public void SmoothMoveTo(Vector3 target, float duration)
    {
        StartCoroutine(SmoothStop(target, duration));
    }

    IEnumerator Move(Vector3 target, float duration)
    {
        float time = 0f;
        Vector3 origin = transform.position;
        while (time < duration)
        {
            float rate = time / duration;
            Vector3 position = Vector3.Lerp(origin, target, rate);
            transform.position = position;
            yield return null;
            time += Time.deltaTime;
        }
        transform.position = target;
    }
    
    IEnumerator SmoothStop(Vector3 target, float duration)
    {
        float time = 0f;
        Vector3 origin = transform.position;
        while (time < duration)
        {
            float phase = 0.5f * Mathf.PI * time / duration;
            float rate = Mathf.Sin(phase);
            Vector3 position = Vector3.Lerp(origin, target, rate);
            transform.position = position;
            yield return null;
            time += Time.deltaTime;
        }
        transform.position = target;
    }

    public void PushAway(Vector3 position, float maxSpeed = 10f)
    {
        Vector3 pushDir = transform.position - position;
        pushDir.y = 0;
        pushDir.Normalize();
        StartCoroutine(Push(pushDir, 1f, maxSpeed));
    }

    IEnumerator Push(Vector3 dir, float duration, float maxSpeed = 10f)
    {
        if (IsMovable)
            StartCoroutine(LostControl(duration * 0.5f));

        float time = 0f;
        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;
            float rate = time / duration;
            float speed = maxSpeed * Mathf.Pow(1f - rate, 2f);
            charaController.Move(dir * speed * Time.deltaTime);
        }
    }

    IEnumerator LostControl(float duration)
    {
        IsMovable = false;
        yield return new WaitForSeconds(duration);
        IsMovable = true;
    }
}