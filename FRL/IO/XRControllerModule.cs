using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace FRL.IO {

  public enum Hand { Left, Right };
  public enum XRButton {
    Trigger, Grip, Touchpad, Menu, Thumbstick, A, B, X, Y, Forward, Back, Left, Right
  }

  public class XREventData : PointerEventData {
    public Hand hand;
    public Vector2 touchpadAxis, thumbstickAxis;
    public float gripAxis, triggerAxis;
    public Vector3 velocity, acceleration;

    public GameObject triggerPress, triggerTouch;
    public GameObject gripPress, gripTouch;
    public GameObject menuPress, menuTouch;
    public GameObject touchpadPress, touchpadTouch;
    public GameObject thumbstickPress, thumbstickTouch;
    public GameObject aPress, aTouch;
    public GameObject bPress, bTouch;
    public GameObject xPress, xTouch;
    public GameObject yPress, yTouch;
    public GameObject forwardPress, backPress, leftPress, rightPress;

    internal XREventData(BaseInputModule module) : base(module) { }

    internal override void Reset() {
      base.Reset();
      touchpadAxis = thumbstickAxis = Vector2.zero;
      gripAxis = triggerAxis = 0f;
      triggerPress = triggerTouch = null;
      gripPress = gripTouch = null;
      menuPress = menuTouch = null;
      touchpadPress = touchpadTouch = null;
      aPress = aTouch = null;
      bPress = bTouch = null;
      xPress = xTouch = null;
      yPress = yTouch = null;
      forwardPress = backPress = leftPress = rightPress = null;
      velocity = acceleration = Vector3.zero;
    }
  }

  public class XRControllerModule : PointerInputModule {

    public Hand hand;

    public XRSystem System;

    private float previousTriggerAxis, previousGripAxis;
    private Vector2 previousTouchpadAxis, previousThumbstickAxis;

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousVelocity, previousAcceleration;

    private Dictionary<XRButton, GameObject> pressPairings = new Dictionary<XRButton, GameObject>();
    private Dictionary<XRButton, List<Receiver>> pressReceivers = new Dictionary<XRButton, List<Receiver>>();
    private Dictionary<XRButton, GameObject> touchPairings = new Dictionary<XRButton, GameObject>();
    private Dictionary<XRButton, List<Receiver>> touchReceivers = new Dictionary<XRButton, List<Receiver>>();

    private Dictionary<XRSystem, Dictionary<XRButton, KeyCode>> leftPressMappings = new Dictionary<XRSystem, Dictionary<XRButton,KeyCode>>() {
      {XRSystem.CV1, new Dictionary<XRButton, KeyCode> {
        {XRButton.Menu, KeyCode.JoystickButton7},
        {XRButton.Thumbstick, KeyCode.JoystickButton8},
        {XRButton.X, KeyCode.JoystickButton2},
        {XRButton.Y, KeyCode.JoystickButton3},
      }},
      {XRSystem.WindowsMR, new Dictionary<XRButton, KeyCode> {
        {XRButton.Menu, KeyCode.JoystickButton6},
        {XRButton.Thumbstick, KeyCode.JoystickButton8},
        {XRButton.Touchpad, KeyCode.JoystickButton16},
      }},
      {XRSystem.Vive, new Dictionary<XRButton, KeyCode> {
        {XRButton.Menu, KeyCode.JoystickButton2},
        {XRButton.Touchpad, KeyCode.JoystickButton8},
      }},
    };

    private Dictionary<XRSystem, Dictionary<XRButton, KeyCode>> leftTouchMappings = new Dictionary<XRSystem, Dictionary<XRButton, KeyCode>>() {
      {XRSystem.CV1, new Dictionary<XRButton, KeyCode> {
        {XRButton.Thumbstick, KeyCode.JoystickButton16},
        {XRButton.X, KeyCode.JoystickButton12},
        {XRButton.Y, KeyCode.JoystickButton13},
        {XRButton.Touchpad, KeyCode.JoystickButton18},
        //{XRButton.Trigger, KeyCode.JoystickButton14},
      }},
      {XRSystem.WindowsMR, new Dictionary<XRButton, KeyCode> {
        {XRButton.Touchpad, KeyCode.JoystickButton18},
      }},
      {XRSystem.Vive, new Dictionary<XRButton, KeyCode> {
        {XRButton.Touchpad, KeyCode.JoystickButton16},
        //{XRButton.Trigger, KeyCode.JoystickButton14}
      }}
    };

    private Dictionary<XRSystem, Dictionary<XRButton, KeyCode>> rightPressMappings = new Dictionary<XRSystem, Dictionary<XRButton, KeyCode>>() {
      {XRSystem.CV1, new Dictionary<XRButton, KeyCode> {
        {XRButton.Thumbstick, KeyCode.JoystickButton9},
        {XRButton.A, KeyCode.JoystickButton0},
        {XRButton.B, KeyCode.JoystickButton1}
      }},
      {XRSystem.WindowsMR, new Dictionary<XRButton, KeyCode> {
        {XRButton.Menu, KeyCode.JoystickButton7},
        {XRButton.Thumbstick, KeyCode.JoystickButton9},
        {XRButton.Touchpad, KeyCode.JoystickButton17},
      }},
      {XRSystem.Vive, new Dictionary<XRButton, KeyCode> {
        {XRButton.Menu, KeyCode.JoystickButton0},
        {XRButton.Touchpad, KeyCode.JoystickButton9}
      }}
    };

   private Dictionary<XRSystem, Dictionary<XRButton, KeyCode>> rightTouchMappings = new Dictionary<XRSystem, Dictionary<XRButton, KeyCode>>() {
      {XRSystem.CV1, new Dictionary<XRButton, KeyCode> {
        {XRButton.Thumbstick, KeyCode.JoystickButton17},
        {XRButton.X, KeyCode.JoystickButton10},
        {XRButton.Y, KeyCode.JoystickButton11},
        {XRButton.Touchpad, KeyCode.JoystickButton19},
        //{XRButton.Trigger, KeyCode.JoystickButton15},
      }},
      {XRSystem.WindowsMR, new Dictionary<XRButton, KeyCode> {
        {XRButton.Touchpad, KeyCode.JoystickButton19},
      }},
      {XRSystem.Vive, new Dictionary<XRButton, KeyCode> {
        {XRButton.Touchpad, KeyCode.JoystickButton17},
        //{XRButton.Trigger, KeyCode.JoystickButton15}
      }}
    };


    protected override PointerEventData pointerEventData {
      get {
        return xrEventData;
      }
    }

    private XREventData eventData;
    public XREventData xrEventData {
      get { return eventData; }
    }

    private bool isTracked = true;
    public bool IsTracked {
      get { return isTracked; }
    }

    public static XRButton[] XRButtons {
      get { return (XRButton[])Enum.GetValues(typeof(XRButton)); }
    }

    protected override void Awake() {
      base.Awake();
      eventData = new XREventData(this);
      foreach (XRButton button in XRButtons) {
        pressPairings.Add(button, null);
        pressReceivers.Add(button, null);
        touchPairings.Add(button, null);
        touchReceivers.Add(button, null);
      }
    }

    protected override void OnDisable() {
      base.OnDisable();
      foreach (XRButton button in XRButtons) {
        ExecutePressUp(button);
        ExecuteGlobalPressUp(button);
        ExecuteTouchUp(button);
        ExecuteGlobalTouchUp(button);
      }
      xrEventData.Reset();
    }

    protected override void Process() {
      PlaceController();
      base.Process();
      xrEventData.hand = this.hand;

      xrEventData.velocity = (transform.position - previousPosition) / Time.deltaTime;
      xrEventData.acceleration = (xrEventData.velocity - previousVelocity) / Time.deltaTime;

      previousVelocity = xrEventData.velocity;
      previousAcceleration = xrEventData.acceleration;

      this.HandleButtons();
    }


    void HandleButtons() {
      previousTouchpadAxis = xrEventData.touchpadAxis;
      previousThumbstickAxis = xrEventData.thumbstickAxis;
      previousGripAxis = xrEventData.gripAxis;
      previousTriggerAxis = xrEventData.triggerAxis;


      xrEventData.triggerAxis = GetTriggerAxis();
      xrEventData.gripAxis = GetGripAxis();
      xrEventData.thumbstickAxis = GetThumbstickAxis();
      xrEventData.touchpadAxis = GetTouchpadAxis();

      if (previousTriggerAxis != 1f && xrEventData.triggerAxis == 1f) {
        ExecuteTriggerClick();
      }
      if (previousGripAxis != 1f && xrEventData.gripAxis == 1f) {
        ExecuteGripClick();
      }

      foreach (XRButton button in XRButtons) {
        if (GetPressDown(button)) {
          ExecutePressDown(button);
          ExecuteGlobalPressDown(button);
        }
        if (GetPress(button)) {
          ExecutePress(button);
          ExecuteGlobalPress(button);
        }
        if (GetPressUp(button)) {
          ExecutePressUp(button);
          ExecuteGlobalPressUp(button);
        }
        if (GetTouchDown(button)) {
          ExecuteTouchDown(button);
          ExecuteGlobalTouchDown(button);
        }
        if (GetTouch(button)) {
          ExecuteTouch(button);
          ExecuteGlobalTouch(button);
        }
        if (GetTouchUp(button)) {
          ExecuteTouchUp(button);
          ExecuteGlobalTouchUp(button);
        }
      }
    }

    public void PlaceController() {
      previousPosition = this.transform.localPosition;
      previousRotation = this.transform.localRotation;
      switch (System) {
        case XRSystem.CV1:
        case XRSystem.Vive:
          PlaceXR();
          break;
        case XRSystem.WindowsMR:
          PlaceWMR();
          break;
        case XRSystem.GearVR:
          PlaceGearVR();
          break;
        default:
          isTracked = false;
          break;
      }
    }

    public void PlaceXR() {
      var node = hand == Hand.Left ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand;
      this.transform.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(node);
      this.transform.localRotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);
      isTracked = previousPosition != this.transform.localPosition && previousRotation != this.transform.localRotation;
    }

    public void PlaceWMR() {
      UnityEngine.XR.XRNode controller = hand == Hand.Left ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand;
      this.transform.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(controller);
      this.transform.localRotation = UnityEngine.XR.InputTracking.GetLocalRotation(controller);
      isTracked = this.transform.localPosition != previousPosition || this.transform.localRotation != previousRotation;
    }

    public void PlaceGearVR() {
      OVRInput.Controller controller = OVRInput.GetActiveController();
      Vector3 position = OVRInput.GetLocalControllerPosition(controller);
      Quaternion rotation = OVRInput.GetLocalControllerRotation(controller);
      this.transform.localPosition = Camera.main.transform.position + position;
      this.transform.localRotation = rotation;
      isTracked = OVRInput.GetActiveController() == controller;
    }

    public Vector2 GetThumbstickAxis() {
      string handLabel = hand == Hand.Left ? "L" : "R";
      switch (System) {
        case XRSystem.CV1:
        case XRSystem.WindowsMR:
          return new Vector2(Input.GetAxis(handLabel + "ThumbstickX"), Input.GetAxis(handLabel + "ThumbstickY"));;
        default:
          return Vector2.zero;
      }
    }

    public Vector2 GetTouchpadAxis() {
      string xLabel;
      string yLabel;
      string handLabel = hand == Hand.Left ? "L" : "R";
      switch (System) {
        case XRSystem.Vive:
          xLabel = handLabel + "ThumbstickX";
          yLabel = handLabel + "ThumbstickY";
          break;
        case XRSystem.WindowsMR:
          xLabel = "WMR_" + handLabel + "TouchpadX";
          yLabel = "WMR_" + handLabel + "TouchpadY";
          break;
        case XRSystem.GearVR:
          return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        default:
          return Vector2.zero;
      }
      return new Vector2(Input.GetAxis(xLabel), Input.GetAxis(yLabel));
    }

    public float GetTriggerAxis() {
      switch (System) {
        case XRSystem.Vive:
        case XRSystem.CV1:
        case XRSystem.WindowsMR:
          return Input.GetAxis((hand == Hand.Left ? "L" : "R") + "Trigger");
        case XRSystem.GearVR:
          return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        default:
          return 0;
      }
    }

    public float GetGripAxis() {
      switch (System) {
        case XRSystem.Vive:
        case XRSystem.CV1:
        case XRSystem.WindowsMR:
          return Input.GetAxis((hand == Hand.Left ? "L" : "R") + "Grip");
        default:
          return 0;
      }
    }

    protected bool GetPressDown(XRButton button) {
      if (System == XRSystem.GearVR) {
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

      KeyCode key = KeyCode.None;
      var mapping = hand == Hand.Left ? leftPressMappings : rightPressMappings;
      if (mapping.ContainsKey(System)) {
        var keys = mapping[System];
        switch (button) {
          case XRButton.Trigger:
            return previousTriggerAxis < 0.5f && xrEventData.triggerAxis >= 0.5f;
          case XRButton.Grip:
            return previousGripAxis < 0.5f && xrEventData.gripAxis >= 0.5f;
          case XRButton.Forward:
            return previousThumbstickAxis.y < 0.5f && xrEventData.thumbstickAxis.y >= 0.5f;
          case XRButton.Back:
            return previousThumbstickAxis.y > -0.5f && xrEventData.thumbstickAxis.y <= -0.5f;
          case XRButton.Left:
            return previousThumbstickAxis.x > -0.5f && xrEventData.thumbstickAxis.x <= -0.5f;
          case XRButton.Right:
            return previousThumbstickAxis.x < 0.5f && xrEventData.thumbstickAxis.x >= 0.5f;
        }
        if (keys.ContainsKey(button))
          key = keys[button];
      }
      return Input.GetKeyDown(key);
    }

    protected bool GetPress(XRButton button) {
      if (System == XRSystem.GearVR) {
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

      KeyCode key = KeyCode.None;
      var mapping = hand == Hand.Left ? leftPressMappings : rightPressMappings;
      if (mapping.ContainsKey(System)) {
        var keys = mapping[System];
        switch (button) {
          case XRButton.Trigger:
            return previousTriggerAxis >= 0.5f && xrEventData.triggerAxis >= 0.5f;
          case XRButton.Grip:
            return previousGripAxis >= 0.5f && xrEventData.gripAxis >= 0.5f;
          case XRButton.Forward:
            return previousThumbstickAxis.y >= 0.5f && xrEventData.thumbstickAxis.y >= 0.5f;
          case XRButton.Back:
            return previousThumbstickAxis.y <= -0.5f && xrEventData.thumbstickAxis.y <= -0.5f;
          case XRButton.Left:
            return previousThumbstickAxis.x <= -0.5f && xrEventData.thumbstickAxis.x <= -0.5f;
          case XRButton.Right:
            return previousThumbstickAxis.x >= 0.5f && xrEventData.thumbstickAxis.x >= 0.5f;
        }
        if (keys.ContainsKey(button))
          key = keys[button];
      }
      return Input.GetKey(key);
    }

    protected bool GetPressUp(XRButton button) {
      if (System == XRSystem.GearVR) {
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

      KeyCode key = KeyCode.None;
      var mapping = hand == Hand.Left ? leftPressMappings : rightPressMappings;
      if (mapping.ContainsKey(System)) {
        var keys = mapping[System];
        switch (button) {
          case XRButton.Trigger:
            return previousTriggerAxis >= 0.5f && xrEventData.triggerAxis < 0.5f;
          case XRButton.Grip:
            return previousGripAxis >= 0.5f && xrEventData.gripAxis <= 0.5f;
          case XRButton.Forward:
            return previousThumbstickAxis.y >= 0.5f && xrEventData.thumbstickAxis.y < 0.5f;
          case XRButton.Back:
            return previousThumbstickAxis.y <= -0.5f && xrEventData.thumbstickAxis.y > -0.5f;
          case XRButton.Left:
            return previousThumbstickAxis.x <= -0.5f && xrEventData.thumbstickAxis.x > -0.5f;
          case XRButton.Right:
            return previousThumbstickAxis.x >= 0.5f && xrEventData.thumbstickAxis.x < 0.5f;
        }
        if (keys.ContainsKey(button))
          key = keys[button];
      }
      return Input.GetKeyUp(key);
    }

    protected bool GetTouchDown(XRButton button) {
      if (System == XRSystem.GearVR && button == XRButton.Touchpad)
        return OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad);

      KeyCode key = KeyCode.None;
      var mapping = hand == Hand.Left ? leftTouchMappings : rightTouchMappings;
      if (mapping.ContainsKey(System)) {
        var keys = mapping[System];
        switch (button) {
          case XRButton.Trigger:
            return previousTriggerAxis == 0f && xrEventData.triggerAxis > 0f;
          case XRButton.Grip:
            return previousGripAxis == 0f && xrEventData.gripAxis > 0f;
        }
        if (keys.ContainsKey(button))
          key = keys[button];
      }
      return Input.GetKeyDown(key);
    }

    protected bool GetTouch(XRButton button) {
      if (System == XRSystem.GearVR && button == XRButton.Touchpad)
        return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad);

      KeyCode key = KeyCode.None;
      var mapping = hand == Hand.Left ? leftTouchMappings : rightTouchMappings;
      if (mapping.ContainsKey(System)) {
        var keys = mapping[System];
        switch (button) {
          case XRButton.Trigger:
            return previousTriggerAxis > 0f && xrEventData.triggerAxis > 0f;
          case XRButton.Grip:
            return previousGripAxis > 0f && xrEventData.gripAxis > 0f;
        }
        if (keys.ContainsKey(button))
          key = keys[button];
      }
      return Input.GetKey(key);
    }

    protected bool GetTouchUp(XRButton button) {
      if (System == XRSystem.GearVR && button == XRButton.Touchpad)
        return OVRInput.GetUp(OVRInput.Touch.PrimaryTouchpad);

      KeyCode key = KeyCode.None;
      var mapping = hand == Hand.Left ? leftTouchMappings : rightTouchMappings;
      if (mapping.ContainsKey(System)) {
        var keys = mapping[System];
        switch (button) {
          case XRButton.Trigger:
            return previousTriggerAxis > 0f && xrEventData.triggerAxis == 0f;
          case XRButton.Grip:
            return previousGripAxis > 0f && xrEventData.gripAxis == 0f;
        }
        if (keys.ContainsKey(button))
          key = keys[button];
      }
      return Input.GetKeyUp(key);
    }

    protected bool GetClick(XRButton button) {
      return false;
    }


    private void ExecuteTriggerClick() {
      if (xrEventData.triggerPress != null) {
        ExecuteEvents.Execute<IPointerTriggerClickHandler>(xrEventData.triggerPress, xrEventData, (x, y) => {
          x.OnPointerTriggerClick(xrEventData);
        });
      }
      foreach (Receiver r in pressReceivers[XRButton.Trigger])
        if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
          ExecuteEvents.Execute<IGlobalTriggerClickHandler>(r.gameObject, xrEventData,
            (x, y) => x.OnGlobalTriggerClick(xrEventData));
    }

    private void ExecuteGripClick() {
      if (xrEventData.gripPress != null) {
        ExecuteEvents.Execute<IPointerGripClickHandler>(xrEventData.gripPress, xrEventData, (x, y) => {
          x.OnPointerGripClick(xrEventData);
        });
      }
      foreach (Receiver r in pressReceivers[XRButton.Grip])
        if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
          ExecuteEvents.Execute<IGlobalGripClickHandler>(r.gameObject, xrEventData,
            (x, y) => x.OnGlobalGripClick(xrEventData));
    }

    private void ExecutePressDown(XRButton id) {
      GameObject go = xrEventData.currentRaycast;
      if (go == null)
        return;

      //If there's a receiver component and it has a module, only cast to it if it's this module.
      Receiver r = go.GetComponent<Receiver>();
      if (r != null && r.module != null && r.module != this)
        return;

      switch (id) {
        case XRButton.Trigger:
          xrEventData.triggerPress = go;
          ExecuteEvents.Execute<IPointerTriggerPressDownHandler>(xrEventData.triggerPress, xrEventData,
            (x, y) => x.OnPointerTriggerPressDown(xrEventData));
          break;
        case XRButton.Grip:
          xrEventData.gripPress = go;
          ExecuteEvents.Execute<IPointerGripPressDownHandler>(xrEventData.gripPress, xrEventData,
            (x, y) => x.OnPointerGripPressDown(xrEventData));
          break;
        case XRButton.A:
          xrEventData.aPress = go;
          ExecuteEvents.Execute<IPointerAPressDownHandler>(xrEventData.aPress, xrEventData,
            (x, y) => x.OnPointerAPressDown(xrEventData));
          break;
        case XRButton.B:
          xrEventData.bPress = go;
          ExecuteEvents.Execute<IPointerBPressDownHandler>(xrEventData.bPress, xrEventData,
            (x, y) => x.OnPointerBPressDown(xrEventData));
          break;
        case XRButton.X:
          xrEventData.xPress = go;
          ExecuteEvents.Execute<IPointerXPressDownHandler>(xrEventData.xPress, xrEventData,
            (x, y) => x.OnPointerXPressDown(xrEventData));
          break;
        case XRButton.Y:
          xrEventData.yPress = go;
          ExecuteEvents.Execute<IPointerYPressDownHandler>(xrEventData.yPress, xrEventData,
            (x, y) => x.OnPointerYPressDown(xrEventData));
          break;
        case XRButton.Thumbstick:
          xrEventData.thumbstickPress = go;
          ExecuteEvents.Execute<IPointerThumbstickPressDownHandler>(xrEventData.thumbstickPress, xrEventData,
            (x, y) => x.OnPointerThumbstickPressDown(xrEventData));
          break;
        case XRButton.Touchpad:
          xrEventData.touchpadPress = go;
          ExecuteEvents.Execute<IPointerTouchpadPressDownHandler>(xrEventData.touchpadPress, xrEventData,
            (x, y) => x.OnPointerTouchpadPressDown(xrEventData));
          break;
        case XRButton.Menu:
          xrEventData.menuPress = go;
          ExecuteEvents.Execute<IPointerMenuPressDownHandler>(xrEventData.menuPress, xrEventData,
            (x, y) => x.OnPointerMenuPressDown(xrEventData));
          break;
        case XRButton.Forward:
          xrEventData.forwardPress = go;
          ExecuteEvents.Execute<IPointerForwardDownHandler>(xrEventData.forwardPress, xrEventData,
            (x, y) => x.OnPointerForwardDown(xrEventData));
          break;
        case XRButton.Back:
          xrEventData.backPress = go;
          ExecuteEvents.Execute<IPointerBackDownHandler>(xrEventData.backPress, xrEventData,
            (x, y) => x.OnPointerBackDown(xrEventData));
          break;
        case XRButton.Left:
          xrEventData.leftPress = go;
          ExecuteEvents.Execute<IPointerLeftDownHandler>(xrEventData.leftPress, xrEventData,
            (x, y) => x.OnPointerLeftDown(xrEventData));
          break;
        case XRButton.Right:
          xrEventData.rightPress = go;
          ExecuteEvents.Execute<IPointerRightDownHandler>(xrEventData.rightPress, xrEventData,
            (x, y) => x.OnPointerRightDown(xrEventData));
          break;
      }
      //Add pairing.
      pressPairings[id] = go;
    }

    private void ExecutePress(XRButton id) {
      if (pressPairings[id] == null)
        return;

      switch (id) {
        case XRButton.Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressHandler>(xrEventData.triggerPress, xrEventData,
            (x, y) => x.OnPointerTriggerPress(xrEventData));
          break;
        case XRButton.Grip:
          ExecuteEvents.Execute<IPointerGripPressHandler>(xrEventData.gripPress, xrEventData,
            (x, y) => x.OnPointerGripPress(xrEventData));
          break;
        case XRButton.A:
          ExecuteEvents.Execute<IPointerAPressHandler>(xrEventData.aPress, xrEventData,
            (x, y) => x.OnPointerAPress(xrEventData));
          break;
        case XRButton.B:
          ExecuteEvents.Execute<IPointerBPressHandler>(xrEventData.bPress, xrEventData,
            (x, y) => x.OnPointerBPress(xrEventData));
          break;
        case XRButton.X:
          ExecuteEvents.Execute<IPointerXPressHandler>(xrEventData.xPress, xrEventData,
            (x, y) => x.OnPointerXPress(xrEventData));
          break;
        case XRButton.Y:
          ExecuteEvents.Execute<IPointerYPressHandler>(xrEventData.yPress, xrEventData,
            (x, y) => x.OnPointerYPress(xrEventData));
          break;
        case XRButton.Thumbstick:
          ExecuteEvents.Execute<IPointerThumbstickPressHandler>(xrEventData.thumbstickPress, xrEventData,
            (x, y) => x.OnPointerThumbstickPress(xrEventData));
          break;
        case XRButton.Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressHandler>(xrEventData.touchpadPress, xrEventData,
            (x, y) => x.OnPointerTouchpadPress(xrEventData));
          break;
        case XRButton.Menu:
          ExecuteEvents.Execute<IPointerMenuPressHandler>(xrEventData.menuPress, xrEventData,
            (x, y) => x.OnPointerMenuPress(xrEventData));
          break;
        case XRButton.Forward:
          ExecuteEvents.Execute<IPointerForwardHandler>(xrEventData.forwardPress, xrEventData,
            (x, y) => x.OnPointerForward(xrEventData));
          break;
        case XRButton.Back:
          ExecuteEvents.Execute<IPointerBackHandler>(xrEventData.backPress, xrEventData,
            (x, y) => x.OnPointerBack(xrEventData));
          break;
        case XRButton.Left:
          ExecuteEvents.Execute<IPointerLeftHandler>(xrEventData.leftPress, xrEventData,
            (x, y) => x.OnPointerLeft(xrEventData));
          break;
        case XRButton.Right:
          ExecuteEvents.Execute<IPointerRightHandler>(xrEventData.rightPress, xrEventData,
            (x, y) => x.OnPointerRight(xrEventData));
          break;
      }
    }

    private void ExecutePressUp(XRButton id) {
      if (pressPairings[id] == null)
        return;

      switch (id) {
        case XRButton.Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressUpHandler>(xrEventData.triggerPress, xrEventData,
            (x, y) => x.OnPointerTriggerPressUp(xrEventData));
          xrEventData.triggerPress = null;
          break;
        case XRButton.Grip:
          ExecuteEvents.Execute<IPointerGripPressUpHandler>(xrEventData.gripPress, xrEventData,
            (x, y) => x.OnPointerGripPressUp(xrEventData));
          xrEventData.gripPress = null;
          break;
        case XRButton.A:
          ExecuteEvents.Execute<IPointerAPressUpHandler>(xrEventData.aPress, xrEventData,
            (x, y) => x.OnPointerAPressUp(xrEventData));
          xrEventData.aPress = null;
          break;
        case XRButton.B:
          ExecuteEvents.Execute<IPointerBPressUpHandler>(xrEventData.bPress, xrEventData,
            (x, y) => x.OnPointerBPressUp(xrEventData));
          xrEventData.bPress = null;
          break;
        case XRButton.X:
          ExecuteEvents.Execute<IPointerXPressUpHandler>(xrEventData.xPress, xrEventData,
            (x, y) => x.OnPointerXPressUp(xrEventData));
          xrEventData.xPress = null;
          break;
        case XRButton.Y:
          ExecuteEvents.Execute<IPointerYPressUpHandler>(xrEventData.yPress, xrEventData,
            (x, y) => x.OnPointerYPressUp(xrEventData));
          xrEventData.yPress = null;
          break;
        case XRButton.Thumbstick:
          ExecuteEvents.Execute<IPointerThumbstickPressUpHandler>(xrEventData.thumbstickPress, xrEventData,
            (x, y) => x.OnPointerThumbstickPressUp(xrEventData));
          xrEventData.thumbstickPress = null;
          break;
        case XRButton.Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressUpHandler>(xrEventData.touchpadPress, xrEventData,
            (x, y) => x.OnPointerTouchpadPressUp(xrEventData));
          break;
        case XRButton.Menu:
          ExecuteEvents.Execute<IPointerMenuPressUpHandler>(xrEventData.menuPress, xrEventData,
            (x, y) => x.OnPointerMenuPressUp(xrEventData));
          break;
        case XRButton.Forward:
          ExecuteEvents.Execute<IPointerForwardUpHandler>(xrEventData.forwardPress, xrEventData,
            (x, y) => x.OnPointerForwardUp(xrEventData));
          xrEventData.forwardPress = null;
          break;
        case XRButton.Back:
          ExecuteEvents.Execute<IPointerBackUpHandler>(xrEventData.backPress, xrEventData,
            (x, y) => x.OnPointerBackUp(xrEventData));
          xrEventData.backPress = null;
          break;
        case XRButton.Left:
          ExecuteEvents.Execute<IPointerLeftUpHandler>(xrEventData.leftPress, xrEventData,
            (x, y) => x.OnPointerLeftUp(xrEventData));
          xrEventData.leftPress = null;
          break;
        case XRButton.Right:
          ExecuteEvents.Execute<IPointerRightUpHandler>(xrEventData.rightPress, xrEventData,
            (x, y) => x.OnPointerRightUp(xrEventData));
          xrEventData.rightPress = null;
          break;
      }
      //Remove pairing.
      pressPairings[id] = null;
    }

    private void ExecuteGlobalPressDown(XRButton id) {
      //Add paired list.
      pressReceivers[id] = Receiver.instances;

      switch (id) {
        case XRButton.Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTriggerPressDown(xrEventData));
          break;
        case XRButton.Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalGripPressDown(xrEventData));
          break;
        case XRButton.A:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalAPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalAPressDown(xrEventData));
          break;
        case XRButton.B:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBPressDown(xrEventData));
          break;
        case XRButton.X:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalXPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalXPressDown(xrEventData));
          break;
        case XRButton.Y:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalYPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalYPressDown(xrEventData));
          break;
        case XRButton.Thumbstick:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalThumbstickPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalThumbstickPressDown(xrEventData));
          break;
        case XRButton.Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTouchpadPressDown(xrEventData));
          break;
        case XRButton.Menu:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalMenuPressDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalMenuPressDown(xrEventData));
          break;
        case XRButton.Forward:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalForwardDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalForwardDown(xrEventData));
          break;
        case XRButton.Back:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBackDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBackDown(xrEventData));
          break;
        case XRButton.Left:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalLeftDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalLeftDown(xrEventData));
          break;
        case XRButton.Right:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalRightDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalRightDown(xrEventData));
          break;
      }
    }

    private void ExecuteGlobalPress(XRButton id) {
      if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case XRButton.Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTriggerPress(xrEventData));
          break;
        case XRButton.Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalGripPress(xrEventData));
          break;
        case XRButton.A:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalAPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalAPress(xrEventData));
          break;
        case XRButton.B:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBPress(xrEventData));
          break;
        case XRButton.X:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalXPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalXPress(xrEventData));
          break;
        case XRButton.Y:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalYPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalYPress(xrEventData));
          break;
        case XRButton.Thumbstick:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalThumbstickPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalThumbstickPress(xrEventData));
          break;
        case XRButton.Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTouchpadPress(xrEventData));
          break;
        case XRButton.Menu:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalMenuPressHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalMenuPress(xrEventData));
          break;
        case XRButton.Forward:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalForwardHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalForward(xrEventData));
          break;
        case XRButton.Back:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBackHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBack(xrEventData));
          break;
        case XRButton.Left:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalLeftHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalLeft(xrEventData));
          break;
        case XRButton.Right:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalRightHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalRight(xrEventData));
          break;
      }
    }

    private void ExecuteGlobalPressUp(XRButton id) {
      if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case XRButton.Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTriggerPressUp(xrEventData));
          break;
        case XRButton.Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalGripPressUp(xrEventData));
          break;
        case XRButton.A:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalAPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalAPressUp(xrEventData));
          break;
        case XRButton.B:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBPressUp(xrEventData));
          break;
        case XRButton.X:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalXPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalXPressUp(xrEventData));
          break;
        case XRButton.Y:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalYPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalYPressUp(xrEventData));
          break;
        case XRButton.Thumbstick:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalThumbstickPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalThumbstickPressUp(xrEventData));
          break;
        case XRButton.Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTouchpadPressUp(xrEventData));
          break;
        case XRButton.Menu:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalMenuPressUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalMenuPressUp(xrEventData));
          break;
        case XRButton.Forward:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalForwardUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalForwardUp(xrEventData));
          break;
        case XRButton.Back:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBackUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBackUp(xrEventData));
          break;
        case XRButton.Left:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalLeftUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalLeftUp(xrEventData));
          break;
        case XRButton.Right:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalRightUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalRightUp(xrEventData));
          break;
      }
      //Remove paired list
      pressReceivers[id] = null;
    }

    private void ExecuteTouchDown(XRButton id) {
      GameObject go = xrEventData.currentRaycast;
      if (go == null)
        return;

      switch (id) {
        case XRButton.Touchpad:
          xrEventData.touchpadTouch = go;
          ExecuteEvents.Execute<IPointerTouchpadTouchDownHandler>(xrEventData.touchpadTouch, xrEventData,
            (x, y) => x.OnPointerTouchpadTouchDown(xrEventData));
          break;
        case XRButton.Trigger:
          xrEventData.triggerTouch = go;
          ExecuteEvents.Execute<IPointerTriggerTouchDownHandler>(xrEventData.triggerTouch, xrEventData,
            (x, y) => x.OnPointerTriggerTouchDown(xrEventData));
          break;
        case XRButton.Thumbstick:
          xrEventData.thumbstickTouch = go;
          ExecuteEvents.Execute<IPointerThumbstickTouchDownHandler>(xrEventData.thumbstickTouch, xrEventData,
            (x, y) => x.OnPointerThumbstickTouchDown(xrEventData));
          break;
        case XRButton.A:
          xrEventData.aTouch = go;
          ExecuteEvents.Execute<IPointerATouchDownHandler>(xrEventData.aTouch, xrEventData,
            (x, y) => x.OnPointerATouchDown(xrEventData));
          break;
        case XRButton.B:
          xrEventData.bTouch = go;
          ExecuteEvents.Execute<IPointerBTouchDownHandler>(xrEventData.bTouch, xrEventData,
            (x, y) => x.OnPointerBTouchDown(xrEventData));
          break;
        case XRButton.X:
          xrEventData.xTouch = go;
          ExecuteEvents.Execute<IPointerXTouchDownHandler>(xrEventData.xTouch, xrEventData,
            (x, y) => x.OnPointerXTouchDown(xrEventData));
          break;
        case XRButton.Y:
          xrEventData.yTouch = go;
          ExecuteEvents.Execute<IPointerYTouchDownHandler>(xrEventData.yTouch, xrEventData,
            (x, y) => x.OnPointerYTouchDown(xrEventData));
          break;
        case XRButton.Grip:
          xrEventData.gripTouch = go;
          ExecuteEvents.Execute<IPointerGripTouchDownHandler>(xrEventData.touchpadTouch, xrEventData,
            (x, y) => x.OnPointerGripTouchDown(xrEventData));
          break;
        case XRButton.Menu:
          xrEventData.menuTouch = go;
          ExecuteEvents.Execute<IPointerMenuTouchDownHandler>(xrEventData.touchpadTouch, xrEventData,
            (x, y) => x.OnPointerMenuTouchDown(xrEventData));
          break;
      }

      //Add pairing.
      touchPairings[id] = go;
    }

    private void ExecuteTouch(XRButton id) {
      if (touchPairings[id] == null)
        return;

      switch (id) {
        case XRButton.Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadTouchHandler>(xrEventData.touchpadTouch, xrEventData,
            (x, y) => x.OnPointerTouchpadTouch(xrEventData));
          break;
        case XRButton.Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchHandler>(xrEventData.triggerTouch, xrEventData,
            (x, y) => x.OnPointerTriggerTouch(xrEventData));
          break;
        case XRButton.Thumbstick:
          ExecuteEvents.Execute<IPointerThumbstickTouchHandler>(xrEventData.thumbstickTouch, xrEventData,
            (x, y) => x.OnPointerThumbstickTouch(xrEventData));
          break;
        case XRButton.A:
          ExecuteEvents.Execute<IPointerATouchHandler>(xrEventData.aTouch, xrEventData,
            (x, y) => x.OnPointerATouch(xrEventData));
          break;
        case XRButton.B:
          ExecuteEvents.Execute<IPointerBTouchHandler>(xrEventData.bTouch, xrEventData,
            (x, y) => x.OnPointerBTouch(xrEventData));
          break;
        case XRButton.X:
          ExecuteEvents.Execute<IPointerXTouchHandler>(xrEventData.xTouch, xrEventData,
            (x, y) => x.OnPointerXTouch(xrEventData));
          break;
        case XRButton.Y:
          ExecuteEvents.Execute<IPointerYTouchHandler>(xrEventData.yTouch, xrEventData,
            (x, y) => x.OnPointerYTouch(xrEventData));
          break;
        case XRButton.Grip:
          ExecuteEvents.Execute<IPointerGripTouchHandler>(xrEventData.yTouch, xrEventData,
            (x, y) => x.OnPointerGripTouch(xrEventData));
          break;
        case XRButton.Menu:
          ExecuteEvents.Execute<IPointerMenuTouchHandler>(xrEventData.yTouch, xrEventData,
            (x, y) => x.OnPointerMenuTouch(xrEventData));
          break;
      }
    }

    private void ExecuteTouchUp(XRButton id) {
      if (touchPairings[id] == null)
        return;

      switch (id) {
        case XRButton.Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadTouchUpHandler>(xrEventData.touchpadTouch, xrEventData,
            (x, y) => x.OnPointerTouchpadTouchUp(xrEventData));
          xrEventData.touchpadTouch = null;
          break;
        case XRButton.Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchUpHandler>(xrEventData.triggerTouch, xrEventData,
            (x, y) => x.OnPointerTriggerTouchUp(xrEventData));
          xrEventData.triggerTouch = null;
          break;
        case XRButton.Thumbstick:
          ExecuteEvents.Execute<IPointerThumbstickTouchUpHandler>(xrEventData.thumbstickTouch, xrEventData,
            (x, y) => x.OnPointerThumbstickTouchUp(xrEventData));
          xrEventData.thumbstickTouch = null;
          break;
        case XRButton.A:
          ExecuteEvents.Execute<IPointerATouchUpHandler>(xrEventData.aTouch, xrEventData,
            (x, y) => x.OnPointerATouchUp(xrEventData));
          xrEventData.aTouch = null;
          break;
        case XRButton.B:
          ExecuteEvents.Execute<IPointerBTouchUpHandler>(xrEventData.bTouch, xrEventData,
            (x, y) => x.OnPointerBTouchUp(xrEventData));
          xrEventData.bTouch = null;
          break;
        case XRButton.X:
          ExecuteEvents.Execute<IPointerXTouchUpHandler>(xrEventData.xTouch, xrEventData,
            (x, y) => x.OnPointerXTouchUp(xrEventData));
          xrEventData.xTouch = null;
          break;
        case XRButton.Y:
          ExecuteEvents.Execute<IPointerYTouchUpHandler>(xrEventData.yTouch, xrEventData,
            (x, y) => x.OnPointerYTouchUp(xrEventData));
          xrEventData.yTouch = null;
          break;
        case XRButton.Grip:
          ExecuteEvents.Execute<IPointerGripTouchUpHandler>(xrEventData.yTouch, xrEventData,
            (x, y) => x.OnPointerGripTouchUp(xrEventData));
          xrEventData.yTouch = null;
          break;
        case XRButton.Menu:
          ExecuteEvents.Execute<IPointerMenuTouchUpHandler>(xrEventData.yTouch, xrEventData,
            (x, y) => x.OnPointerMenuTouchUp(xrEventData));
          xrEventData.yTouch = null;
          break;
      }
    }

    public void ExecuteGlobalTouchDown(XRButton id) {
      //Add paired list.
      touchReceivers[id] = Receiver.instances;

      switch (id) {
        case XRButton.Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTriggerTouchDown(xrEventData));
          break;
        case XRButton.Touchpad:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTouchpadTouchDown(xrEventData));
          break;
        case XRButton.A:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalATouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalATouchDown(xrEventData));
          break;
        case XRButton.B:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBTouchDown(xrEventData));
          break;
        case XRButton.X:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalXTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalXTouchDown(xrEventData));
          break;
        case XRButton.Y:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalYTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalYTouchDown(xrEventData));
          break;
        case XRButton.Thumbstick:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalThumbstickTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalThumbstickTouchDown(xrEventData));
          break;
        case XRButton.Grip:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalGripTouchDown(xrEventData));
          break;
        case XRButton.Menu:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalMenuTouchDownHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalMenuTouchDown(xrEventData));
          break;
      }
    }

    public void ExecuteGlobalTouch(XRButton id) {
      if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case XRButton.Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTriggerTouch(xrEventData));
          break;
        case XRButton.Touchpad:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTouchpadTouch(xrEventData));
          break;
        case XRButton.A:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalATouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalATouch(xrEventData));
          break;
        case XRButton.B:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBTouch(xrEventData));
          break;
        case XRButton.X:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalXTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalXTouch(xrEventData));
          break;
        case XRButton.Y:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalYTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalYTouch(xrEventData));
          break;
        case XRButton.Thumbstick:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalThumbstickTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalThumbstickTouch(xrEventData));
          break;
        case XRButton.Grip:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalGripTouch(xrEventData));
          break;
        case XRButton.Menu:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalMenuTouchHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalMenuTouch(xrEventData));
          break;
      }
    }

    public void ExecuteGlobalTouchUp(XRButton id) {
      if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case XRButton.Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTriggerTouchUp(xrEventData));
          break;
        case XRButton.Touchpad:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalTouchpadTouchUp(xrEventData));
          break;
        case XRButton.A:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalATouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalATouchUp(xrEventData));
          break;
        case XRButton.B:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalBTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalBTouchUp(xrEventData));
          break;
        case XRButton.X:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalXTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalXTouchUp(xrEventData));
          break;
        case XRButton.Y:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalYTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalYTouchUp(xrEventData));
          break;
        case XRButton.Thumbstick:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalThumbstickTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalThumbstickTouchUp(xrEventData));
          break;
        case XRButton.Grip:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalGripTouchUp(xrEventData));
          break;
        case XRButton.Menu:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalMenuTouchUpHandler>(r.gameObject, xrEventData,
                (x, y) => x.OnGlobalMenuTouchUp(xrEventData));
          break;
      }
      //Remove paired list
      touchReceivers[id] = null;
    }
  }
}

