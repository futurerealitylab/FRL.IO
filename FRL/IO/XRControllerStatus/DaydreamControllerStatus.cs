using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
#if DAYDREAM
  public class DaydreamControllerStatus : XRControllerStatus {


    public DaydreamControllerStatus(XRHand hand) : base(hand) {
      
    }

    public override bool GetClick(XRButton button) {
      return GetPressDown(button);
    }

    public override bool GetPress(XRButton button) {
      switch (button) {
        case XRButton.Touchpad:
          return GvrControllerInput.ClickButton;
        case XRButton.Menu:
          return GvrControllerInput.AppButton;
        case XRButton.Home:
          return GvrControllerInput.HomeButtonState;
        default:
          return false;
      }
    }

    public override bool GetPressDown(XRButton button) {
      switch (button) {
        case XRButton.Touchpad:
          return GvrControllerInput.ClickButtonDown;
        case XRButton.Menu:
          return GvrControllerInput.AppButtonDown;
        case XRButton.Home:
          return GvrControllerInput.HomeButtonDown;
        default:
          return false;
      }
    }

    public override bool GetPressUp(XRButton button) {
      switch (button) {
        case XRButton.Touchpad:
          return GvrControllerInput.ClickButtonUp;
        case XRButton.Menu:
          return GvrControllerInput.AppButtonUp;
        default:
          return false;
      }
    }

    public override bool GetTouch(XRButton button) {
      if (button == XRButton.Touchpad) return GvrControllerInput.IsTouching;
      else return false;
    }

    public override bool GetTouchDown(XRButton button) {
      if (button == XRButton.Touchpad) return GvrControllerInput.TouchDown;
      else return false;
    }

    public override bool GetTouchUp(XRButton button) {
      if (button == XRButton.Touchpad) return GvrControllerInput.TouchUp;
      else return false;
    }

    protected override void GenerateCurrentStatus() {
      cPos = Camera.main.transform.rotation * (hand == XRHand.Left ? new Vector3(-0.2f, -0.3f, 0.4f) : new Vector3(0.2f, -0.3f, 0.4f));
      cRot = Camera.main.transform.rotation * GvrControllerInput.Orientation;

      cTouchpadAxis = GvrControllerInput.TouchPos;
    }
  }
#else
  public class DaydreamControllerStatus : BrokenControllerStatus {
    public DaydreamControllerStatus(XRHand hand) : base(hand, "Daydream") { }
  }
#endif
}

