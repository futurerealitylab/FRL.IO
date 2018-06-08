using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace FRL {
  [CustomEditor(typeof(XRManager))]
  public class XRManagerEditor : Editor {

    private List<InputAxis> xrAxes = new List<InputAxis>() {
      new InputAxis("LThumbstickX","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,1,0),
      new InputAxis("LThumbstickY","","","","","","",0,0.001f,1,true,true,AxisType.JoystickAxis,2,0),
      new InputAxis("RThumbstickX","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,4,0),
      new InputAxis("RThumbstickY","","","","","","",0,0.001f,1,true,true,AxisType.JoystickAxis,5,0),
      new InputAxis("WMR_LTouchpadX","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,17,0),
      new InputAxis("WMR_LTouchpadY","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,18,0),
      new InputAxis("WMR_RTouchpadX","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,19,0),
      new InputAxis("WMR_RTouchpadY","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,20,0),
      new InputAxis("LTrigger","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,9,0),
      new InputAxis("RTrigger","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,10,0),
      new InputAxis("LGrip","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,11,0),
      new InputAxis("RGrip","","","","","","",0,0.001f,1,true,false,AxisType.JoystickAxis,12,0)
    };

    public override void OnInspectorGUI() {
      UpdateXRAxes();

      GUILayout.Space(5);
      XRManager script = (XRManager)target;
      XRSystem system = (XRSystem)EditorGUILayout.EnumPopup("System: ", script.System);
      bool switchTarget = EditorGUILayout.ToggleLeft("Switch Build Target On Change", script.switchBuildTargetOnChange);
      script.switchBuildTargetOnChange = switchTarget;

      if (script.System != system) {
        if (EditorApplication.isPlaying) {
          Debug.LogError("Cannot switch systems during Play Mode.");
          return;
        }
        Undo.RecordObject(script, "Set Value");

        script.SwitchToSystem(system);
        bool vrs = system == XRSystem.Standalone ? false : true;

        PlayerSettings.virtualRealitySupported = vrs;

        if (switchTarget)
          SwitchActiveBuildTarget(script.System);
      }

      GUILayout.Space(5);
      EditorGUILayout.LabelField("Supported External SDKS:", EditorStyles.boldLabel);
      for (int i = 0; i < script.AllSDKs.Count; i++) {
        script.EnabledSDKs[i] = EditorGUILayout.ToggleLeft(script.SDKNames[i], script.EnabledSDKs[i]);
      }

      //UpdateScriptingDefineSymbols(script);

      EditorUtility.SetDirty(script);
    }

    private void UpdateXRAxes() {
      foreach (InputAxis axis in xrAxes) {
        InputManagerUtility.UpdateAxis(axis);
      }
    }

    private void UpdateScriptingDefineSymbols(XRManager script) {

      BuildTargetGroup[] groups = new BuildTargetGroup[] {
        BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.WSA
      };

      foreach (BuildTargetGroup group in groups) {
        string s = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        List<string> defined = new List<string>(s.Split(';'));
        
        for (int i = 0; i < script.AllSDKs.Count; i++) {
          string sdk = script.AllSDKs[i];
          if (script.EnabledSDKs[i]) {
            if (!defined.Contains(sdk)) defined.Add(sdk);
          } else if (defined.Contains(sdk)) defined.Remove(sdk);
        }

        string result = "";
        foreach (string symbol in defined) result += symbol + ";";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, result);
      }
    }

    private void SwitchActiveBuildTarget(XRSystem system) {
      switch (system) {
        case XRSystem.Standalone:
        case XRSystem.CV1:
        case XRSystem.Vive:
          EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
          break;
        case XRSystem.WindowsMR:
          EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
          break;
        case XRSystem.GearVROculusGo:
          EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
          break;
      }
    }
  }
}

