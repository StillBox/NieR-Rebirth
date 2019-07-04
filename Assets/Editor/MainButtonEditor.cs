using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(MainButton))]
[CanEditMultipleObjects]

public class MainButtonEditor : ButtonEditor
{
    private SerializedObject button;
    private SerializedProperty controlName;
    private SerializedProperty controlGroup;
    private SerializedProperty focusSound;
    private SerializedProperty pressSound;
    private SerializedProperty height;
    private SerializedProperty widthNormal;
    private SerializedProperty widthHighlight;
    private SerializedProperty widthPressed;
    private SerializedProperty speed;

    protected override void OnEnable()
    {
        base.OnEnable();
        button = new SerializedObject(target);
        controlName = button.FindProperty("controlName");
        controlGroup = button.FindProperty("controlGroup");
        focusSound = button.FindProperty("focusSound");
        pressSound = button.FindProperty("pressSound");
        height = button.FindProperty("height");
        widthNormal = button.FindProperty("widthNormal");
        widthHighlight = button.FindProperty("widthHighlight");
        widthPressed = button.FindProperty("widthPressed");
        speed = button.FindProperty("speed");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        button.Update();
        EditorGUILayout.PropertyField(controlName);
        EditorGUILayout.PropertyField(controlGroup);
        EditorGUILayout.PropertyField(focusSound);
        EditorGUILayout.PropertyField(pressSound);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(widthNormal);
        EditorGUILayout.PropertyField(widthHighlight);
        EditorGUILayout.PropertyField(widthPressed);
        EditorGUILayout.PropertyField(speed);
        button.ApplyModifiedProperties();
    }
}