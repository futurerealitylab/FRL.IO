using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
#if DAYDREAM
  public class DaydreamControllerStatus : XRControllerStatus {
    public override bool GetClick(XRButton button) {
      throw new System.NotImplementedException();
    }

    public override bool GetPress(XRButton button) {
      throw new System.NotImplementedException();
    }

    public override bool GetPressDown(XRButton button) {
      throw new System.NotImplementedException();
    }

    public override bool GetPressUp(XRButton button) {
      throw new System.NotImplementedException();
    }

    public override bool GetTouch(XRButton button) {
      throw new System.NotImplementedException();
    }

    public override bool GetTouchDown(XRButton button) {
      throw new System.NotImplementedException();
    }

    public override bool GetTouchUp(XRButton button) {
      throw new System.NotImplementedException();
    }

    protected override void GenerateCurrentStatus() {
      throw new System.NotImplementedException();
    }
  }
#else
  public class DaydreamControllerStatus : BrokenControllerStatus {
    public DaydreamControllerStatus(XRHand hand) : base(hand, "Daydream") { }
  }
#endif
}

