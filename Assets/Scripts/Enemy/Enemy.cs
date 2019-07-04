using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    #region Auxiliary functions and values 

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

    #endregion

    //main

    public int healthPoint = 1;
    public bool isLitUp = true;
    public bool isPushable = true;
    public UnityEvent onDestroyEvent = null;
    public CharacterController controller;
    public Armor armor;

    protected bool isInBattle = false;
    virtual public bool IsInBattle
    {
        get { return isInBattle; }
        set { isInBattle = value; }
    }
    
    virtual public void SetActive(bool value)
    {
        if (value && isLitUp)
        {
            EffectManager.instance.SetSearchLight(transform.position);
            Invoke("ShowUp", 0.25f);
        }
        else
            gameObject.SetActive(value);
    }

    private void ShowUp()
    {
        gameObject.SetActive(true);
    }

    #region For Base Movement
    //Uncontrolled movement

    public void MoveBy(Vector3 offset, float duration, bool controllerMove = false)
    {
        StartCoroutine(Move(transform.position + offset, duration, controllerMove));
    }

    public void MoveTo(Vector3 position, float duration, bool controllerMove = false)
    {
        StartCoroutine(Move(position, duration, controllerMove));
    }

    IEnumerator Move(Vector3 target, float duration, bool controllerMove = false)
    {
        float time = 0f;
        Vector3 origin = transform.position;
        while (time < duration)
        {
            float rate = time / duration;
            Vector3 position = new Vector3()
            {
                x = Mathf.Lerp(origin.x, target.x, rate),
                y = Mathf.Lerp(origin.y, target.y, rate),
                z = Mathf.Lerp(origin.z, target.z, rate)
            };
            if (controllerMove && controller != null)
            {
                controller.Move(position - transform.position);
            }
            else
            {
                transform.position = position;
            }
            yield return null;
            time += Time.deltaTime;
        }
        if (controllerMove && controller != null)
        {
            controller.Move(target - transform.position);
        }
        else
        {
            transform.position = target;
        }
    }

    virtual protected void OnControllerColliderHit(ControllerColliderHit hit)
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
    }

    #endregion

    #region For Damage and Death

    virtual public void Damage(int damagePoint = 1)
    {
        if (healthPoint <= 0) return;

        if (armor != null && armor.IsArmored)
            damagePoint--;

        if (IsInBattle)
        {
            healthPoint -= damagePoint;
            if (healthPoint <= 0)
            {
                Explode();
                IsInBattle = false;
                SoundManager.instance.PlayEfx(Efx.DEATH_ENEMY, transform.position);
            }
        }

        if (armor != null && !armor.IsArmored)
        {
            armor.SetArmor();
            if (healthPoint > 0)
                SoundManager.instance.PlayEfx(Efx.DAMAGE_CUBE, transform.position);
        }
    }

    virtual protected void Explode()
    {
        EffectManager.instance.Explode(transform.position, ExplosionType.SMALL);
        OnDeath();
    }

    virtual protected void OnDeath()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.Remove(this);

        if (armor != null)
            Invoke("Destroy", armor.armorTime);
        else
            Destroy();
    }

    virtual protected void Destroy()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (healthPoint == 0 && onDestroyEvent != null)
        {
            onDestroyEvent.Invoke();
        }
    }

    #endregion
}

