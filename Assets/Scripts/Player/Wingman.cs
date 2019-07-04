using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wingman : MonoBehaviour
{
    const float SPEED = Mathf.PI;
    const float RADIUS = 1.2f;

    private int life = 3;
    [SerializeField] private GameObject core;
    [SerializeField] private GameObject fore;
    [SerializeField] private GameObject port;
    [SerializeField] private GameObject stbd;
    [SerializeField] private Armor armor;
    [SerializeField] private PlayerShield shieldPrefab;

    float originAngle = 0f;
    float angle = 0f;
    
    private void Update()
    {
        angle += Time.deltaTime * SPEED;
        Vector3 offset = new Vector3();
        offset.x = RADIUS * Mathf.Cos(originAngle + angle);
        offset.z = RADIUS * Mathf.Sin(originAngle + angle);
        transform.localPosition = offset;
    }

    public void SetStartPhase(float value)
    {
        originAngle = value;
    }

    public void Damage()
    {
        if (armor.IsArmored) return;

        armor.SetArmor();
        Instantiate(shieldPrefab, transform);

        if (Player.IsSuper)
        {
            SoundManager.instance.PlayEfx(Efx.DAMAGE_PLAYER, transform.position);
            return;
        }

        life--;
        switch (life)
        {
            case 2:
                port.SetActive(false);
                SoundManager.instance.PlayEfx(Efx.DAMAGE_PLAYER, transform.position);
                break;
            case 1:
                stbd.SetActive(false);
                SoundManager.instance.PlayEfx(Efx.DAMAGE_PLAYER, transform.position);
                break;
            case 0:
                fore.SetActive(false);
                core.SetActive(false);
                SoundManager.instance.PlayEfx(Efx.DEATH_PLAYER, transform.position);
                EffectManager.instance.Explode(transform.position, ExplosionType.LARGE);
                Destroy(gameObject);
                break;
        }
    }
}
