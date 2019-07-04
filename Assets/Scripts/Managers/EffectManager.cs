using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance = null;

    [SerializeField] private Explosion explosionS;
    [SerializeField] private Explosion explosionL;
    [SerializeField] private Explosion explosionCube;
    [SerializeField] private Explosion explosionSoft;
    [SerializeField] private BulletExplosion explosionHard;
    [SerializeField] private Residue residue;

    [SerializeField] private LightBeam lightBeam;
    [SerializeField] private SearchLight searchLight;
    [SerializeField] private SearchLight searchLightWhite;
    [SerializeField] private WarningRegion warningRegion;

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
        DontDestroyOnLoad(gameObject);
    }
    
    public void Explode(Vector3 position, ExplosionType type)
    {
        GameObject explosion = null;
        switch (type)
        {
            case ExplosionType.SMALL:
                explosion = Instantiate(explosionS).gameObject;
                Instantiate(residue).GetComponent<Transform>().position = position;
                break;
            case ExplosionType.LARGE:
                explosion = Instantiate(explosionL).gameObject;
                break;
            case ExplosionType.FLOOR:
                explosion = Instantiate(explosionS).gameObject;
                break;
            case ExplosionType.CUBE:
                explosion = Instantiate(explosionCube).gameObject;
                break;
            case ExplosionType.SOFT:
                explosion = Instantiate(explosionSoft).gameObject;
                break;
            case ExplosionType.HARD:
                explosion = Instantiate(explosionHard).gameObject;
                break;
        }
        explosion.transform.position = position;
    }

    public void SetLightBeam(Vector3 position, bool isShowUp)
    {
        SoundManager.instance.PlayEfx(isShowUp ? Efx.ACTIVATE : Efx.ACTIVATED, position);
        Instantiate(lightBeam).GetComponent<Transform>().position = position;
    }

    public void SetSearchLight(Vector3 position, bool enemy = true)
    {
        SoundManager.instance.PlayEfx(Efx.SHOW_UP, position);
        if (enemy)
            Instantiate(searchLight).GetComponent<Transform>().position = position;
        else
            Instantiate(searchLightWhite).GetComponent<Transform>().position = position;
    }

    public void SetWarningRegion(Vector3 beginPoint, Vector3 endPoint, float duration)
    {
        WarningRegion region = Instantiate(warningRegion);
        region.BeginPoint = beginPoint;
        region.EndPoint = endPoint;
        region.Warn(duration);
    }
}

public enum ExplosionType
{
    SMALL,
    LARGE,
    FLOOR,
    CUBE,
    SOFT,
    HARD
}