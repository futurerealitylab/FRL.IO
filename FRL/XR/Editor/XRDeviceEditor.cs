using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FRL {
  [CustomEditor(typeof(XRDevice))]
  public class XRDeviceEditor : Editor {

    public override void OnInspectorGUI() {
      XRDevice script = (XRDevice)target;

      Color c = script.IsTracked ? Color.green : Color.red;
      string text = script.IsTracked ? "Tracked" : "Untracked";

      if (!EditorApplication.isPlaying) {
        text = "Unknown";
        c = Color.gray;
      }

      text = "System: " + script.System.ToString() + "\t" + text;
      GUIStyle style = new GUIStyle();
      style.normal.textColor = c;
      EditorGUILayout.LabelField(text, style);
      base.OnInspectorGUI();
      EditorUtility.SetDirty(script);

    }
  }

  [CustomEditor(typeof(XRHMD))]
  public class XRHMDEditor : XRDeviceEditor {

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      XRHMD script = (XRHMD)target;
      int number = EditorGUILayout.IntField("Editor Tracking Override: ", script.trackingNumber);
      script.UpdateTrackingNumber(number);
      EditorUtility.SetDirty(script);
    }
  }

  [CustomEditor(typeof(XRController))]
  public class XRControllerEditor : XRDeviceEditor { }
}
