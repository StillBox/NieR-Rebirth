using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance = null;

    [SerializeField] private HUDTimer hudTimer;
    [SerializeField] private MessageBox messageBox;
    [SerializeField] private HUDProgress hudProgress;
    [SerializeField] private HPBar hpBarPrefab;

    [HideInInspector] public bool isShowingMessage = false;

    private Camera renderCamera;
    private Vector2 canvasScale;
    private Queue<Message> messageQueue;
    private Dictionary<int, HPBar> hpBars;

    private float cursorTimer = 0f;

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

        messageQueue = new Queue<Message>();
        hpBars = new Dictionary<int, HPBar>();
    }

    void Start()
    {
        hudTimer.SetActive(false);
        messageBox.SetActive(false);
        hudProgress.SetActive(false);

        canvasScale = GetComponent<CanvasScaler>().referenceResolution;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0)
        {
            cursorTimer = 3f;
        }
        if (Cursor.visible)
        {
            if (cursorTimer <= 0 || GameCursor.isActivated) Cursor.visible = false;
            else cursorTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            if (cursorTimer > 0f && !GameCursor.isActivated) Cursor.visible = true;
        }

        if (renderCamera == null) return;

        if (hudTimer.IsActive)
        {
            if (hudTimer.anchor != null)
            {
                Vector3 screenPoint = renderCamera.WorldToScreenPoint(hudTimer.anchor.position);
                Vector2 relative = new Vector2
                {
                    x = screenPoint.x / Screen.width - 0.5f,
                    y = screenPoint.y / Screen.height - 0.5f
                };
                Vector2 canvasPosition = new Vector2
                {
                    x = relative.x * canvasScale.x + 120f,
                    y = relative.y * canvasScale.y - 10f
                };
                hudTimer.SetPosition(canvasPosition);
            }
            else
            {
                hudTimer.SetPosition(new Vector2(120f, -10f));
            }
        }

        if (hudProgress.IsActive)
        {
            if (hudProgress.anchor != null)
            {
                Vector3 screenPoint = renderCamera.WorldToScreenPoint(hudProgress.anchor.position);
                Vector2 relative = new Vector2
                {
                    x = screenPoint.x / Screen.width - 0.5f,
                    y = screenPoint.y / Screen.height - 0.5f
                };
                Vector2 canvasPosition = new Vector2
                {
                    x = relative.x * canvasScale.x,
                    y = relative.y * canvasScale.y
                };
                hudProgress.SetPosition(canvasPosition);
            }
            else
                hudProgress.SetPosition(Vector2.zero);
        }

        foreach (HPBar hpBar in hpBars.Values)
        {
            if (hpBar.anchor != null)
            {
                Vector3 screenPoint = renderCamera.WorldToScreenPoint(hpBar.anchor.position + new Vector3(0f, 0.5f, 0f));
                Vector2 relative = new Vector2
                {
                    x = screenPoint.x / Screen.width,
                    y = screenPoint.y / Screen.height
                };
                
                if (!hpBar.IsAutoFaded && hpBar.hp > 0f)
                {
                    if (relative.x < -0.1f || relative.x > 1.1f || relative.y < -0.2f || relative.y > 1.1f)
                    {
                        hpBar.FadeOut(0.2f);
                    }
                    else
                    {
                        hpBar.FadeIn(0.2f);
                    }
                }

                relative.x = Mathf.Clamp(relative.x, 0.05f, 0.95f);
                relative.y = Mathf.Clamp(relative.y, 0f, 0.9f);
                hpBar.SetPosition(relative + hpBar.offset);
            }
            else
            {
                hpBar.SetPosition(hpBar.offset);
            }
        }
    }

    public void ResetAll()
    {
        StopAllCoroutines();

        isShowingMessage = false;
        messageBox.SetActive(false);
        messageQueue = new Queue<Message>();

        hudTimer.SetActive(false);

        hudProgress.SetActive(false);

        List<int> keys = new List<int>();
        foreach (int key in hpBars.Keys)
        {
            keys.Add(key);
        }
        foreach (int key in keys)
        {
            RemoveHPBar(key);
        }
    }

    public void SetCamera(Camera renderCamera)
    {
        this.renderCamera = renderCamera;
        GetComponent<Canvas>().worldCamera = renderCamera;
        GetComponent<Canvas>().planeDistance = 1f;
    }
    
    //Functions for message box

    public void ShowMessage(Message message)
    {
        if (isShowingMessage) messageQueue.Enqueue(message);
        else StartCoroutine(ShowMessageBox(message));
    }

    public void ShowMessages(Message[] messages)
    {
        foreach (Message message in messages) messageQueue.Enqueue(message);
        if (!isShowingMessage) StartCoroutine(ShowMessageBox(messageQueue.Dequeue()));
    }
    
    IEnumerator ShowMessageBox(Message message)
    {
        isShowingMessage = true;
        yield return new WaitForSeconds(message.wait);

        SoundManager.instance.PlayUiEfx(UiEfx.MESSAGE);
        messageBox.SetActive(true);
        messageBox.SetHeader(message.header);
        messageBox.SetMessage(message.message);
        Vector2 position = message.position;
        if (position.magnitude > 999f)
        {
            float radius = Random.Range(360f, 540f);
            float angle = Random.Range(0f, 2f * Mathf.PI);
            position.x = radius * Mathf.Cos(angle);
            position.y = radius * Mathf.Sin(angle) * Screen.height / Screen.width;
        }
        messageBox.SetPosition(position);

        float time = 0f;
        Vector2 baseSpeed = -20f * position.normalized;
        while (time < message.duration)
        {
            Vector2 speed = baseSpeed;
            if (time < 0.2f)
            {
                float rate = time / 0.2f;
                speed *= Mathf.Sin(0.5f * Mathf.PI * rate);
                messageBox.SetAlpha(rate);
            }
            if (time > message.duration - 0.5f)
            {
                float rate = (message.duration - time) / 0.5f;
                speed *= Mathf.Sin(0.5f * Mathf.PI * rate);
                messageBox.SetAlpha(rate);
            }
            messageBox.MoveBy(speed * Time.deltaTime);
            yield return null;
            time += Time.deltaTime;
        }
        messageBox.SetActive(false);

        if (messageQueue.Count != 0) StartCoroutine(ShowMessageBox(messageQueue.Dequeue()));
        else isShowingMessage = false;
    }
    
    //Fuctions for timer
    public void SetTimer(float time, HUDTimer.TimeOverHandler handler = null, Transform anchor = null)
    {
        hudTimer.anchor = anchor;
        hudTimer.SetActive(true);
        hudTimer.Set(time, handler);
    }
    
    public void HideTimer()
    {
        hudTimer.anchor = null;
        hudTimer.SetActive(false);
    }

    public bool IsTimeOver()
    {
        if (!hudTimer.IsActive) return true;
        return hudTimer.IsOver;
    }

    //Functions for progress

    public void ShowProgress(Transform anchor)
    {
        hudProgress.anchor = anchor;
        if (hudProgress != null)
            hudProgress.SetActive(true);
    }

    public void SetProgress(float value)
    {
        if (hudProgress != null)
            hudProgress.SetProgress(value);
    }

    public void HideProgress()
    {
        hudProgress.anchor = null;
        if (hudProgress != null)
            hudProgress.SetActive(false);
    }

    //Functions for hp bar

    public void AddHPBar(int key, string character, float x, float y, float width, Transform anchor = null)
    {
        hpBars.Add(key, Instantiate(hpBarPrefab, transform));
        hpBars[key].anchor = anchor;
        hpBars[key].character = character;
        hpBars[key].offset = new Vector2(x, y);
        hpBars[key].SetSize(width);
    }

    public void AutoFadeHPBar(int key, bool value)
    {
        if (hpBars.ContainsKey(key))
            hpBars[key].autoFade = value;
    }

    public void SetHP(int key, float hp)
    {
        if (hpBars.ContainsKey(key))
            hpBars[key].SetHP(hp);
    }

    public void ShowHPBar(int key)
    {
        if (hpBars.ContainsKey(key))
            hpBars[key].Show();
    }

    public void HideHPBar(int key)
    {
        if (hpBars.ContainsKey(key))
            hpBars[key].Hide();
    }

    public void RemoveHPBar(int key)
    {
        if (hpBars.ContainsKey(key))
        {
            Destroy(hpBars[key].gameObject);
            hpBars.Remove(key);
        }
    }
}