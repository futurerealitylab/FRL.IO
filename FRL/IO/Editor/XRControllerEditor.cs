using UnityEditor;
using System.Collections.Generic;

namespace FRL.IO {
  [CustomEditor(typeof(XRControllerModule))]
  public class XRControllerModuleEditor : Editor {

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
      base.OnInspectorGUI();
      foreach (InputAxis axis in xrAxes) {
        InputManagerUtility.UpdateAxis(axis);
      }
    }
  }
}

