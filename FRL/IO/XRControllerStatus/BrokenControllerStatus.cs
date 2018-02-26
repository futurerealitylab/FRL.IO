using UnityEngine;

namespace FRL.IO {
  public class BrokenControllerStatus : XRControllerStatus {

    private string message;

    public BrokenControllerStatus(XRHand hand, string msg) : base(hand) {
      this.message = msg;
    }

    private void LogMessage() {
      Debug.LogError("Error! You're missing something for: " + this.message);
    }

    public override bool GetClick(XRButton button) {
      LogMessage();
      return false;
    }

    public override bool GetPress(XRButton button) {
      LogMessage();
      return false;
    }

    public override bool GetPressDown(XRButton button) {
      LogMessage();
      return false;
    }

    public override bool GetPressUp(XRButton button) {
      LogMessage();
      return false;
    }

    public override bool GetTouch(XRButton button) {
      LogMessage();
      return false;
    }

    public override bool GetTouchDown(XRButton button) {
      LogMessage();
      return false;
    }

    public override bool GetTouchUp(XRButton button) {
      LogMessage();
      return false;
    }

    protected override void GenerateCurrentStatus() {
      LogMessage();
    }
  }
}


