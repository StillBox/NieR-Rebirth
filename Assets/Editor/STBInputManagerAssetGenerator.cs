using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class STBInputManagerAssetGenerator
{
    static List<AxisPreset> axes = new List<AxisPreset>();

    static STBInputManagerAssetGenerator()
    {
        ApplyAxes();
    }

    [MenuItem("Edit/Project Settings/STBInput/Setup Base InputManager")]
    static void GenerateInputManagerAsset()
    {
        ApplyAxes();
    }
    /*
    [MenuItem("Edit/Project Settings/STBInput/Export Input Settings")]
    static void ExprotInputSettings()
    {
        string path = EditorUtility.SaveFilePanel("Save input Settings", "", "STBInput", "set");
    }
    
    [MenuItem("Edit/Project Settings/STBInput/Import Input Settings")]
    static void ImportInputSettings()
    {

    }
    */
    static void ApplyAxes()
    {
        SetupAxes();

        var inputManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
        var serializedObject = new SerializedObject(inputManagerAsset);
        var axisArray = serializedObject.FindProperty("m_Axes");

        axisArray.arraySize = axes.Count;
        serializedObject.ApplyModifiedProperties();

        for (int i = 0; i < axes.Count; i++)
        {
            var axisEntry = axisArray.GetArrayElementAtIndex(i);
            axes[i].ApplyTo(ref axisEntry);
        }

        serializedObject.ApplyModifiedProperties();

        AssetDatabase.Refresh();
    }

    static void SetupAxes()
    {
        axes.Clear();
        CreateRequiredPresets();
    }

    static void CreateRequiredPresets()
    {
        for (int j = 0; j <= STBInput.JoystickCount; j++)
        {
            for (int i = 0; i < STBInput.AxisCount; i++)
            {
                axes.Add(new AxisPreset(STBInput.JoystickAxisName(i, j), 2, i, j));
            }
            for (int i = 0; i < STBInput.ButtonCount; i++)
            {
                axes.Add(new AxisPreset(STBInput.JoystickButtonName(i, j), 0, i, j));
            }
        }

        axes.Add(new AxisPreset("Mouse X", 1, 0, 0));
        axes.Add(new AxisPreset("Mouse Y", 1, 1, 0));
        axes.Add(new AxisPreset("Mouse Z", 1, 2, 0));
    }
    
    static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);

        do
        {
            if (child.name == name)
            {
                return child;
            }
        } while (child.Next(false));

        return null;
    }

    internal class AxisPreset
    {
        public string name;
        public string descriptiveName;
        public string descriptiveNegativeName;
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;
        public float gravity;
        public float deadZone = 0.001f;
        public float sensitivity = 1.0f;
        public bool snap;
        public bool invert;
        public int type;
        public int axis;
        public int joyNum;
        
        public AxisPreset()
        {
        }
        
        public AxisPreset(SerializedProperty axisPreset)
        {
            this.name = GetChildProperty(axisPreset, "m_Name").stringValue;
            this.descriptiveName = GetChildProperty(axisPreset, "descriptiveName").stringValue;
            this.descriptiveNegativeName = GetChildProperty(axisPreset, "descriptiveNegativeName").stringValue;
            this.negativeButton = GetChildProperty(axisPreset, "negativeButton").stringValue;
            this.positiveButton = GetChildProperty(axisPreset, "positiveButton").stringValue;
            this.altNegativeButton = GetChildProperty(axisPreset, "altNegativeButton").stringValue;
            this.altPositiveButton = GetChildProperty(axisPreset, "altPositiveButton").stringValue;
            this.gravity = GetChildProperty(axisPreset, "gravity").floatValue;
            this.deadZone = GetChildProperty(axisPreset, "dead").floatValue;
            this.sensitivity = GetChildProperty(axisPreset, "sensitivity").floatValue;
            this.snap = GetChildProperty(axisPreset, "snap").boolValue;
            this.invert = GetChildProperty(axisPreset, "invert").boolValue;
            this.type = GetChildProperty(axisPreset, "type").intValue;
            this.axis = GetChildProperty(axisPreset, "axis").intValue;
            this.joyNum = GetChildProperty(axisPreset, "joyNum").intValue;
        }
        
        public AxisPreset(string name, int type, int num, int joyNum)
        {
            this.name = name;
            this.descriptiveName = "";
            this.descriptiveNegativeName = "";
            this.negativeButton = "";
            this.positiveButton = "";
            this.altNegativeButton = "";
            this.altPositiveButton = "";
            this.snap = false;
            this.invert = false;
            this.type = type;
            this.joyNum = joyNum;
            switch (type)
            {
                case 0:
                    this.gravity = 1000f;
                    this.deadZone = 0.001f;
                    this.sensitivity = 1000f;
                    this.positiveButton = joyNum == 0 ? "joystick button " + num :
                        "joystick " + joyNum + " button " + num;
                    this.axis = 1;
                    break;
                case 1:
                    this.gravity = 0.0f;
                    this.deadZone = 0f;
                    this.sensitivity = 0.1f;
                    this.axis = num;
                    break;
                case 2:
                    this.gravity = 0.0f;
                    this.deadZone = 0.1f;
                    this.sensitivity = 1f;
                    this.axis = num;
                    break;
            }
        }
        
        public void ApplyTo(ref SerializedProperty axisPreset)
        {
            GetChildProperty(axisPreset, "m_Name").stringValue = name;
            GetChildProperty(axisPreset, "descriptiveName").stringValue = descriptiveName;
            GetChildProperty(axisPreset, "descriptiveNegativeName").stringValue = descriptiveNegativeName;
            GetChildProperty(axisPreset, "negativeButton").stringValue = negativeButton;
            GetChildProperty(axisPreset, "positiveButton").stringValue = positiveButton;
            GetChildProperty(axisPreset, "altNegativeButton").stringValue = altNegativeButton;
            GetChildProperty(axisPreset, "altPositiveButton").stringValue = altPositiveButton;
            GetChildProperty(axisPreset, "gravity").floatValue = gravity;
            GetChildProperty(axisPreset, "dead").floatValue = deadZone;
            GetChildProperty(axisPreset, "sensitivity").floatValue = sensitivity;
            GetChildProperty(axisPreset, "snap").boolValue = snap;
            GetChildProperty(axisPreset, "invert").boolValue = invert;
            GetChildProperty(axisPreset, "type").intValue = type;
            GetChildProperty(axisPreset, "axis").intValue = axis;
            GetChildProperty(axisPreset, "joyNum").intValue = joyNum;
        }        
    }
}
