using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputDeviceType
{
    KeyboardAndMouse,
    PS4Controller,
    XboxOneController,
    CommonController
}

public enum JoystickAxis
{
    _XAxis = 0,
    _YAxis,
    _3rdAxis,
    _4thAxis,
    _5thAxis,
    _6thAxis,
    _7thAxis,
    _8thAxis,
    _9thAxis,
    _10thAxis,
    _11thAxis,
    _12thAxis,
    _13thAxis,
    _14thAxis,
    _15thAxis,
    _16thAxis
}

public class STBInputElement : MonoBehaviour
{
    public InputDeviceType deviceType;
    public bool invert;
    public JoystickAxis axis;
    public string negativeButton;
    public string positiveButton;
}
