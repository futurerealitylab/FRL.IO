using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace FRL.IO {
  public class WMRControllerStatus : XRControllerStatus {

    private static Dictionary<XRButton, KeyCode> leftPressMappings = new Dictionary<XRButton, KeyCode>() {
      {XRButton.Menu, KeyCode.JoystickButton6},
      {XRButton.Thumbstick, KeyCode.JoystickButton8},
      {XRButton.Touchpad, KeyCode.JoystickButton16},
    };

    private static Dictionary<XRButton, KeyCode> rightPressMappings = new Dictionary<XRButton, KeyCode>() {
      {XRButton.Menu, KeyCode.JoystickButton7},
      {XRButton.Thumbstick, KeyCode.JoystickButton9},
      {XRButton.Touchpad, KeyCode.JoystickButton17},
    };

    private static Dictionary<XRButton, KeyCode> leftTouchMappings = new Dictionary<XRButton, KeyCode>() {
      {XRButton.Touchpad, KeyCode.JoystickButton18},
    };

    private static Dictionary<XRButton, KeyCode> rightTouchMappings = new Dictionary<XRButton, KeyCode>() {
      {XRButton.Touchpad, KeyCode.JoystickButton19},
    };

    private XRNode node;
    private Dictionary<XRButton, KeyCode> touchMappings;
    private Dictionary<XRButton, KeyCode> pressMappings;

    public WMRControllerStatus(XRHand hand) : base(hand) {
      node = hand == XRHand.Left ? XRNode.LeftHand : XRNode.RightHand;
      this.touchMappings = hand == XRHand.Left ? leftTouchMappings : rightTouchMappings;
      this.pressMappings = hand == XRHand.Left ? leftPressMappings : rightPressMappings;
    }

    protected override void GenerateCurrentStatus() {
      //Position and Rotation
      cPos = InputTracking.GetLocalPosition(node);
      cRot = InputTracking.GetLocalRotation(node);

      //Buttons
      cGripAxis = Input.GetAxis((hand == XRHand.Left ? "L" : "R") + "Grip");
      cTriggerAxis = Input.GetAxis((hand == XRHand.Left ? "L" : "R") + "Trigger");
      string handLabel = this.hand == XRHand.Left ? "L" : "R";
      cThumbstickAxis = new Vector2(Input.GetAxis(handLabel + "ThumbstickX"), Input.GetAxis(handLabel + "ThumbstickY")); ;
      cTouchpadAxis = new Vector2(Input.GetAxis("WMR_" + handLabel + "TouchpadX"), Input.GetAxis("WMR_" + handLabel + "TouchpadY"));
    }

    public override bool GetClick(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return pTriggerAxis < 1f && cTriggerAxis == 1f;
        case XRButton.Grip:
          return pGripAxis < 1f && cGripAxis == 1f;
        case XRButton.Thumbstick:
          return GetPressDown(button);
      }
      return false;
    }

    public override bool GetPress(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return pTriggerAxis >= 0.5f && cTriggerAxis >= 0.5f;
        case XRButton.Grip:
          return pGripAxis >= 0.5f && cGripAxis >= 0.5f;
        case XRButton.Forward:
          return pThumbstickAxis.y >= 0.5f && cThumbstickAxis.y >= 0.5f;
        case XRButton.Back:
          return pThumbstickAxis.y <= -0.5f && cThumbstickAxis.y <= -0.5f;
        case XRButton.Left:
          return pThumbstickAxis.x <= -0.5f && cThumbstickAxis.x <= -0.5f;
        case XRButton.Right:
          return pThumbstickAxis.x >= 0.5f && cThumbstickAxis.x >= 0.5f;
      }
      if (!pressMappings.ContainsKey(button)) return false;
      return Input.GetKey(pressMappings[button]);
    }

    public override bool GetPressDown(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return pTriggerAxis < 0.5f && cTriggerAxis >= 0.5f;
        case XRButton.Grip:
          return pGripAxis < 0.5f && cGripAxis >= 0.5f;
        case XRButton.Forward:
          return pThumbstickAxis.y < 0.5f && cThumbstickAxis.y >= 0.5f;
        case XRButton.Back:
          return pThumbstickAxis.y > -0.5f && cThumbstickAxis.y <= -0.5f;
        case XRButton.Left:
          return pThumbstickAxis.x > -0.5f && cThumbstickAxis.x <= -0.5f;
        case XRButton.Right:
          return pThumbstickAxis.x < 0.5f && cThumbstickAxis.x >= 0.5f;
      }
      if (!pressMappings.ContainsKey(button)) return false;
      return Input.GetKeyDown(pressMappings[button]);
    }

    public override bool GetPressUp(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return pTriggerAxis >= 0.5f && cTriggerAxis < 0.5f;
        case XRButton.Grip:
          return pGripAxis >= 0.5f && cGripAxis <= 0.5f;
        case XRButton.Forward:
          return pThumbstickAxis.y >= 0.5f && cThumbstickAxis.y < 0.5f;
        case XRButton.Back:
          return pThumbstickAxis.y <= -0.5f && cThumbstickAxis.y > -0.5f;
        case XRButton.Left:
          return pThumbstickAxis.x <= -0.5f && cThumbstickAxis.x > -0.5f;
        case XRButton.Right:
          return pThumbstickAxis.x >= 0.5f && cThumbstickAxis.x < 0.5f;
      }
      if (!pressMappings.ContainsKey(button)) return false;
      return Input.GetKeyUp(pressMappings[button]);
    }

    public override bool GetTouch(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return pTriggerAxis > 0f && cTriggerAxis > 0f;
        case XRButton.Grip:
          return pGripAxis > 0f && cGripAxis > 0f;
      }
      if (!touchMappings.ContainsKey(button)) return false;
      return Input.GetKey(touchMappings[button]);
    }

    public override bool GetTouchDown(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return pTriggerAxis == 0f && cTriggerAxis > 0f;
        case XRButton.Grip:
          return pGripAxis == 0f && cGripAxis > 0f;
      }
      if (!touchMappings.ContainsKey(button)) return false;
      return Input.GetKeyDown(touchMappings[button]);
    }

    public override bool GetTouchUp(XRButton button) {
      switch (button) {
        case XRButton.Trigger:
          return pTriggerAxis > 0f && cTriggerAxis == 0f;
        case XRButton.Grip:
          return pGripAxis > 0f && cGripAxis == 0f;
      }
      if (!touchMappings.ContainsKey(button)) return false;
      return Input.GetKeyUp(touchMappings[button]);
    }
  }
}

