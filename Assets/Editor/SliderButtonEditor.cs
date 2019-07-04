using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SliderButton))]
[CanEditMultipleObjects]

public class SliderButtonEditor : ButtonEditor
{
    private SerializedObject slider;
    private SerializedProperty controlName;
    private SerializedProperty controlGroup;
    private SerializedProperty maxValue;
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
        slider = new SerializedObject(target);
        controlName = slider.FindProperty("controlName");
        controlGroup = slider.FindProperty("controlGroup");
        maxValue = slider.FindProperty("maxValue");
        onValueChanged = slider.FindProperty("onValueChanged");
        description = slider.FindProperty("description");
        focusSound = slider.FindProperty("focusSound");
        pressSound = slider.FindProperty("pressSound");
        darkColor = slider.FindProperty("darkColor");
        lightColor = slider.FindProperty("lightColor");
        pressColor = slider.FindProperty("pressColor");
        normalColor = slider.FindProperty("normalColor");
        normalWidth = slider.FindProperty("normalWidth");
        pressWidth = slider.FindProperty("pressWidth");
        speed = slider.FindProperty("speed");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        slider.Update();
        EditorGUILayout.PropertyField(controlName);
        EditorGUILayout.PropertyField(controlGroup);
        EditorGUILayout.PropertyField(maxValue);
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
        slider.ApplyModifiedProperties();
    }
}
