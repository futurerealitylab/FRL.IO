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
  private Dictionary<OVRInput.RawButton, GameObject> touchPairings = new Dictionary<OVRInput.RawButton, GameObject>();
  private Dictionary<OVRInput.RawButton, List<Receiver>> touchReceivers = new Dictionary<OVRInput.RawButton, List<Receiver>>();

  private new EventData eventData;

  private OVRInput.RawButton[] leftPressIds = new OVRInput.RawButton[] {
    OVRInput.RawButton.X, OVRInput.RawButton.Y, OVRInput.RawButton.LIndexTrigger,
    OVRInput.RawButton.LHandTrigger, OVRInput.RawButton.LThumbstick
  };

  private OVRInput.RawButton[] rightPressIds = new OVRInput.RawButton[] {
    OVRInput.RawButton.A, OVRInput.RawButton.B, OVRInput.RawButton.RIndexTrigger,
    OVRInput.RawButton.RHandTrigger, OVRInput.RawButton.RThumbstick
  };


  protected override void Awake() {
    eventData = new EventData(this);

    foreach (OVRInput.RawButton button in leftPressIds) {
      pressPairings.Add(button, null);
      pressReceivers.Add(button, null);
    }
    foreach (OVRInput.RawButton button in rightPressIds) {
      pressPairings.Add(button, null);
      pressReceivers.Add(button, null);
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

    eventData.Reset();
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
    return eventData;
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
    float previousPrimary = eventData.triggerAxis;
    float previousSecondary = eventData.gripAxis;

    eventData.triggerAxis = GetTriggerAxis();
    eventData.gripAxis = GetGripAxis();
    eventData.thumbstickAxis = GetThumbstickAxis();

    if (previousPrimary != 1.0f && eventData.triggerAxis == 1f) {
      ExecuteTriggerClick();
    }
    if (previousSecondary != 1.0f && eventData.gripAxis == 1f) {
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

  }

  public void ExecutePressDown(OVRInput.RawButton id) {
    GameObject go = eventData.currentRaycast;
    if (go == null)
      return;

    //If there's a receiver component, only cast to it if it's this module.
    Receiver r = go.GetComponent<Receiver>();
    if (r != null && r.module != null && r.module != this)
      return;

    switch (id) {
      case OVRInput.RawButton.LIndexTrigger:
      case OVRInput.RawButton.RIndexTrigger:
        eventData.triggerPress = go;
        ExecuteEvents.Execute<IPointerTriggerPressDownHandler>(eventData.triggerPress, eventData,
          (x, y) => x.OnPointerTriggerPressDown(eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        eventData.gripPress = go;
        ExecuteEvents.Execute<IPointerGripPressDownHandler>(eventData.gripPress, eventData,
          (x, y) => x.OnPointerGripPressDown(eventData));
        break;
      case OVRInput.RawButton.A:
        eventData.aPress = go;
        ExecuteEvents.Execute<IPointerAPressDownHandler>(eventData.aPress, eventData,
          (x, y) => x.OnPointerAPressDown(eventData));
        break;
      case OVRInput.RawButton.B:
        eventData.bPress = go;
        ExecuteEvents.Execute<IPointerBPressDownHandler>(eventData.bPress, eventData,
          (x, y) => x.OnPointerBPressDown(eventData));
        break;
      case OVRInput.RawButton.X:
        eventData.xPress = go;
        ExecuteEvents.Execute<IPointerXPressDownHandler>(eventData.xPress, eventData,
          (x, y) => x.OnPointerXPressDown(eventData));
        break;
      case OVRInput.RawButton.Y:
        eventData.yPress = go;
        ExecuteEvents.Execute<IPointerYPressDownHandler>(eventData.yPress, eventData,
          (x, y) => x.OnPointerYPressDown(eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        eventData.thumbstickPress = go;
        ExecuteEvents.Execute<IPointerThumbstickPressDownHandler>(eventData.yPress, eventData,
          (x, y) => x.OnPointerThumbstickPressDown(eventData));
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
        ExecuteEvents.Execute<IPointerTriggerPressHandler>(eventData.triggerPress, eventData,
          (x, y) => x.OnPointerTriggerPress(eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        ExecuteEvents.Execute<IPointerGripPressHandler>(eventData.gripPress, eventData,
          (x, y) => x.OnPointerGripPress(eventData));
        break;
      case OVRInput.RawButton.A:
        ExecuteEvents.Execute<IPointerAPressHandler>(eventData.aPress, eventData,
          (x, y) => x.OnPointerAPress(eventData));
        break;
      case OVRInput.RawButton.B:
        ExecuteEvents.Execute<IPointerBPressHandler>(eventData.bPress, eventData,
          (x, y) => x.OnPointerBPress(eventData));
        break;
      case OVRInput.RawButton.X:
        ExecuteEvents.Execute<IPointerXPressHandler>(eventData.xPress, eventData,
          (x, y) => x.OnPointerXPress(eventData));
        break;
      case OVRInput.RawButton.Y:
        ExecuteEvents.Execute<IPointerYPressHandler>(eventData.yPress, eventData,
          (x, y) => x.OnPointerYPress(eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        ExecuteEvents.Execute<IPointerThumbstickPressHandler>(eventData.yPress, eventData,
          (x, y) => x.OnPointerThumbstickPress(eventData));
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
        ExecuteEvents.Execute<IPointerTriggerPressUpHandler>(eventData.triggerPress, eventData,
          (x, y) => x.OnPointerTriggerPressUp(eventData));
        eventData.triggerPress = null;
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        ExecuteEvents.Execute<IPointerGripPressUpHandler>(eventData.gripPress, eventData,
          (x, y) => x.OnPointerGripPressUp(eventData));
        eventData.gripPress = null;
        break;
      case OVRInput.RawButton.A:
        ExecuteEvents.Execute<IPointerAPressUpHandler>(eventData.aPress, eventData,
          (x, y) => x.OnPointerAPressUp(eventData));
        eventData.aPress = null;
        break;
      case OVRInput.RawButton.B:
        ExecuteEvents.Execute<IPointerBPressUpHandler>(eventData.bPress, eventData,
          (x, y) => x.OnPointerBPressUp(eventData));
        eventData.bPress = null;
        break;
      case OVRInput.RawButton.X:
        ExecuteEvents.Execute<IPointerXPressUpHandler>(eventData.xPress, eventData,
          (x, y) => x.OnPointerXPressUp(eventData));
        eventData.xPress = null;
        break;
      case OVRInput.RawButton.Y:
        ExecuteEvents.Execute<IPointerYPressUpHandler>(eventData.yPress, eventData,
          (x, y) => x.OnPointerYPressUp(eventData));
        eventData.yPress = null;
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        ExecuteEvents.Execute<IPointerThumbstickPressUpHandler>(eventData.yPress, eventData,
          (x, y) => x.OnPointerThumbstickPressUp(eventData));
        eventData.yPress = null;
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
            ExecuteEvents.Execute<IGlobalTriggerPressDownHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalTriggerPressDown(eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this))) {
            ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalGripPressDown(eventData));
            ExecuteEvents.Execute<IGlobalGripClickHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalGripClick(eventData));
          }
        break;
      case OVRInput.RawButton.A:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalAPressDownHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalAPressDown(eventData));
        break;
      case OVRInput.RawButton.B:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBPressDownHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalBPressDown(eventData));
        break;
      case OVRInput.RawButton.X:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXPressDownHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalXPressDown(eventData));
        break;
      case OVRInput.RawButton.Y:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYPressDownHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalYPressDown(eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickPressDownHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalThumbstickPressDown(eventData));
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
            ExecuteEvents.Execute<IGlobalTriggerPressHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalTriggerPress(eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalGripPressHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalGripPress(eventData));
        break;
      case OVRInput.RawButton.A:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalAPressHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalAPress(eventData));
        break;
      case OVRInput.RawButton.B:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBPressHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalBPress(eventData));
        break;
      case OVRInput.RawButton.X:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXPressHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalXPress(eventData));
        break;
      case OVRInput.RawButton.Y:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYPressHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalYPress(eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickPressHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalThumbstickPress(eventData));
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
            ExecuteEvents.Execute<IGlobalTriggerPressUpHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalTriggerPressUp(eventData));
        break;
      case OVRInput.RawButton.LHandTrigger:
      case OVRInput.RawButton.RHandTrigger:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalGripPressUpHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalGripPressUp(eventData));
        break;
      case OVRInput.RawButton.A:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalAPressUpHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalAPressUp(eventData));
        break;
      case OVRInput.RawButton.B:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalBPressUpHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalBPressUp(eventData));
        break;
      case OVRInput.RawButton.X:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalXPressUpHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalXPressUp(eventData));
        break;
      case OVRInput.RawButton.Y:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalYPressUpHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalYPressUp(eventData));
        break;
      case OVRInput.RawButton.LThumbstick:
      case OVRInput.RawButton.RThumbstick:
        foreach (Receiver r in pressReceivers[id])
          if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
            ExecuteEvents.Execute<IGlobalThumbstickPressUpHandler>(r.gameObject, eventData,
              (x, y) => x.OnGlobalThumbstickPressUp(eventData));
        break;
      default:
        throw new System.Exception("Unknown/Illegal OVRInput.RawButton.");
    }
    //Remove paired list
    pressReceivers[id] = null;
  }

  public void ExecuteTriggerClick() {
    if (eventData.currentRaycast != null) {
      ExecuteEvents.Execute<IPointerTriggerClickHandler>(eventData.currentRaycast, eventData, (x, y) => {
        x.OnPointerTriggerClick(eventData);
      });
    }
  }

  public void ExecuteGripClick() {
    if (eventData.currentRaycast != null) {
      ExecuteEvents.Execute<IPointerGripClickHandler>(eventData.currentRaycast, eventData, (x, y) => {
        x.OnPointerGripClick(eventData);
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

    internal EventData(OculusTouchModule module) : base(module) {
      this.oculusTouchModule = module;
    }

    /// <summary>
    /// Reset the event data fields.
    /// </summary>
    internal override void Reset() {
      base.Reset();
    }
  }
}
