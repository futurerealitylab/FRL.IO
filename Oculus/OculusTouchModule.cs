using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.IO;
using System;

public class OculusTouchModule : PointerInputModule {

  public enum Hand { LEFT, RIGHT }

  public Hand hand = Hand.LEFT;

  private Dictionary<OVRInput.RawButton, GameObject> pressPairings = new Dictionary<OVRInput.RawButton, GameObject>();
  private Dictionary<OVRInput.RawButton, List<Receiver>> pressReceivers = new Dictionary<OVRInput.RawButton, List<Receiver>>();
  private Dictionary<OVRInput.RawTouch, GameObject> touchPairings = new Dictionary<OVRInput.RawTouch, GameObject>();
  private Dictionary<OVRInput.RawTouch, List<Receiver>> touchReceivers = new Dictionary<OVRInput.RawTouch, List<Receiver>>();

  private EventData _eventData;
  protected override VREventData eventData {
    get {
      return _eventData;
    }
  }

  private OVRInput.RawButton[] leftPressIds = new OVRInput.RawButton[] {
    OVRInput.RawButton.X, OVRInput.RawButton.Y, OVRInput.RawButton.LIndexTrigger,
    OVRInput.RawButton.LHandTrigger, OVRInput.RawButton.LThumbstick
  };

  private OVRInput.RawButton[] rightPressIds = new OVRInput.RawButton[] {
    OVRInput.RawButton.A, OVRInput.RawButton.B, OVRInput.RawButton.RIndexTrigger,
    OVRInput.RawButton.RHandTrigger, OVRInput.RawButton.RThumbstick
  };

  private OVRInput.RawTouch[] leftTouchIds = new OVRInput.RawTouch[] {
    OVRInput.RawTouch.X, OVRInput.RawTouch.Y, OVRInput.RawTouch.LIndexTrigger,
    OVRInput.RawTouch.LThumbstick, OVRInput.RawTouch.LTouchpad
  };

  private OVRInput.RawTouch[] rightTouchIds = new OVRInput.RawTouch[] {
    OVRInput.RawTouch.A, OVRInput.RawTouch.B, OVRInput.RawTouch.RIndexTrigger,
    OVRInput.RawTouch.RThumbstick, OVRInput.RawTouch.RTouchpad
  };

  protected override void Awake() {
    base.Awake();
    _eventData = new EventData(this);

    foreach (OVRInput.RawButton button in leftPressIds) {
      pressPairings.Add(button, null);
      pressReceivers.Add(button, null);
    }
    foreach (OVRInput.RawButton button in rightPressIds) {
      pressPairings.Add(button, null);
      pressReceivers.Add(button, null);
    }
    foreach (OVRInput.RawTouch touch in leftTouchIds) {
      touchPairings.Add(touch, null);
      touchReceivers.Add(touch, null);
    }
    foreach (OVRInput.RawTouch touch in rightTouchIds) {
      touchPairings.Add(touch, null);
      touchReceivers.Add(touch, null);
    }
  }

  protected override void OnDisable() {
    base.OnDisable();

    foreach (OVRInput.RawButton button in leftPressIds) {
      this.ExecutePressUp(button);
      this.ExecuteGlobalPressUp(button);
    }
    foreach (OVRInput.RawButton button in rightPressIds) {
      this.ExecutePressUp(button);
      this.ExecuteGlobalPressUp(button);
    }
    foreach (OVRInput.RawTouch touch in leftTouchIds) {
      this.ExecuteTouchUp(touch);
      this.ExecuteGlobalTouchUp(touch);
    }
    foreach (OVRInput.RawTouch touch in rightTouchIds) {
      this.ExecuteTouchUp(touch);
      this.ExecuteGlobalTouchUp(touch);
    }

    _eventData.Reset();
  }


  void Update() {
    if (!hasBeenProcessed) {
      Process();
    }
  }

  void LateUpdate() {
    hasBeenProcessed = false;
  }

  protected override void Process() {
    base.Process();
    this.HandleButtons();
  }

  public OculusTouchModule.EventData GetEventData() {
    Update();
    return _eventData;
  }

  public Vector2 GetThumbstickAxis() {
    try {
      switch (hand) {
        case Hand.LEFT:
          return OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        case Hand.RIGHT:
          return OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
        default:
          return Vector2.zero;
      }
    } catch (Exception e) {
      return Vector2.zero;
    }
  }

  public float GetTriggerAxis() {
    switch (hand) {
      case Hand.LEFT:
        return OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
      case Hand.RIGHT:
        return OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
      default:
        return 0;
    }
  }

  public float GetGripAxis() {
    switch (hand) {
      case Hand.LEFT:
        return OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger);
      case Hand.RIGHT:
        return OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);
      default:
        return 0;
    }
  }

  void HandleButtons() {
    float previousPrimary = _eventData.triggerAxis;
    float previousSecondary = _eventData.gripAxis;

    _eventData.triggerAxis = GetTriggerAxis();
    _eventData.gripAxis = GetGripAxis();
    _eventData.thumbstickAxis = GetThumbstickAxis();

    if (previousPrimary != 1.0f && _eventData.triggerAxis == 1f) {
      ExecuteTriggerClick();
    }
    if (previousSecondary != 1.0f && _eventData.gripAxis == 1f) {
      ExecuteGripClick();
    }

    OVRInput.RawButton[] pressIds = hand == Hand.LEFT ? leftPressIds : rightPressIds;
    foreach (OVRInput.RawButton button in pressIds) {
      if (GetPressDown(button)) {
        ExecutePressDown(button);
        ExecuteGlobalPressDown(button);
      } else if (GetPress(button)) {
        ExecutePress(button);
        ExecuteGlobalPress(button);
      } else if (GetPressUp(button)) {
        ExecutePressUp(button);
        ExecuteGlobalPressUp(button);
      }
    }

    OVRInput.RawTouch[] touchIds = hand == Hand.LEFT ? leftTouchIds : rightTouchIds;
    foreach (OVRInput.RawTouch touch in touchIds) {
      if (GetTouchDown(touch)) {
        ExecuteTouchDown(touch);
        ExecuteGlobalTouchDown(touch);
      } else if (GetTouch(touch)) {
        ExecuteTouch(touch);
        ExecuteGlobalTouch(touch);
      } else if (GetTouchUp(touch)) {
        ExecuteTouchUp(touch);
        ExecuteGlobalTouchUp(touch);
      }
    }
  }

  public void ExecutePressDown(OVRInput.RawButton id) {
    GameObject go = _eventData.currentRaycast;
    if (go == null)
      return;

    //If there's a receiver component, only cast to it if it's this module.
    Receiver r = go.GetComponent<Receiver>();
    if (r != null && r.module != null && r.module != this)
      return;

    switch (id) {
      case OVRInput.RawButton.LIndexTrigger:
      case OVRInput.RawButton.RIndexTrigger:
        _eventData.triggerPress = go;
        ExecuteEvents.Execute<IPointerTriggerPressDownHandler>(_eventData.triggerPress, _eventData,
          (x, y) => x.OnPointerTriggerPressDown(_eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        _eventData.gripPress = go;
        ExecuteEvents.Execute<IPointerGripPressDownHandler>(_eventData.gripPress, _eventData,
          (x, y) => x.OnPointerGripPressDown(_eventData));
        break;
      case OVRInput.RawButton.A:
        _eventData.aPress = go;
        ExecuteEvents.Execute<IPointerAPressDownHandler>(_eventData.aPress, _eventData,
          (x, y) => x.OnPointerAPressDown(_eventData));
        break;
      case OVRInput.RawButton.B:
        _eventData.bPress = go;
        ExecuteEvents.Execute<IPointerBPressDownHandler>(_eventData.bPress, _eventData,
          (x, y) => x.OnPointerBPressDown(_eventData));
        break;
      case OVRInput.RawButton.X:
        _eventData.xPress = go;
        ExecuteEvents.Execute<IPointerXPressDownHandler>(_eventData.xPress, _eventData,
          (x, y) => x.OnPointerXPressDown(_eventData));
        break;
      case OVRInput.RawButton.Y:
        _eventData.yPress = go;
        ExecuteEvents.Execute<IPointerYPressDownHandler>(_eventData.yPress, _eventData,
          (x, y) => x.OnPointerYPressDown(_eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        _eventData.thumbstickPress = go;
        ExecuteEvents.Execute<IPointerThumbstickPressDownHandler>(_eventData.thumbstickPress, _eventData,
          (x, y) => x.OnPointerThumbstickPressDown(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
    //Add pairing.
    pressPairings[id] = go;
  }

  public void ExecutePress(OVRInput.RawButton id) {
    if (pressPairings[id] == null)
      return;

    switch (id) {
      case OVRInput.RawButton.LIndexTrigger:
      case OVRInput.RawButton.RIndexTrigger:
        ExecuteEvents.Execute<IPointerTriggerPressHandler>(_eventData.triggerPress, _eventData,
          (x, y) => x.OnPointerTriggerPress(_eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        ExecuteEvents.Execute<IPointerGripPressHandler>(_eventData.gripPress, _eventData,
          (x, y) => x.OnPointerGripPress(_eventData));
        break;
      case OVRInput.RawButton.A:
        ExecuteEvents.Execute<IPointerAPressHandler>(_eventData.aPress, _eventData,
          (x, y) => x.OnPointerAPress(_eventData));
        break;
      case OVRInput.RawButton.B:
        ExecuteEvents.Execute<IPointerBPressHandler>(_eventData.bPress, _eventData,
          (x, y) => x.OnPointerBPress(_eventData));
        break;
      case OVRInput.RawButton.X:
        ExecuteEvents.Execute<IPointerXPressHandler>(_eventData.xPress, _eventData,
          (x, y) => x.OnPointerXPress(_eventData));
        break;
      case OVRInput.RawButton.Y:
        ExecuteEvents.Execute<IPointerYPressHandler>(_eventData.yPress, _eventData,
          (x, y) => x.OnPointerYPress(_eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        ExecuteEvents.Execute<IPointerThumbstickPressHandler>(_eventData.thumbstickPress, _eventData,
          (x, y) => x.OnPointerThumbstickPress(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
  }

  public void ExecutePressUp(OVRInput.RawButton id) {
    if (pressPairings[id] == null)
      return;

    switch (id) {
      case OVRInput.RawButton.LIndexTrigger:
      case OVRInput.RawButton.RIndexTrigger:
        ExecuteEvents.Execute<IPointerTriggerPressUpHandler>(_eventData.triggerPress, _eventData,
          (x, y) => x.OnPointerTriggerPressUp(_eventData));
        _eventData.triggerPress = null;
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        ExecuteEvents.Execute<IPointerGripPressUpHandler>(_eventData.gripPress, _eventData,
          (x, y) => x.OnPointerGripPressUp(_eventData));
        _eventData.gripPress = null;
        break;
      case OVRInput.RawButton.A:
        ExecuteEvents.Execute<IPointerAPressUpHandler>(_eventData.aPress, _eventData,
          (x, y) => x.OnPointerAPressUp(_eventData));
        _eventData.aPress = null;
        break;
      case OVRInput.RawButton.B:
        ExecuteEvents.Execute<IPointerBPressUpHandler>(_eventData.bPress, _eventData,
          (x, y) => x.OnPointerBPressUp(_eventData));
        _eventData.bPress = null;
        break;
      case OVRInput.RawButton.X:
        ExecuteEvents.Execute<IPointerXPressUpHandler>(_eventData.xPress, _eventData,
          (x, y) => x.OnPointerXPressUp(_eventData));
        _eventData.xPress = null;
        break;
      case OVRInput.RawButton.Y:
        ExecuteEvents.Execute<IPointerYPressUpHandler>(_eventData.yPress, _eventData,
          (x, y) => x.OnPointerYPressUp(_eventData));
        _eventData.yPress = null;
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        ExecuteEvents.Execute<IPointerThumbstickPressUpHandler>(_eventData.thumbstickPress, _eventData,
          (x, y) => x.OnPointerThumbstickPressUp(_eventData));
        _eventData.thumbstickPress = null;
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
    //Remove pairing.
    pressPairings[id] = null;
  }

  public void ExecuteGlobalPressDown(OVRInput.RawButton id) {
    //Add paired list.
    pressReceivers[id] = Receiver.instances;

    switch (id) {
      case OVRInput.RawButton.LIndexTrigger:
      case OVRInput.RawButton.RIndexTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTriggerPressDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTriggerPressDown(_eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this))) {
            ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalGripPressDown(_eventData));
            ExecuteEvents.Execute<IGlobalGripClickHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalGripClick(_eventData));
          }
        break;
      case OVRInput.RawButton.A:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalAPressDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalAPressDown(_eventData));
        break;
      case OVRInput.RawButton.B:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBPressDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalBPressDown(_eventData));
        break;
      case OVRInput.RawButton.X:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXPressDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalXPressDown(_eventData));
        break;
      case OVRInput.RawButton.Y:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYPressDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalYPressDown(_eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickPressDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalThumbstickPressDown(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
  }

  public void ExecuteGlobalPress(OVRInput.RawButton id) {
    if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
      return;
    }

    switch (id) {
      case OVRInput.RawButton.LIndexTrigger:
      case OVRInput.RawButton.RIndexTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTriggerPressHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTriggerPress(_eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalGripPressHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalGripPress(_eventData));
        break;
      case OVRInput.RawButton.A:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalAPressHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalAPress(_eventData));
        break;
      case OVRInput.RawButton.B:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBPressHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalBPress(_eventData));
        break;
      case OVRInput.RawButton.X:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXPressHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalXPress(_eventData));
        break;
      case OVRInput.RawButton.Y:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYPressHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalYPress(_eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickPressHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalThumbstickPress(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
  }

  public void ExecuteGlobalPressUp(OVRInput.RawButton id) {
    if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
      return;
    }

    switch (id) {
      case OVRInput.RawButton.LIndexTrigger:
      case OVRInput.RawButton.RIndexTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTriggerPressUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTriggerPressUp(_eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalGripPressUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalGripPressUp(_eventData));
        break;
      case OVRInput.RawButton.A:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalAPressUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalAPressUp(_eventData));
        break;
      case OVRInput.RawButton.B:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBPressUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalBPressUp(_eventData));
        break;
      case OVRInput.RawButton.X:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXPressUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalXPressUp(_eventData));
        break;
      case OVRInput.RawButton.Y:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYPressUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalYPressUp(_eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickPressUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalThumbstickPressUp(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
    //Remove paired list
    pressReceivers[id] = null;
  }

  private void ExecuteTouchDown(OVRInput.RawTouch id) {
    GameObject go = _eventData.currentRaycast;
    if (go == null)
      return;

    switch (id) {
      case OVRInput.RawTouch.LTouchpad:
      case OVRInput.RawTouch.RTouchpad:
        _eventData.touchpadTouch = go;
        ExecuteEvents.Execute<IPointerTouchpadTouchDownHandler>(_eventData.touchpadTouch, _eventData,
          (x, y) => x.OnPointerTouchpadTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.LIndexTrigger:
      case OVRInput.RawTouch.RIndexTrigger:
        _eventData.triggerTouch = go;
        ExecuteEvents.Execute<IPointerTriggerTouchDownHandler>(_eventData.triggerTouch, _eventData,
          (x, y) => x.OnPointerTriggerTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.LThumbstick:
      case OVRInput.RawTouch.RThumbstick:
        _eventData.thumbstickTouch = go;
        ExecuteEvents.Execute<IPointerThumbstickTouchDownHandler>(_eventData.thumbstickTouch, _eventData,
          (x, y) => x.OnPointerThumbstickTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.A:
        _eventData.aTouch = go;
        ExecuteEvents.Execute<IPointerATouchDownHandler>(_eventData.aTouch, _eventData,
          (x, y) => x.OnPointerATouchDown(_eventData));
        break;
      case OVRInput.RawTouch.B:
        _eventData.bTouch = go;
        ExecuteEvents.Execute<IPointerBTouchDownHandler>(_eventData.bTouch, _eventData,
          (x, y) => x.OnPointerBTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.X:
        _eventData.xTouch = go;
        ExecuteEvents.Execute<IPointerXTouchDownHandler>(_eventData.xTouch, _eventData,
          (x, y) => x.OnPointerXTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.Y:
        _eventData.yTouch = go;
        ExecuteEvents.Execute<IPointerYTouchDownHandler>(_eventData.yTouch, _eventData,
          (x, y) => x.OnPointerYTouchDown(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawTouch.");
    }

    //Add pairing.
    touchPairings[id] = go;
  }

  private void ExecuteTouch(OVRInput.RawTouch id) {
    if (touchPairings[id] == null)
      return;

    switch (id) {
      case OVRInput.RawTouch.LTouchpad:
      case OVRInput.RawTouch.RTouchpad:
        ExecuteEvents.Execute<IPointerTouchpadTouchHandler>(_eventData.touchpadTouch, _eventData,
          (x, y) => x.OnPointerTouchpadTouch(_eventData));
        break;
      case OVRInput.RawTouch.LIndexTrigger:
      case OVRInput.RawTouch.RIndexTrigger:
        ExecuteEvents.Execute<IPointerTriggerTouchHandler>(_eventData.triggerTouch, _eventData,
          (x, y) => x.OnPointerTriggerTouch(_eventData));
        break;
      case OVRInput.RawTouch.LThumbstick:
      case OVRInput.RawTouch.RThumbstick:
        ExecuteEvents.Execute<IPointerThumbstickTouchHandler>(_eventData.thumbstickTouch, _eventData,
          (x, y) => x.OnPointerThumbstickTouch(_eventData));
        break;
      case OVRInput.RawTouch.A:
        ExecuteEvents.Execute<IPointerATouchHandler>(_eventData.aTouch, _eventData,
          (x, y) => x.OnPointerATouch(_eventData));
        break;
      case OVRInput.RawTouch.B:
        ExecuteEvents.Execute<IPointerBTouchHandler>(_eventData.bTouch, _eventData,
          (x, y) => x.OnPointerBTouch(_eventData));
        break;
      case OVRInput.RawTouch.X:
        ExecuteEvents.Execute<IPointerXTouchHandler>(_eventData.xTouch, _eventData,
          (x, y) => x.OnPointerXTouch(_eventData));
        break;
      case OVRInput.RawTouch.Y:
        ExecuteEvents.Execute<IPointerYTouchHandler>(_eventData.yTouch, _eventData,
          (x, y) => x.OnPointerYTouch(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawTouch.");
    }
  }

  private void ExecuteTouchUp(OVRInput.RawTouch id) {
    if (touchPairings[id] == null)
      return;

    switch (id) {
      case OVRInput.RawTouch.LTouchpad:
      case OVRInput.RawTouch.RTouchpad:
        ExecuteEvents.Execute<IPointerTouchpadTouchUpHandler>(_eventData.touchpadTouch, _eventData,
          (x, y) => x.OnPointerTouchpadTouchUp(_eventData));
        _eventData.touchpadTouch = null;
        break;
      case OVRInput.RawTouch.LIndexTrigger:
      case OVRInput.RawTouch.RIndexTrigger:
        ExecuteEvents.Execute<IPointerTriggerTouchUpHandler>(_eventData.triggerTouch, _eventData,
          (x, y) => x.OnPointerTriggerTouchUp(_eventData));
        _eventData.triggerTouch = null;
        break;
      case OVRInput.RawTouch.LThumbstick:
      case OVRInput.RawTouch.RThumbstick:
        ExecuteEvents.Execute<IPointerThumbstickTouchUpHandler>(_eventData.thumbstickTouch, _eventData,
          (x, y) => x.OnPointerThumbstickTouchUp(_eventData));
        _eventData.thumbstickTouch = null;
        break;
      case OVRInput.RawTouch.A:
        ExecuteEvents.Execute<IPointerATouchUpHandler>(_eventData.aTouch, _eventData,
          (x, y) => x.OnPointerATouchUp(_eventData));
        _eventData.aTouch = null;
        break;
      case OVRInput.RawTouch.B:
        ExecuteEvents.Execute<IPointerBTouchUpHandler>(_eventData.bTouch, _eventData,
          (x, y) => x.OnPointerBTouchUp(_eventData));
        _eventData.bTouch = null;
        break;
      case OVRInput.RawTouch.X:
        ExecuteEvents.Execute<IPointerXTouchUpHandler>(_eventData.xTouch, _eventData,
          (x, y) => x.OnPointerXTouchUp(_eventData));
        _eventData.xTouch = null;
        break;
      case OVRInput.RawTouch.Y:
        ExecuteEvents.Execute<IPointerYTouchUpHandler>(_eventData.yTouch, _eventData,
          (x, y) => x.OnPointerYTouchUp(_eventData));
        _eventData.yTouch = null;
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawTouch.");
    }
  }

  public void ExecuteGlobalTouchDown(OVRInput.RawTouch id) {
    //Add paired list.
    touchReceivers[id] = Receiver.instances;

    switch (id) {
      case OVRInput.RawTouch.LIndexTrigger:
      case OVRInput.RawTouch.RIndexTrigger:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTriggerTouchDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTriggerTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.LTouchpad:
      case OVRInput.RawTouch.RTouchpad:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this))) {
            ExecuteEvents.Execute<IGlobalTouchpadTouchDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTouchpadTouchDown(_eventData));
          }
        break;
      case OVRInput.RawTouch.A:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalATouchDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalATouchDown(_eventData));
        break;
      case OVRInput.RawTouch.B:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBTouchDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalBTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.X:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXTouchDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalXTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.Y:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYTouchDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalYTouchDown(_eventData));
        break;
      case OVRInput.RawTouch.LThumbstick:
      case OVRInput.RawTouch.RThumbstick:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickTouchDownHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalThumbstickTouchDown(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
  }

  public void ExecuteGlobalTouch(OVRInput.RawTouch id) {
    if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
      return;
    }

    switch (id) {
      case OVRInput.RawTouch.LIndexTrigger:
      case OVRInput.RawTouch.RIndexTrigger:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTriggerTouchHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTriggerTouch(_eventData));
        break;
      case OVRInput.RawTouch.LTouchpad:
      case OVRInput.RawTouch.RTouchpad:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTouchpadTouchHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTouchpadTouch(_eventData));
        break;
      case OVRInput.RawTouch.A:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalATouchHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalATouch(_eventData));
        break;
      case OVRInput.RawTouch.B:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBTouchHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalBTouch(_eventData));
        break;
      case OVRInput.RawTouch.X:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXTouchHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalXTouch(_eventData));
        break;
      case OVRInput.RawTouch.Y:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYTouchHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalYTouch(_eventData));
        break;
      case OVRInput.RawTouch.LThumbstick:
      case OVRInput.RawTouch.RThumbstick:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickTouchHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalThumbstickTouch(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
  }

  public void ExecuteGlobalTouchUp(OVRInput.RawTouch id) {
    if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
      return;
    }

    switch (id) {
      case OVRInput.RawTouch.LIndexTrigger:
      case OVRInput.RawTouch.RIndexTrigger:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTriggerTouchUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTriggerTouchUp(_eventData));
        break;
      case OVRInput.RawTouch.LTouchpad:
      case OVRInput.RawTouch.RTouchpad:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalTouchpadTouchUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalTouchpadTouchUp(_eventData));
        break;
      case OVRInput.RawTouch.A:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalATouchUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalATouchUp(_eventData));
        break;
      case OVRInput.RawTouch.B:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBTouchUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalBTouchUp(_eventData));
        break;
      case OVRInput.RawTouch.X:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXTouchUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalXTouchUp(_eventData));
        break;
      case OVRInput.RawTouch.Y:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYTouchUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalYTouchUp(_eventData));
        break;
      case OVRInput.RawTouch.LThumbstick:
      case OVRInput.RawTouch.RThumbstick:
        foreach (Receiver r in touchReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickTouchUpHandler>(r.gameObject, _eventData,
              (x, y) => x.OnGlobalThumbstickTouchUp(_eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
    //Remove paired list
    touchReceivers[id] = null;
  }

  private void ExecuteTriggerClick() {
    if (_eventData.currentRaycast != null) {
      ExecuteEvents.Execute<IPointerTriggerClickHandler>(_eventData.currentRaycast, _eventData, (x, y) => {
        x.OnPointerTriggerClick(_eventData);
      });
    }
  }

  private void ExecuteGripClick() {
    if (_eventData.currentRaycast != null) {
      ExecuteEvents.Execute<IPointerGripClickHandler>(_eventData.currentRaycast, _eventData, (x, y) => {
        x.OnPointerGripClick(_eventData);
      });
    }
  }

  private bool GetPressDown(OVRInput.RawButton button) {
    return OVRInput.GetDown(button);
  }

  private bool GetPress(OVRInput.RawButton button) {
    return OVRInput.Get(button);
  }

  private bool GetPressUp(OVRInput.RawButton button) {
    return OVRInput.GetUp(button);
  }

  private bool GetTouchDown(OVRInput.RawTouch touch) {
    return OVRInput.GetDown(touch);
  }

  private bool GetTouch(OVRInput.RawTouch touch) {
    return OVRInput.Get(touch);
  }

  private bool GetTouchUp(OVRInput.RawTouch touch) {
    return OVRInput.GetUp(touch);
  }

  public class EventData : VREventData {

    /// <summary>
    /// The OculusTouchModule that manages the instance of EventData.
    /// </summary>
    public OculusTouchModule oculusTouchModule;

    public Vector2 thumbstickAxis;

    public float gripAxis;

    public GameObject aPress;
    public GameObject bPress;
    public GameObject xPress;
    public GameObject yPress;
    public GameObject thumbstickPress;

    public GameObject aTouch;
    public GameObject bTouch;
    public GameObject xTouch;
    public GameObject yTouch;
    public GameObject gripTouch;
    public GameObject thumbstickTouch;

    internal EventData(OculusTouchModule module) : base(module) {
      this.oculusTouchModule = module;
    }

    /// <summary>
    /// Reset the event data fields.
    /// </summary>
    internal override void Reset() {
      base.Reset();

      this.gripAxis = 0;
      this.thumbstickAxis = Vector2.zero;
      
      this.aPress = null;
      this.aTouch = null;
      this.bPress = null;
      this.bTouch = null;
      this.gripTouch = null;
      this.thumbstickPress = null;
      this.thumbstickTouch = null;
      this.xPress = null;
      this.xTouch = null;
      this.yPress = null;
      this.yTouch = null;
    }
  }
}
