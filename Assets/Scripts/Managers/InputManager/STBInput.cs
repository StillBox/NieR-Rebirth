using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class STBInput : MonoBehaviour
{
    public static STBInput instance = null;

    #region Functions for Presets and Getting Unity Input Values

    public enum JoystickType
    {
        All,
        PlayStation4,
        Xbox_One,
        Unrecognized,
        Unconnected
    }

    public static readonly int JoystickCount = 4;
    public static JoystickType[] joysticks = new JoystickType[5];

    public static readonly int AxisCount = 16;
    public static string JoystickAxisName(int index, int joyNum = 0)
    {
        StringBuilder axisName = new StringBuilder();
        axisName.Append("Joystick");
        if (joyNum != 0) axisName.Append(joyNum);
        switch (index)
        {
            case 0:
                axisName.Append("AxisX");
                break;
            case 1:
                axisName.Append("AxisY");
                break;
            default:
                axisName.Append("Axis" + index);
                break;
        }
        return axisName.ToString();
    }
    public static string JoystickAxisName(JoystickAxis axis, int joyNum = 0)
    {
        return JoystickAxisName((int)axis, joyNum);
    }

    public static readonly int ButtonCount = 16;
    public static string JoystickButtonName(string index, int joyNum = 0)
    {
        StringBuilder buttonName = new StringBuilder();
        buttonName.Append("Joystick");
        if (joyNum != 0) buttonName.Append(joyNum);
        buttonName.Append("Button" + index);
        return buttonName.ToString();
    }
    public static string JoystickButtonName(int index, int joyNum = 0)
    {
        return JoystickButtonName(index.ToString(), joyNum);
    }

    private static float GetJoystickAxisRaw(int index, int joyNum = 0)
    {
        float value = Input.GetAxisRaw(JoystickAxisName(index, joyNum));
        if (joysticks[joyNum] == JoystickType.PlayStation4)
        {
            if (index == 3 || index == 4) value = 0.5f * (value + 1f);
        }
        return value;
    }

    private static float GetJoystickAxisRaw(JoystickAxis axis, int joyNum = 0)
    {
        return GetJoystickAxisRaw((int)axis, joyNum);
    }

    private static float GetJoystickAxis(int index, int joyNum = 0)
    {
        float value = Input.GetAxis(JoystickAxisName(index, joyNum));
        if (joysticks[joyNum] == JoystickType.PlayStation4)
        {
            if (index == 3 || index == 4) value = 0.5f * (value + 1f);
        }
        return value;
    }

    private static float GetJoystickAxis(JoystickAxis axis, int joyNum = 0)
    {
        return GetJoystickAxis((int)axis, joyNum);
    }

    private static bool GetJoystickButton(string index, int joyNum = 0)
    {
        return Input.GetButton(JoystickButtonName(index, joyNum));
    }

    private static bool GetJoystickButton(int index, int joyNum = 0)
    {
        return Input.GetButton(JoystickButtonName(index, joyNum));
    }

    private static bool GetJoystickButtonDown(string index, int joyNum = 0)
    {
        return Input.GetButtonDown(JoystickButtonName(index, joyNum));
    }

    private static bool GetJoystickButtonDown(int index, int joyNum = 0)
    {
        return Input.GetButtonDown(JoystickButtonName(index, joyNum));
    }

    private static bool GetJoystickButtonUp(string index, int joyNum = 0)
    {
        return Input.GetButtonUp(JoystickButtonName(index, joyNum));
    }

    private static bool GetJoystickButtonUp(int index, int joyNum = 0)
    {
        return Input.GetButtonUp(JoystickButtonName(index, joyNum));
    }

    private static bool AnyJoystickAxis(int joyNum)
    {
        for (int i = 0; i < AxisCount; i++)
        {
            if (GetJoystickAxis(i, joyNum) != 0f) return true;
        }
        return false;
    }

    private static bool AnyJoystickButton(int joyNum)
    {
        for (int i = 0; i < ButtonCount; i++)
        {
            if (GetJoystickButton(i, joyNum)) return true;
        }
        return false;
    }

    private static bool AnyJoystickInput(int joyNum)
    {
        return AnyJoystickAxis(joyNum) || AnyJoystickButton(joyNum);
    }

    private static bool AnyMouseMovement()
    {
        return Input.GetAxis("Mouse X") != 0f ||
            Input.GetAxis("Mouse Y") != 0f ||
            Input.GetAxis("Mouse Z") != 0f;
    }

    IEnumerator CheckJoysticks()
    {
        joysticks[0] = JoystickType.All;
        while (true)
        {
            string[] names = Input.GetJoystickNames();
            for (int j = 1; j <= JoystickCount; j++)
            {
                if (j <= names.Length)
                {
                    switch (names[j - 1].Length)
                    {
                        case 19:
                            joysticks[j] = JoystickType.PlayStation4;
                            break;
                        case 33:
                            joysticks[j] = JoystickType.Xbox_One;
                            break;
                        default:
                            joysticks[j] = JoystickType.Unrecognized;
                            break;
                    }
                }
                else
                {
                    joysticks[j] = JoystickType.Unconnected;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion

    #region Functions for Axes(Buttons) Setting and Getting

    public enum AxisType
    {
        Axis,
        Button
    }

    public enum PlayerNum
    {
        AllPlayer = -1,
        Player1 = 0,
        Player2,
        Player3,
        Player4
    }
    
    [System.Serializable]
    public class STBAxis
    {
        public string name;
        public AxisType type;
        public PlayerNum playerNum;
        public bool sumInputs;
        public STBInputElement[] inputs;
    }
    
    /// <summary>
    /// DeviceNum 0 for Keyboard and mouse, 1 - 4 for joysticks
    /// </summary>
    public static int[] playerDevices = { 0, -1, -1, -1 };

    static InputDeviceType GetDeviceType(int deviceNum)
    {
        if (deviceNum == 0)
        {
            return InputDeviceType.KeyboardAndMouse;
        }
        else
        {
            switch (joysticks[deviceNum])
            {
                case JoystickType.PlayStation4:
                    return InputDeviceType.PS4Controller;
                case JoystickType.Xbox_One:
                    return InputDeviceType.XboxOneController;
                default:
                    return InputDeviceType.XboxOneController;
            }
        }
    }

    public static InputDeviceType GetPlayerDeviceType(int playerIndex)
    {
        return GetDeviceType(playerDevices[playerIndex]);
    }

    //For Get Axis or Button Input

    static STBAxis FindAxis(string name)
    {
        foreach (STBAxis axis in instance.axes)
        {
            if (axis.name.Equals(name))
                return axis;
        }
        return null;
    }

    static bool EffectPlayer(STBAxis axis, int index)
    {
        if (!IsPlayerDeviceSet(index)) return false;
        if (axis.playerNum == PlayerNum.AllPlayer) return true;
        if (axis.playerNum == (PlayerNum)index) return true;
        return false;
    }

    //Axis
    
    private static float MaxAxis(float a, float b)
    {
        return Mathf.Abs(a) > Mathf.Abs(b) ? a : b;
    }

    public static float GetAxisRaw(string axisName, int deviceNum)
    {
        STBAxis axis = FindAxis(axisName);
        if (axis == null) return 0f;

        float value = 0f;
        InputDeviceType deviceType = GetDeviceType(deviceNum);
        
        switch (deviceType)
        {
            case InputDeviceType.KeyboardAndMouse:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != InputDeviceType.KeyboardAndMouse) continue;
                    
                    float keyAxis = 0f;
                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (Input.GetKey(input.negativeButton)) keyAxis -= 1f;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (Input.GetKey(input.positiveButton)) keyAxis += 1f;

                    if (axis.sumInputs) value += keyAxis;
                    else value = MaxAxis(value, keyAxis);                    
                }
                break;
            default:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != deviceType) continue;
                    
                    float joyAxis = GetJoystickAxisRaw(input.axis, deviceNum);
                    if (input.invert) joyAxis *= -1f;
                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (GetJoystickButton(input.negativeButton, deviceNum)) joyAxis -= 1f;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (GetJoystickButton(input.positiveButton, deviceNum)) joyAxis += 1f;

                    if (axis.sumInputs) value += joyAxis;
                    else value = MaxAxis(value, joyAxis);                    
                }
                break;
        }

        value = Mathf.Clamp(value, -1f, 1f);
        return value;
    }

    public static float GetAxisRaw(string axisName)
    {
        STBAxis axis = FindAxis(axisName);
        if (axis == null) return 0f;

        float value = 0f;

        for (int i = 0; i < 4; i++)
        {
            if (!EffectPlayer(axis, i)) continue;
            
            float playerAxis = GetAxisRaw(axisName, playerDevices[i]);
            if (axis.sumInputs) value += playerAxis;
            else value = MaxAxis(value, playerAxis);            
        }

        value = Mathf.Clamp(value, -1f, 1f);
        return value;
    }

    public static float GetAxis(string axisName, int deviceNum)
    {
        STBAxis axis = FindAxis(axisName);
        if (axis == null) return 0f;

        float value = 0f;
        InputDeviceType deviceType = GetDeviceType(deviceNum);

        switch (deviceType)
        {
            case InputDeviceType.KeyboardAndMouse:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != InputDeviceType.KeyboardAndMouse) continue;

                    float keyAxis = 0f;
                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (Input.GetKey(input.negativeButton)) keyAxis -= 1f;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (Input.GetKey(input.positiveButton)) keyAxis += 1f;

                    if (axis.sumInputs) value += keyAxis;
                    else value = MaxAxis(value, keyAxis);                    
                }
                break;
            default:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != deviceType) continue;
                    
                    float joyAxis = GetJoystickAxis(input.axis, deviceNum);
                    if (input.invert) joyAxis *= -1f;
                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (GetJoystickButton(input.negativeButton, deviceNum)) joyAxis -= 1f;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (GetJoystickButton(input.positiveButton, deviceNum)) joyAxis += 1f;

                    if (axis.sumInputs) value += joyAxis;
                    else value = MaxAxis(value, joyAxis);                    
                }
                break;
        }

        value = Mathf.Clamp(value, -1f, 1f);
        return value;
    }

    public static float GetAxis(string axisName)
    {
        STBAxis axis = FindAxis(axisName);
        if (axis == null) return 0f;

        float value = 0f;

        for (int i = 0; i < 4; i++)
        {
            if (!EffectPlayer(axis, i)) continue;

            float playerAxis = GetAxis(axisName, playerDevices[i]);
            if (axis.sumInputs) value += playerAxis;
            else value = MaxAxis(value, playerAxis);            
        }

        value = Mathf.Clamp(value, -1f, 1f);
        return value;
    }

    // Button

    public static bool GetButton(string buttonName, int deviceNum)
    {
        STBAxis axis = FindAxis(buttonName);
        if (axis == null) return false;

        InputDeviceType deviceType = GetDeviceType(deviceNum);

        switch (deviceType)
        {
            case InputDeviceType.KeyboardAndMouse:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != InputDeviceType.KeyboardAndMouse) continue;

                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (Input.GetKey(input.negativeButton)) return true;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (Input.GetKey(input.positiveButton)) return true;                    
                }
                break;
            default:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != deviceType) continue;
                    
                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (GetJoystickButton(input.negativeButton, deviceNum)) return true;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (GetJoystickButton(input.positiveButton, deviceNum)) return true;                    
                }
                break;
        }
        return false;
    }

    public static bool GetButton(string buttonName)
    {
        STBAxis axis = FindAxis(buttonName);
        if (axis == null) return false;

        for (int i = 0; i < 4; i++)
        {
            if (!EffectPlayer(axis, i)) continue;
            if (GetButton(buttonName, playerDevices[i])) return true;
        }
        return false;
    }

    public static bool GetButtonDown(string buttonName, int deviceNum)
    {
        STBAxis axis = FindAxis(buttonName);
        if (axis == null) return false;

        InputDeviceType deviceType = GetDeviceType(deviceNum);

        switch (deviceType)
        {
            case InputDeviceType.KeyboardAndMouse:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != InputDeviceType.KeyboardAndMouse) continue;
                    
                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (Input.GetKeyDown(input.negativeButton)) return true;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (Input.GetKeyDown(input.positiveButton)) return true;                    
                }
                break;
            default:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != deviceType) continue;
                    
                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (GetJoystickButtonDown(input.negativeButton, deviceNum)) return true;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (GetJoystickButtonDown(input.positiveButton, deviceNum)) return true;                    
                }
                break;
        }
        return false;
    }

    public static bool GetButtonDown(string buttonName)
    {
        STBAxis axis = FindAxis(buttonName);
        if (axis == null) return false;

        if (instance.playerCount == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (GetButtonDown(buttonName, i)) return true;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (!EffectPlayer(axis, i)) continue;
                if (GetButtonDown(buttonName, playerDevices[i])) return true;
            }
        }
        return false;
    }

    public static bool GetButtonUp(string buttonName, int deviceNum)
    {
        STBAxis axis = FindAxis(buttonName);
        if (axis == null) return false;

        InputDeviceType deviceType = GetDeviceType(deviceNum);

        switch (deviceType)
        {
            case InputDeviceType.KeyboardAndMouse:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != InputDeviceType.KeyboardAndMouse) continue;

                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (Input.GetKeyUp(input.negativeButton)) return true;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (Input.GetKeyUp(input.positiveButton)) return true;
                }
                break;
            default:
                foreach (STBInputElement input in axis.inputs)
                {
                    if (input.deviceType != deviceType) continue;

                    if (!string.IsNullOrEmpty(input.negativeButton))
                        if (GetJoystickButtonUp(input.negativeButton, deviceNum)) return true;
                    if (!string.IsNullOrEmpty(input.positiveButton))
                        if (GetJoystickButtonUp(input.positiveButton, deviceNum)) return true;
                }
                break;
        }
        return false;
    }

    public static bool GetButtonUp(string buttonName)
    {
        STBAxis axis = FindAxis(buttonName);
        if (axis == null) return false;

        if (instance.playerCount == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (GetButtonUp(buttonName, i)) return true;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (!EffectPlayer(axis, i)) continue;
                if (GetButtonUp(buttonName, playerDevices[i])) return true;
            }
        }
        return false;
    }

    // Device
    
    static bool IsPlayerDeviceSet(int index)
    {
        return playerDevices[index] != -1;
    }

    static bool IsDeviceOccupied(int deviceNum)
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerDevices[i] == deviceNum) return true;
        }
        return false;
    }

    public static int ActiveInputDevice()
    {
        for (int j = 1; j <= JoystickCount; j++)
        {
            if (AnyJoystickInput(j)) return j;
        }

        if (Input.anyKey)
        {
            return 0;
        }

        else return -1;
    }

    public static bool IsDetectingDevice = false;

    public static void SetPlayerDevice(int index)
    {
        IsDetectingDevice = true;
        instance.StartCoroutine(DetectPlayerDevice(index));
    }

    static IEnumerator DetectPlayerDevice(int index)
    {
        while (IsDetectingDevice)
        {
            int activeDeviceNum = ActiveInputDevice();
            if (activeDeviceNum != -1 &&
                !IsDeviceOccupied(activeDeviceNum))
            {
                playerDevices[index] = activeDeviceNum;
                IsDetectingDevice = false;
            }
            yield return null;
        }
    }

    public static void SetPlayerDevice(int index, string enter, string cancel)
    {
        IsDetectingDevice = true;
        instance.StartCoroutine(DetectPlayerDevice(index, enter, cancel));
    }

    static IEnumerator DetectPlayerDevice(int index, string enter, string cancel)
    {
        while (IsDetectingDevice)
        {
            for (int deviceNum = 0; deviceNum < 5; deviceNum++)
            {
                if (IsDeviceOccupied(deviceNum)) continue;
                if (GetButtonDown(enter, deviceNum))
                {
                    playerDevices[index] = deviceNum;
                    IsDetectingDevice = false;
                }
            }
            if (GetButtonDown(cancel))
                IsDetectingDevice = false;
            yield return null;
        }
    }

    #endregion

    //For Inspector and Initial

    [Range(1, 4)]
    public int playerCount = 1;
    public bool autoCheckDevice = true;
    [SerializeField] private STBAxis[] axes;

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

    private void Start()
    {
        StartCoroutine(CheckJoysticks());
        StartCoroutine(SetFirstPlayerDevice());
    }

    IEnumerator SetFirstPlayerDevice()
    {
        SetPlayerDevice(0);
        while (IsDetectingDevice) yield return null;

        if (playerCount == 1)
            StartCoroutine(SetFirstPlayerDevice());
        else if (autoCheckDevice)
            StartCoroutine(SetRestPlayerDevice());
    }

    IEnumerator SetRestPlayerDevice()
    {
        for (int i = 1; i < playerCount; i++)
        {
            SetPlayerDevice(i);
            while (IsDetectingDevice) yield return null;
        }
    }

    //For Monitoring

    static bool isJoystickMonitorOn = false;
    static bool isDeviceMonitorOn = false;
    static bool isInputMonitorOn = false;

    private void OnGUI()
    {
        if (!GameManager.isDebugModeOn) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleLeft;

        float heightExpand = 0f;

        if (GUI.Button(new Rect(0f, 0f, 160f, 30f), "Joystick Monitor"))
            isJoystickMonitorOn = !isJoystickMonitorOn;

        if (isJoystickMonitorOn)
        {
            for (int j = 0; j <= JoystickCount; j++)
            {
                Rect rect = new Rect(20f + 280f * j, 40f, 200f, 20f);
                GUI.Label(rect, "Joystick " + j + " - " + joysticks[j].ToString(), style);

                rect.y += 30f;
                GUI.Label(rect, "Axes :", style);
                rect.x += 60f;
                GUI.Label(rect, AnyJoystickAxis(j).ToString(), style);

                for (int i = 0; i < AxisCount; i++)
                {
                    rect.x = 20f + 280f * j;
                    rect.y = 100f + 20f * i;
                    GUI.Label(rect, JoystickAxisName(i, j).Substring(j == 0 ? 8 : 9) + " :", style);
                    rect.x += 60f;
                    GUI.Label(rect, string.Format("{0:N2}", GetJoystickAxis(i, j)), style);
                }

                rect.x = 140f + 280f * j;
                rect.y = 70f;
                GUI.Label(rect, "Buttons :", style);
                rect.x += 70f;
                GUI.Label(rect, AnyJoystickButton(j).ToString(), style);

                for (int i = 0; i < ButtonCount; i++)
                {
                    rect.x = 140f + 280f * j;
                    rect.y = 100f + 20f * i;
                    GUI.Label(rect, JoystickButtonName(i, j).Substring(j == 0 ? 8 : 9) + " :", style);
                    rect.x += 70f;
                    GUI.Label(rect, GetJoystickButton(i, j).ToString(), style);
                }
            }
            heightExpand += 100f + 20f * (Mathf.Max(AxisCount, ButtonCount) - 1);
        }

        if (GUI.Button(new Rect(0f, 30f + heightExpand, 160f, 30f), "Player Device Monitor"))
            isDeviceMonitorOn = !isDeviceMonitorOn;

        if (isDeviceMonitorOn)
        {
            for (int i = 0; i < playerCount; i++)
            {
                Rect rect = new Rect(20f + 280f * i, 70f + heightExpand, 200f, 20f);
                string device = playerDevices[i] == -1 ? "Unset" :
                    playerDevices[i] == 0 ? "Keyboard / Mouse" :
                    "Joystick " + playerDevices[i];
                GUI.Label(rect, "Player " + (i + 1) + " - " + device, style);
            }
            heightExpand += 40f;
        }

        if (GUI.Button(new Rect(0f, 60f + heightExpand, 160f, 30f), "Input Monitor"))
            isInputMonitorOn = !isInputMonitorOn;

        if (isInputMonitorOn)
        {
            for (int i = 0; i < axes.Length; i++)
            {
                Rect rect = new Rect(20f, 100f + heightExpand + 20f * i, 200f, 20f);
                GUI.Label(rect, axes[i].name, style);
                rect.x += 120f;
                switch (axes[i].type)
                {
                    case AxisType.Axis:
                        GUI.Label(rect, string.Format("{0:N2}", GetAxis(axes[i].name)), style);
                        break;
                    case AxisType.Button:
                        GUI.Label(rect, GetButton(axes[i].name).ToString(), style);
                        break;
                }
            }
        }
    }
}