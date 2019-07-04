using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(DropDownButton))]
[CanEditMultipleObjects]

public class DropDownEditor : ButtonEditor
{
    private SerializedObject dropDown;
    private SerializedProperty controlName;
    private SerializedProperty controlGroup;
    private SerializedProperty options;
    private SerializedProperty onValueChanged;
    private SerializedProperty description;
    private SerializedProperty focusSound;
    private SerializedProperty pressSound;
    private SerializedProperty darkColor;
    private SerializedProperty lightColor;
    private SerializedProperty pressColor;
    private SerializedProperty normalColor;
    private SerializedProperty normalWidth;
    private SerializedProperty pressWidth;
    private SerializedProperty speed;

    protected override void OnEnable()
    {
        base.OnEnable();
        dropDown = new SerializedObject(target);
        controlName = dropDown.FindProperty("controlName");
        controlGroup = dropDown.FindProperty("controlGroup");
        options = dropDown.FindProperty("options");
        onValueChanged = dropDown.FindProperty("onValueChanged");
        description = dropDown.FindProperty("description");
        focusSound = dropDown.FindProperty("focusSound");
        pressSound = dropDown.FindProperty("pressSound");
        darkColor = dropDown.FindProperty("darkColor");
        lightColor = dropDown.FindProperty("lightColor");
        pressColor = dropDown.FindProperty("pressColor");
        normalColor = dropDown.FindProperty("normalColor");
        normalWidth = dropDown.FindProperty("normalWidth");
        pressWidth = dropDown.FindProperty("pressWidth");
        speed = dropDown.FindProperty("speed");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        dropDown.Update();
        EditorGUILayout.PropertyField(controlName);
        EditorGUILayout.PropertyField(controlGroup);
        EditorGUILayout.PropertyField(options, true);
        EditorGUILayout.PropertyField(onValueChanged);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(focusSound);
        EditorGUILayout.PropertyField(pressSound);
        EditorGUILayout.PropertyField(darkColor);
        EditorGUILayout.PropertyField(lightColor);
        EditorGUILayout.PropertyField(pressColor);
        EditorGUILayout.PropertyField(normalColor);
        EditorGUILayout.PropertyField(normalWidth);
        EditorGUILayout.PropertyField(pressWidth);
        EditorGUILayout.PropertyField(speed);
        dropDown.ApplyModifiedProperties();
    }
}