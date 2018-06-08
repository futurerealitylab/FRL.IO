using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace FRL.IO {

  public enum XRHand { Left, Right, None };
  public enum XRButton {
    Trigger, Grip, Touchpad, Menu, Thumbstick, A, B, X, Y, Forward, Back, Left, Right, Home
  }

  public class XREventData : PointerEventData {
    public XRHand hand;
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

    private static List<XRControllerModule> _modules = new List<XRControllerModule>();
    public static List<XRControllerModule> Modules {
      get { return _modules; }
    }

    public XRHand hand;

    private XRSystem _system;
    public XRSystem System {
      get { return _system; }
      set {
        _system = value;
        status = GetControllerStatus();
      }
    }

    public bool updateTracking = true;

    private float previousTriggerAxis, previousGripAxis;
    private Vector2 previousTouchpadAxis, previousThumbstickAxis;

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousVelocity, previousAcceleration;

    private Dictionary<XRButton, GameObject> pressPairings = new Dictionary<XRButton, GameObject>();
    private Dictionary<XRButton, List<Receiver>> pressReceivers = new Dictionary<XRButton, List<Receiver>>();
    private Dictionary<XRButton, GameObject> touchPairings = new Dictionary<XRButton, GameObject>();
    private Dictionary<XRButton, List<Receiver>> touchReceivers = new Dictionary<XRButton, List<Receiver>>();

    protected override PointerEventData pointerEventData {
      get {
        return xrEventData;
      }
    }

    private XRControllerStatus status;
    public XREventData xrEventData { get; private set; }

    private bool isTracked = true;
    public bool IsTracked {
      get { return isTracked; }
    }

    public static XRButton[] XRButtons {
      get { return (XRButton[])Enum.GetValues(typeof(XRButton)); }
    }

    protected override void Awake() {
      base.Awake();
      xrEventData = new XREventData(this);
      status = GetControllerStatus();
    }

    protected virtual void OnEnable() {
      _modules.Add(this);
      foreach (XRButton button in XRButtons) {
        pressPairings.Add(button, null);
        pressReceivers.Add(button, null);
        touchPairings.Add(button, null);
        touchReceivers.Add(button, null);
      }
    }

    protected override void OnDisable() {
      base.OnDisable();
      _modules.Remove(this);
      foreach (XRButton button in XRButtons) {
        ExecutePressUp(button);
        ExecuteGlobalPressUp(button);
        ExecuteTouchUp(button);
        ExecuteGlobalTouchUp(button);
      }
      xrEventData.Reset();
    }

    private XRControllerStatus GetControllerStatus() {
      switch (this.System) {
        case XRSystem.ViveFocus:
          return new FocusControllerStatus(this.hand);
        case XRSystem.Vive:
          return new ViveControllerStatus(this.hand);
        case XRSystem.CV1:
          return new OculusTouchControllerStatus(this.hand);
        case XRSystem.Daydream:
          return new DaydreamControllerStatus(this.hand);
        case XRSystem.WindowsMR:
          return new WMRControllerStatus(this.hand);
        case XRSystem.GearVR:
          return new GVRControllerStatus(this.hand);
        case XRSystem.Standalone:
          return new StandaloneControllerStatus();
        default:
          return new BrokenControllerStatus(this.hand, "Unknown XRSystem");
      }
    }

    protected override void Process() {
      status.Generate();

      this.isTracked = status.IsTracked;
      if (this.IsTracked && updateTracking) {
        this.transform.localPosition = status.Position;
        this.transform.localRotation = status.Rotation;
      }

      this.xrEventData.hand = this.hand;
      this.xrEventData.velocity = status.Velocity;
      this.xrEventData.acceleration = status.Acceleration;
      this.xrEventData.triggerAxis = status.TriggerAxis;
      this.xrEventData.gripAxis = status.GripAxis;
      this.xrEventData.thumbstickAxis = status.ThumbstickAxis;
      this.xrEventData.touchpadAxis = status.TouchpadAxis;

      base.Process();
      this.HandleButtons();
    }

    void HandleButtons() {
      if (status.GetClick(XRButton.Trigger)) ExecuteTriggerClick();
      if (status.GetClick(XRButton.Grip)) ExecuteGripClick();

      foreach (XRButton button in XRButtons) {
        if (status.GetPressDown(button)) {
          ExecutePressDown(button);
          ExecuteGlobalPressDown(button);
        }
        if (status.GetPress(button)) {
          ExecutePress(button);
          ExecuteGlobalPress(button);
        }
        if (status.GetPressUp(button)) {
          ExecutePressUp(button);
          ExecuteGlobalPressUp(button);
        }
        if (status.GetTouchDown(button)) {
          ExecuteTouchDown(button);
          ExecuteGlobalTouchDown(button);
        }
        if (status.GetTouch(button)) {
          ExecuteTouch(button);
          ExecuteGlobalTouch(button);
        }
        if (status.GetTouchUp(button)) {
          ExecuteTouchUp(button);
          ExecuteGlobalTouchUp(button);
        }
      }
    }

    public Vector2 GetThumbstickAxis() {
      return status.ThumbstickAxis;
    }

    public Vector2 GetTouchpadAxis() {
      return status.TouchpadAxis;
    }

    public float GetTriggerAxis() {
      return status.TriggerAxis;
    }

    public float GetGripAxis() {
      return status.GripAxis;
    }

    public void HapticPulse(float strength, float time) {
      Mathf.Clamp01(strength);
      Keyframe start = new Keyframe(0f, strength);
      Keyframe end = new Keyframe(1f, strength);
      AnimationCurve curve = new AnimationCurve(new Keyframe[2] { start, end });
      HapticPulse(curve, time);
    }

    public void HapticPulse(AnimationCurve curve, float time) {
      status.HapticPulse(curve, time);
    }

    public void HapticPulse(byte[] samples) {
      status.HapticPulse(samples);
    }

    protected bool GetClick(XRButton button) {
      return status.GetClick(button);
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
      foreach (Receiver r in pressReceivers[XRButton.Grip]) {
        if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
          ExecuteEvents.Execute<IGlobalGripClickHandler>(r.gameObject, xrEventData,
            (x, y) => x.OnGlobalGripClick(xrEventData));
      }
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

