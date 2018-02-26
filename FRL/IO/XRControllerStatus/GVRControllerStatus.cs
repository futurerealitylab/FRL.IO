using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
#if OVR
  public class GVRControllerStatus : XRControllerStatus {

    private OVRInput.Controller controller;

    public override bool IsTracked {
      get {
        return (controller != default(OVRInput.Controller) && OVRInput.GetActiveController() == controller);
      }
    }

    public GVRControllerStatus(XRHand hand) : base(hand) {
      if (hand == XRHand.Right) return;
      controller = OVRInput.GetActiveController();
    }

    protected override void GenerateCurrentStatus() {
      //Position and Rotation
      cPos = Camera.main.transform.localPosition + OVRInput.GetLocalControllerPosition(controller);
      cRot = OVRInput.GetLocalControllerRotation(controller);
      //Axes
      cThumbstickAxis = Vector2.zero;
      cTouchpadAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
      cTriggerAxis = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
      cGripAxis = 0f;
    }

    public override bool GetClick(XRButton button) {
      throw new System.NotImplementedException();
    }

    public override bool GetPress(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        case XRButton.Menu:
          return OVRInput.Get(OVRInput.Button.Back);
        case XRButton.Touchpad:
          return OVRInput.Get(OVRInput.Button.PrimaryTouchpad);
        default:
          return false;
      }
    }

    public override bool GetPressDown(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
        case XRButton.Menu:
          return OVRInput.GetDown(OVRInput.Button.Back);
        case XRButton.Touchpad:
          return OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad);
        default:
          return false;
      }
    }

    public override bool GetPressUp(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger);
        case XRButton.Menu:
          return OVRInput.GetUp(OVRInput.Button.Back);
        case XRButton.Touchpad:
          return OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad);
        default:
          return false;
      }
    }

    public override bool GetTouch(XRButton button) {
      if (button == XRButton.Touchpad) return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad);
      else return false;
    }

    public override bool GetTouchDown(XRButton button) {
      if (button == XRButton.Touchpad) return OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad);
      else return false;
    }

    public override bool GetTouchUp(XRButton button) {
      if (button == XRButton.Touchpad) return OVRInput.GetUp(OVRInput.Touch.PrimaryTouchpad);
      else return false;
    }
  }
#else
  public class GVRControllerStatus : BrokenControllerStatus {
    public GVRControllerStatus(XRHand hand) : base(hand, "GVR") { }
  }
#endif
}

