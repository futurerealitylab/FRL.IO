using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if WAVE
using wvr;
#endif

namespace FRL.IO {
#if WAVE
  public class FocusControllerStatus : XRControllerStatus {

    private const string NAME = "Vive Focus Controller";

    private static Dictionary<XRButton, WVR_InputId> focusMappings = new Dictionary<XRButton, WVR_InputId>() {
      { XRButton.Trigger, WVR_InputId.WVR_InputId_Alias1_Trigger },
      { XRButton.Touchpad, WVR_InputId.WVR_InputId_Alias1_Touchpad },
      { XRButton.Menu, WVR_InputId.WVR_InputId_Alias1_Menu },
      { XRButton.Grip, WVR_InputId.WVR_InputId_Alias1_Grip },
      { XRButton.Left, WVR_InputId.WVR_InputId_Alias1_DPad_Left },
      { XRButton.Right, WVR_InputId.WVR_InputId_Alias1_DPad_Right },
      { XRButton.Back, WVR_InputId.WVR_InputId_Alias1_DPad_Down },
      { XRButton.Forward, WVR_InputId.WVR_InputId_Alias1_DPad_Up }
    };

    private WaveVR.Device device;
    private WaveVR_Controller.Device controller;

    public override bool IsTracked {
      get {
        return (device == null ? false : device.connected);
      }
    }

    public FocusControllerStatus(XRHand hand) : base(hand) {
      if (WaveVR.Instance == null) {
        Debug.LogError("WaveVR.Instance is null!");
        return;
      }
      var wvr_hand = hand == XRHand.Left ? WVR_DeviceType.WVR_DeviceType_Controller_Left : WVR_DeviceType.WVR_DeviceType_Controller_Right;
      device = WaveVR.Instance.getDeviceByType(wvr_hand);
      controller = WaveVR_Controller.Input(wvr_hand);
    }

    protected override void GenerateCurrentStatus() {
      if (device == null) {
        cPos = Vector3.zero;
        cRot = Quaternion.identity;
      } else {
        cPos = Camera.main.transform.localPosition + Camera.main.transform.localRotation * device.rigidTransform.pos;
        if (cPos != Vector3.zero) {
          Vector3 o = hand == XRHand.Left ? new Vector3(-0.2f, -0.3f, 0.4f) : new Vector3(0.2f, -0.3f, 0.4f);
          cPos += Camera.main.transform.rotation * o;
        }
        cRot = device.rigidTransform.rot;
      }

      cTriggerAxis = GetPressDown(XRButton.Trigger) ? 1f : 0f;
      cTouchpadAxis = Vector2.zero;
      cGripAxis = 0f;
      cThumbstickAxis = Vector2.zero;
    }

    public override bool GetClick(XRButton button) {
      return false;
    }

    public override bool GetPress(XRButton button) {
      if (controller == null || !focusMappings.ContainsKey(button)) return false;
      return controller.GetPress(focusMappings[button]);
    }

    public override bool GetPressDown(XRButton button) {
      if (controller == null || !focusMappings.ContainsKey(button)) return false;
      return controller.GetPressDown(focusMappings[button]);
    }

    public override bool GetPressUp(XRButton button) {
      if (controller == null || !focusMappings.ContainsKey(button)) return false;
      return controller.GetPressUp(focusMappings[button]);
    }

    public override bool GetTouch(XRButton button) {
      if (controller == null || !focusMappings.ContainsKey(button)) return false;
      return controller.GetTouch(focusMappings[button]);
    }

    public override bool GetTouchDown(XRButton button) {
      if (controller == null || !focusMappings.ContainsKey(button)) return false;
      return controller.GetTouchDown(focusMappings[button]);
    }

    public override bool GetTouchUp(XRButton button) {
      if (controller == null || !focusMappings.ContainsKey(button)) return false;
      return controller.GetTouchUp(focusMappings[button]);
    }
  }
#else
  public class FocusControllerStatus : BrokenControllerStatus {
    public FocusControllerStatus(XRHand hand) : base(hand, "WaveVR") { }
  }
#endif
}

