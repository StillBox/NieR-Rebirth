using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractivePoint : MonoBehaviour
{
    [SerializeField] private GameObject spriteOut;
    [SerializeField] private GameObject spriteIn;
    [SerializeField] private GameObject breathingLight;
    [SerializeField] private SquareHalo halo;

    public float requiredTime = 1f;
    public bool isProgressRefresh =true;
    public bool isRepeated = false;
    public UnityEvent onActive;

    private bool isActivating = false;
    private float activeTime = 0f;
    private AudioSource activatingSource = null;
    
    public bool Activated
    {
        get; private set;
    }

    void Start()
    {
        spriteIn.SetActive(false);
        spriteOut.SetActive(true);
        breathingLight.SetActive(true);
        isActivating = false;
    }

    void Update()
    {
        if (!Activated && isActivating)
        {
            if (activeTime < requiredTime)
            {
                activeTime += Time.deltaTime;
                HUDManager.instance.SetProgress(Progress);
            }
            if (activeTime >= requiredTime)
            {
                Activated = true;
                onActive.Invoke();
                activatingSource.Stop();
                HUDManager.instance.HideProgress();
                EffectManager.instance.SetLightBeam(transform.position, false);
                if (!isRepeated)
                    Destroy(gameObject);
            }
        }
    }

    public void SetActive(bool value)
    {
        if (value)
            EffectManager.instance.SetLightBeam(transform.position, true);
        gameObject.SetActive(value);
    }

    public float ActiveTime
    {
        get { return activeTime; }
        set { activeTime = value; }
    }

    public float Progress
    {
        get { return activeTime / requiredTime; }
        set { activeTime = requiredTime * value; }
    }

    public void StopActive()
    {
        isActivating = false;
        if (activatingSource != null)
        {
            activatingSource.Stop();
        }
        HUDManager.instance.HideProgress();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCore"))
        {
            Player.instance.interactivePoint = this;
            isActivating = true;
            spriteIn.SetActive(true);
            spriteOut.SetActive(false);
            breathingLight.SetActive(false);
            halo.OnPlayerIn();
            HUDManager.instance.ShowProgress(transform);
            SoundManager.instance.PlayEfx(Efx.ACTIVATE, transform.position);
            activatingSource = SoundManager.instance.PlayLoopEfx(Efx.ACTIVATING, transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerCore"))
        {
            isActivating = false;
            spriteIn.SetActive(false);
            spriteOut.SetActive(true);
            breathingLight.SetActive(true);
            halo.OnPlayerOut();
            activatingSource.Stop();
            if (isProgressRefresh)
            {
                activeTime = 0f;
            }
            HUDManager.instance.HideProgress();
        }
    }

    private void OnDestroy()
    {
        StopActive();
    }
}