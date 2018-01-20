using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FRL {
  public enum AxisType {
    KeyOrMouseButton = 0,
    MouseMovement = 1,
    JoystickAxis = 2
  };

  public class InputAxis {
    public string name;
    public string descriptiveName;
    public string descriptiveNegativeName;
    public string negativeButton;
    public string positiveButton;
    public string altNegativeButton;
    public string altPositiveButton;

    public float gravity;
    public float dead;
    public float sensitivity;

    public bool snap = false;
    public bool invert = false;

    public AxisType type;

    public int axis;
    public int joyNum;

    public InputAxis(string name, string descriptiveName, string descriptiveNegativeName, string negativeButton, string positiveButton,
      string altNegativeButton, string altPositiveButton, float gravity, float dead, float sensitivity, bool snap, bool invert,
      AxisType type, int axis, int joyNum) {
      
      this.name = name;
      this.descriptiveName = descriptiveName;
      this.descriptiveNegativeName = descriptiveNegativeName;
      this.negativeButton = negativeButton;
      this.positiveButton = positiveButton;
      this.altNegativeButton = altNegativeButton;
      this.altPositiveButton = altPositiveButton;
      this.gravity = gravity;
      this.dead = dead;
      this.sensitivity = sensitivity;
      this.snap = snap;
      this.invert = invert;
      this.type = type;
      this.axis = axis;
      this.joyNum = joyNum;
    }
  }

  public class InputManagerUtility {

    public static void ClearAxes() {
      SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
      SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
      axesProperty.ClearArray();
      serializedObject.ApplyModifiedProperties();
    }

    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
      SerializedProperty child = parent.Copy();
      child.Next(true);
      do {
        if (child.name == name) return child;
      }
      while (child.Next(false));
      return null;
    }

    private static bool AxisDefined(string axisName) {
      SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
      SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

      axesProperty.Next(true);
      axesProperty.Next(true);
      while (axesProperty.Next(false)) {
        SerializedProperty axis = axesProperty.Copy();
        axis.Next(true);
        if (axis.stringValue == axisName) return true;
      }
      return false;
    }

    private static int AxisIndex(string axisName) {
      SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
      SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

      axesProperty.Next(true);
      axesProperty.Next(true);
      int index = 0;
      while (axesProperty.Next(false)) {
        SerializedProperty axis = axesProperty.Copy();
        axis.Next(true);
        if (axis.stringValue == axisName) return index;
        index += 1;
      }
      return -1;
    }

    public static void UpdateAxis(InputAxis axis) {
      SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
      SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

      int targetIndex = -1;
      if (!AxisDefined(axis.name)) {
        axesProperty.arraySize++;
        serializedObject.ApplyModifiedProperties();
        targetIndex = axesProperty.arraySize - 1;
      } else {
        targetIndex = AxisIndex(axis.name);
      }

      if (targetIndex == -1) {
        Debug.LogError("Could not find index for axis: " + axis.name);
        return;
      }

      SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(targetIndex);

      GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
      GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
      GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
      GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
      GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
      GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
      GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
      GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
      GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
      GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
      GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
      GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
      GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
      GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
      GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

      serializedObject.ApplyModifiedProperties();
    }
  }
}

