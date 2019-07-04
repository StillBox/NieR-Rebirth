using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const float MIX_OFFSET = 0.04f;

    public static SoundManager instance = null;

    public BgmSource bgmSource = new BgmSource();
    public BgmSource subBgmSource = new BgmSource();
    
    public AudioSource uiEfxSource;

    public AudioSource efxPrefab;
    public AudioSource loopEfxPrefab;
    private Queue<AudioSource> efxSources;
    private Queue<AudioSource> loopEfxSources;

    public AudioClip[] uiEfxClips;
    public AudioClip[] efxClips;

    private int[] efxCounts;
    private bool isCrossFading;
    private float reverbDegree;

    private bool isBgmLoop = false;
    private float loopPoint = 0f;
    private float loopLength = 0f;

    void Start()
    {
        efxCounts = new int[efxClips.Length];
        for (int i = 0; i < efxCounts.Length; i++)
        {
            efxCounts[i] = 0;
        }
        StartCoroutine(EfxCountDown());
    }

    private void Update()
    {
        if (isBgmLoop && IsPlaying)
        {
            if (Current >= loopPoint + loopLength)
            {
                SeekTo(Current - loopLength);
            }
        }
    }

    /************************
    * Volume Control Fields *
    ************************/

    private float efxVolume;
    private float bgmVolume;

    public float MainVolume
    {
        get
        { return AudioListener.volume; }
        set
        { AudioListener.volume = value; }
    }

    public float EfxVolume
    {
        get
        {
            return efxVolume;
        }
        set
        {
            efxVolume = value;
            uiEfxSource.volume = efxVolume;
            foreach (AudioSource source in efxSources)
            {
                source.volume = efxVolume;
            }
            foreach (AudioSource source in loopEfxSources)
            {
                source.volume = efxVolume;
            }
        }
    }

    public float BgmVolume
    {
        get
        {
            return bgmVolume;
        }
        set
        {
            bgmVolume = value;
            bgmSource.volume = bgmVolume;
            subBgmSource.volume = bgmVolume;
        }
    }

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

        efxSources = new Queue<AudioSource>();
        loopEfxSources = new Queue<AudioSource>();
    }

    /********************
    * Functions for efx *
    ********************/

    //UI EFX
    
    public void PlayUiEfx(UiEfx efx)
    {
        PlayUiEfx((int)efx);
    }

    public void PlayUiEfx(int index)
    {
        if (index < 0 || index >= uiEfxClips.Length) return;
        PlayUiEfx(uiEfxClips[index]);
    }

    public void PlayUiEfx(AudioClip clip)
    {
        uiEfxSource.PlayOneShot(clip);
    }

    //Game Efx

    AudioSource FindValidSource()
    {
        foreach (AudioSource source in efxSources)
        {
            if (!source.isPlaying)
                return source;
        }
        if (efxSources.Count < 32)
        {
            AudioSource newSource = Instantiate(efxPrefab, transform);
            efxSources.Enqueue(newSource);
            return newSource;
        }
        else
        {
            AudioSource source = efxSources.Dequeue();
            source.Stop();
            efxSources.Enqueue(source);
            return source;
        }
    }

    AudioSource FindValidLoopSource()
    {
        foreach (AudioSource source in loopEfxSources)
        {
            if (!source.isPlaying)
                return source;
        }
        if (loopEfxSources.Count < 4)
        {
            AudioSource newSource = Instantiate(loopEfxPrefab, transform);
            loopEfxSources.Enqueue(newSource);
            return newSource;
        }
        else
        {
            AudioSource source = loopEfxSources.Dequeue();
            source.Stop();
            loopEfxSources.Enqueue(source);
            return source;
        }
    }

    public AudioSource PlayEfx(Efx efx, Vector3 position)
    {
        return PlayEfx((int)efx, position);
    }

    public AudioSource PlayEfx(int index, Vector3 position)
    {
        if (index < 0 || index >= efxClips.Length) return null;
        if (efxCounts[index] > 5) return null;
        efxCounts[index]++;
        return PlayEfx(efxClips[index], position);
    }

    public AudioSource PlayEfx(AudioClip clip, Vector3 position)
    {
        AudioSource source = FindValidSource();
        source.gameObject.transform.position = position;
        source.PlayOneShot(clip);
        return source;
    }

    public AudioSource PlayLoopEfx(Efx efx, Vector3 position)
    {
        return PlayLoopEfx((int)efx, position);
    }

    public AudioSource PlayLoopEfx(int index, Vector3 position)
    {
        if (index < 0 || index >= efxClips.Length) return null;
        return PlayLoopEfx(efxClips[index], position);
    }

    public AudioSource PlayLoopEfx(AudioClip clip, Vector3 position)
    {
        AudioSource source = FindValidLoopSource();
        source.gameObject.transform.position = position;
        source.clip = clip;
        source.Play();
        return source;
    }

    public void PauseEfx()
    {
        foreach (AudioSource source in efxSources)
            source.Pause();
        foreach (AudioSource source in loopEfxSources)
            source.Pause();
    }

    public void UnPauseEfx()
    {
        foreach (AudioSource source in efxSources)
            source.UnPause();
        foreach (AudioSource source in loopEfxSources)
            source.UnPause();
    }

    IEnumerator EfxCountDown()
    {
        while (true)
        {
            for (int i = 0; i < efxCounts.Length; i++)
            {
                efxCounts[i] = Mathf.Max(0, efxCounts[i] - 1);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    /********************
    * Functions for bgm *
    ********************/

    public bool PlayBgm(AudioClip clip, float timeCrossFade = 0f)
    {
        if (isCrossFading)
            return false;

        isBgmLoop = false;
        if (timeCrossFade <= 0f)
        {
            bgmSource.Stop();
            bgmSource.volume = bgmVolume;
            bgmSource.clip = clip;
            bgmSource.time = 0f;
            bgmSource.Play();
        }
        else
        {
            StartCoroutine(CrossFadeBgm(clip, timeCrossFade));
        }

        return true;
    }

    public void SetBgmLoop(float loopPoint, float loopLength)
    {
        isBgmLoop = true;
        this.loopPoint = loopPoint;
        this.loopLength = loopLength;
    }

    public void PauseBgm()
    {
        if (bgmSource.isPlaying) bgmSource.Pause();
        if (subBgmSource.isPlaying) subBgmSource.Pause();
    }

    public void UnPauseBgm()
    {
        bgmSource.UnPause();
        subBgmSource.UnPause();
    }

    public void StopBgm(float timeFade = 0f)
    {
        if (timeFade <= 0f)
        {
            bgmSource.Stop();
            subBgmSource.Stop();
        }
        else
        {
            StartCoroutine(FadeOutBgm(timeFade));
        }
    }

    public void SetReverbBgm(bool value, float duration = 0.5f)
    {
        if (value)
            StartCoroutine(ReverbOn(duration));
        else
            StartCoroutine(ReverbOff(duration));
    }

    public bool IsPlaying
    {
        get { return bgmSource.isPlaying; }
    }

    public float Current
    {
        get { return bgmSource.time; }
    }

    public void SeekTo(float time)
    {
        bgmSource.time = time;
        if (subBgmSource.isPlaying) subBgmSource.time = time;
    }
    
    private IEnumerator CrossFadeBgm(AudioClip clip, float duration)
    {
        isCrossFading = true;

        BgmSource tmp = bgmSource;
        bgmSource = subBgmSource;
        subBgmSource = tmp;
        
        bgmSource.clip = clip;
        bgmSource.time = 0f;
        bgmSource.Play();

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t);
            subBgmSource.volume = Mathf.Lerp(bgmVolume, 0f, t);
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        bgmSource.volume = bgmVolume;
        subBgmSource.Stop();

        isCrossFading = false;
    }

    private IEnumerator FadeOutBgm(float duration)
    {
        isCrossFading = true;

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            bgmSource.volume = Mathf.Lerp(bgmVolume, 0f, t);
            subBgmSource.volume = Mathf.Lerp(bgmVolume, 0f, t);
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        bgmSource.Stop();
        subBgmSource.Stop();

        isCrossFading = false;
    }

    private IEnumerator ReverbOn(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            if (bgmSource.isPlaying) bgmSource.SetReverb(Mathf.Lerp(0f, 1f, t));
            if (subBgmSource.isPlaying) subBgmSource.SetReverb(Mathf.Lerp(0f, 1f, t));
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        if (bgmSource.isPlaying) bgmSource.SetReverb(1f);
        if (subBgmSource.isPlaying) subBgmSource.SetReverb(1f);
    }

    private IEnumerator ReverbOff(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            if (bgmSource.isPlaying) bgmSource.SetReverb(Mathf.Lerp(1f, 0f, t));
            if (subBgmSource.isPlaying) subBgmSource.SetReverb(Mathf.Lerp(1f, 0f, t));
            yield return null;
            time += Time.unscaledDeltaTime;
        }
        if (bgmSource.isPlaying) bgmSource.SetReverb(0f);
        if (subBgmSource.isPlaying) subBgmSource.SetReverb(0f);
    }

    //For Monitoring

    static bool isBgmMonitorOn = false;
    static bool isEfxMonitorOn = false;

    private void OnGUI()
    {
        if (!GameManager.isDebugModeOn) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        float heightExpand = 0f;

        if (GUI.Button(new Rect(Screen.width - 160f, 0f, 160f, 30f), "BGM Monitor"))
            isBgmMonitorOn = !isBgmMonitorOn;

        if (isBgmMonitorOn)
        {
            Rect rect = new Rect(Screen.width - 160f, 40f, 160f, 20f);
            
            GUI.Label(rect, "Time : " + string.Format("{0:N3}", Current) + (isCrossFading ? " Fading" : " Normal"), style);

            rect.y += 20f;
            if (GUI.Button(rect, "Loop Test")) SeekTo(loopPoint + loopLength - 3f);

            rect.y += 30f;
            GUI.Label(rect, "Volume : " + string.Format("{0:N1}", BgmVolume), style);
            rect.width = 20f;
            if (GUI.Button(rect, "◀")) GameManager.SetBgmVolume(BgmVolume - 0.1f);
            rect.x = Screen.width - 20f;
            if (GUI.Button(rect, "▶")) GameManager.SetBgmVolume(BgmVolume + 0.1f);

            heightExpand += 90f;
        }

        if (GUI.Button(new Rect(Screen.width - 160f, 30f + heightExpand, 160f, 30f), "SE Monitor"))
            isEfxMonitorOn = !isEfxMonitorOn;

        if (isEfxMonitorOn)
        {
            Rect rect = new Rect(Screen.width - 160f, 70f + heightExpand, 160f, 20f);
            
            for (int i = 0; i < efxCounts.Length; i++)
            {
                string info = i.ToString() + " : " + efxCounts[i].ToString();
                GUI.Label(rect, info, style);
                rect.y += 20f;
            }

            rect.y += 10f;
            GUI.Label(rect, "Volume : " + string.Format("{0:N1}", EfxVolume), style);
            rect.width = 20f;
            if (GUI.Button(rect, "◀")) GameManager.SetEfxVolume(EfxVolume - 0.1f);
            rect.x = Screen.width - 20f;
            if (GUI.Button(rect, "▶")) GameManager.SetEfxVolume(EfxVolume + 0.1f);
        }
    }
}

[System.Serializable]
public class BgmSource
{
    public AudioSource origin;
    public AudioSource reverb;

    [HideInInspector] public bool isReverbOn = false;
    [HideInInspector] public float reverbDegree = 0f;

    public void SetReverb(float value)
    {
        reverbDegree = value;
        volume = volume;
        if (reverbDegree == 0f)
        {
            reverb.Stop();
            isReverbOn = false;
        }
        else
        {
            if (!reverb.isPlaying)
            {
                reverb.time = time;
                reverb.Play();
            }
            isReverbOn = true;
        }
    }

    public float volume
    {
        get
        {
            return origin.volume + reverb.volume;
        }
        set
        {
            origin.volume = value * (1f - reverbDegree);
            reverb.volume = value * reverbDegree;
        }
    }

    public AudioClip clip
    {
        get
        {
            return origin.clip;
        }
        set
        {
            origin.clip = value;
            reverb.clip = value;
        }
    }

    public float time
    {
        get { return origin.time; }
        set { origin.time = value; }
    }

    public bool isPlaying
    {
        get { return origin.isPlaying; }
    }

    public void Play()
    {
        origin.Play();
        if (isReverbOn) reverb.Play();
    }

    public void Pause()
    {
        origin.Pause();
        if (isReverbOn) reverb.Pause();
    }

    public void UnPause()
    {
        origin.UnPause();
        if (isReverbOn) reverb.UnPause();
    }

    public void Stop()
    {
        origin.Stop();
        reverb.Stop();
    }
}

public enum Efx
{
    ACTIVATE = 0,
    ACTIVATING,
    ACTIVATED,
    DAMAGE_CUBE,
    DAMAGE_PLAYER,
    DEATH_ENEMY,
    DEATH_PLAYER,
    DESTROY_BULLET,
    DESTROY_CUBE,
    FIRE_ENEMY,
    FIRE_PLAYER,
    HIT_HARD,
    HIT_HARD_ENEMY,
    SHOW_UP,
    BREAK
}

public enum UiEfx
{
    SELECT = 0,
    DECIDE_1,
    DECIDE_2,
    DECIDE_3,
    CANCEL,
    SCENE_CHANGE,
    MESSAGE,
    TEXT,
    TICK,
    SNOW,
    CALL_9S_1,
    CALL_9S_2,
    CALL_9S_3,
    FLASH,
    TITLE_PRESS
}