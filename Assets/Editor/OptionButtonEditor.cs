using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(OptionButton))]
[CanEditMultipleObjects]

public class OptionButtonEditor : ButtonEditor
{
    private SerializedObject button;
    private SerializedProperty controlName;
    private SerializedProperty controlGroup;
    private SerializedProperty description;
    private SerializedProperty focusSound;
    private SerializedProperty pressSound;
    private SerializedProperty darkColor;
    private SerializedProperty lightColor;
    private SerializedProperty pressColor;

    protected override void OnEnable()
    {
        base.OnEnable();
        button = new SerializedObject(target);
        controlName = button.FindProperty("controlName");
        controlGroup = button.FindProperty("controlGroup");
        description = button.FindProperty("description");
        focusSound = button.FindProperty("focusSound");
        pressSound = button.FindProperty("pressSound");
        darkColor = button.FindProperty("darkColor");
        lightColor = button.FindProperty("lightColor");
        pressColor = button.FindProperty("pressColor");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        button.Update();
        EditorGUILayout.PropertyField(controlName);
        EditorGUILayout.PropertyField(controlGroup);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(focusSound);
        EditorGUILayout.PropertyField(pressSound);
        EditorGUILayout.PropertyField(darkColor);
        EditorGUILayout.PropertyField(lightColor);
        EditorGUILayout.PropertyField(pressColor);
        button.ApplyModifiedProperties();
    }
}