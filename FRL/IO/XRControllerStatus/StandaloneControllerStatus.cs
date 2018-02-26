using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public class StandaloneControllerStatus : XRControllerStatus {
    
    public override bool GetClick(XRButton button) {
      return false;
    }

    public override bool GetPress(XRButton button) {
      return false;
    }

    public override bool GetPressDown(XRButton button) {
      return false;
    }

    public override bool GetPressUp(XRButton button) {
      return false;
    }

    public override bool GetTouch(XRButton button) {
      return false;
    }

    public override bool GetTouchDown(XRButton button) {
      return false;
    }

    public override bool GetTouchUp(XRButton button) {
      return false;
    }

    protected override void GenerateCurrentStatus() {
    }
  }
}

