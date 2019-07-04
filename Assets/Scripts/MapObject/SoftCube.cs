using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftCube : MonoBehaviour
{
    public int healthPoint = 2;

    [SerializeField] private Armor armor;
    
    public void Damage()
    {
        if (armor.IsArmored) return;

        healthPoint--;
        armor.SetArmor();

        if (healthPoint <= 0)
        {
            SoundManager.instance.PlayEfx(Efx.DESTROY_CUBE, transform.position);
            Invoke("Explode", armor.armorTime);
        }
        else
        {
            SoundManager.instance.PlayEfx(Efx.DAMAGE_CUBE, transform.position);
        }
    }

    public void Explode()
    {
        EffectManager.instance.Explode(transform.position, ExplosionType.CUBE);
        Destroy(gameObject);
    }
}