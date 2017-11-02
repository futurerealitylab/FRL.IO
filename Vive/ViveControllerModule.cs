
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;
using System.Collections;

namespace FRL.IO {
  public class ViveControllerModule : PointerInputModule {
    private Dictionary<EVRButtonId, GameObject> pressPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<Receiver>> pressReceivers = new Dictionary<EVRButtonId, List<Receiver>>();
    private Dictionary<EVRButtonId, GameObject> touchPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<Receiver>> touchReceivers = new Dictionary<EVRButtonId, List<Receiver>>();
    private SteamVR_TrackedObject controller;

    private new EventData eventData;

    //Steam Controller button and axis ids
    private EVRButtonId[] pressIds = new EVRButtonId[] {
      EVRButtonId.k_EButton_ApplicationMenu,
      EVRButtonId.k_EButton_Grip,
      EVRButtonId.k_EButton_SteamVR_Touchpad,
      EVRButtonId.k_EButton_SteamVR_Trigger
    };

    private EVRButtonId[] touchIds = new EVRButtonId[] {
      EVRButtonId.k_EButton_SteamVR_Touchpad,
      EVRButtonId.k_EButton_SteamVR_Trigger
    };

    private EVRButtonId[] axisIds = new EVRButtonId[] {
      EVRButtonId.k_EButton_SteamVR_Touchpad,
      EVRButtonId.k_EButton_SteamVR_Trigger
    };

    protected override void Awake() {
      base.Awake();
      controller = this.GetComponent<SteamVR_TrackedObject>();

      eventData = new EventData(this, controller);

      foreach (EVRButtonId button in pressIds) {
        pressPairings.Add(button, null);
        pressReceivers.Add(button, null);
      }

      foreach (EVRButtonId button in touchIds) {
        touchPairings.Add(button, null);
        touchReceivers.Add(button, null);
      }
    }

    protected override void OnDisable() {
      base.OnDisable();

      foreach (EVRButtonId button in pressIds) {
        this.ExecutePressUp(button);
        this.ExecuteGlobalPressUp(button);
      }

      foreach (EVRButtonId button in touchIds) {
        this.ExecuteTouchUp(button);
        this.ExecuteGlobalTouchUp(button);
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

    public void HideModel() {
      SteamVR_RenderModel model = GetComponentInChildren<SteamVR_RenderModel>();
      if (model) {
        model.gameObject.SetActive(false);
      }
    }

    public void ShowModel() {
      SteamVR_RenderModel model = GetComponentInChildren<SteamVR_RenderModel>();
      if (model) {
        model.gameObject.SetActive(true);
      }
    }

    IEnumerator Pulse(float duration, ushort strength) {
      float startTime = Time.realtimeSinceStartup;
      while (Time.realtimeSinceStartup - startTime < duration) {
        SteamVR_Controller.Input((int)controller.index).TriggerHapticPulse(strength);
        yield return null;
      }
    }

    // Duration in seconds, strength is a value from 0 to 3999.
    public void TriggerHapticPulse(float duration, ushort strength) {
      StartCoroutine(Pulse(duration, strength));
    }

    public void TriggerHapticPulse(ushort strength) {
      try {
        SteamVR_Controller.Input((int)controller.index).TriggerHapticPulse(strength);
      } catch (System.Exception e) {
        Debug.LogWarning("Attempted trigger haptic pulse on null controller.");
      }

    }

    public ViveControllerModule.EventData GetEventData() {
      Update();
      return eventData;
    }

    public float GetTriggerAxis() {
      try {
        return SteamVR_Controller.Input((int)controller.index).GetAxis(axisIds[1]).x;
      } catch (System.Exception e) {
        return 0f;
      }
    }

    public Vector2 GetTouchpadAxis() {
      try {
        return SteamVR_Controller.Input((int)controller.index).GetAxis(axisIds[0]);
      } catch (System.Exception e) {
        return Vector2.zero;
      }
    }

    void HandleButtons() {
      int index = (int)controller.index;

      float previousX = eventData.triggerAxis;

      eventData.touchpadAxis = GetTouchpadAxis();
      eventData.triggerAxis = GetTriggerAxis();

      //Click
      if (previousX != 1.0f && eventData.triggerAxis == 1f) {
        ExecuteTriggerClick();
      }


      //Press
      foreach (EVRButtonId button in pressIds) {
        if (GetPressDown(index, button)) {
          ExecutePressDown(button);
          ExecuteGlobalPressDown(button);
        } else if (GetPress(index, button)) {
          ExecutePress(button);
          ExecuteGlobalPress(button);
        } else if (GetPressUp(index, button)) {
          ExecutePressUp(button);
          ExecuteGlobalPressUp(button);
        }
      }

      //Touch
      foreach (EVRButtonId button in touchIds) {
        if (GetTouchDown(index, button)) {
          ExecuteTouchDown(button);
          ExecuteGlobalTouchDown(button);
        } else if (GetTouch(index, button)) {
          ExecuteTouch(button);
          ExecuteGlobalTouch(button);
        } else if (GetTouchUp(index, button)) {
          ExecuteTouchUp(button);
          ExecuteGlobalTouchUp(button);
        }
      }
    }

    private void ExecutePressDown(EVRButtonId id) {
      GameObject go = eventData.currentRaycast;
      if (go == null)
        return;

      //If there's a receiver component, only cast to it if it's this module.
      Receiver r = go.GetComponent<Receiver>();
      if (r != null && r.module != null && r.module != this)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          eventData.appMenuPress = go;
          ExecuteEvents.Execute<IPointerAppMenuPressDownHandler>(eventData.appMenuPress, eventData,
            (x, y) => x.OnPointerAppMenuPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          eventData.gripPress = go;
          ExecuteEvents.Execute<IPointerGripPressDownHandler>(eventData.gripPress, eventData,
            (x, y) => x.OnPointerGripPressDown(eventData));
          ExecuteEvents.Execute<IPointerGripClickHandler>(eventData.gripPress, eventData,
              (x, y) => x.OnPointerGripClick(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          eventData.touchpadPress = go;
          ExecuteEvents.Execute<IPointerTouchpadPressDownHandler>(eventData.touchpadPress, eventData,
            (x, y) => x.OnPointerTouchpadPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          eventData.triggerPress = go;
          ExecuteEvents.Execute<IPointerTriggerPressDownHandler>(eventData.triggerPress, eventData,
            (x, y) => x.OnPointerTriggerPressDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Add pairing.
      pressPairings[id] = go;
    }

    private void ExecutePress(EVRButtonId id) {
      if (pressPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          ExecuteEvents.Execute<IPointerAppMenuPressHandler>(eventData.appMenuPress, eventData,
            (x, y) => x.OnPointerAppMenuPress(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IPointerGripPressHandler>(eventData.gripPress, eventData,
            (x, y) => x.OnPointerGripPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressHandler>(eventData.touchpadPress, eventData,
            (x, y) => x.OnPointerTouchpadPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressHandler>(eventData.triggerPress, eventData,
            (x, y) => x.OnPointerTriggerPress(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    private void ExecutePressUp(EVRButtonId id) {
      if (pressPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          ExecuteEvents.Execute<IPointerAppMenuPressUpHandler>(eventData.appMenuPress, eventData,
            (x, y) => x.OnPointerAppMenuPressUp(eventData));
          eventData.appMenuPress = null;
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IPointerGripPressUpHandler>(eventData.gripPress, eventData,
            (x, y) => x.OnPointerGripPressUp(eventData));
          eventData.gripPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressUpHandler>(eventData.touchpadPress, eventData,
            (x, y) => x.OnPointerTouchpadPressUp(eventData));
          eventData.touchpadPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressUpHandler>(eventData.triggerPress, eventData,
            (x, y) => x.OnPointerTriggerPressUp(eventData));
          eventData.triggerPress = null;
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Remove pairing.
      pressPairings[id] = null;
    }

    private void ExecuteTouchDown(EVRButtonId id) {
      GameObject go = eventData.currentRaycast;
      if (go == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          eventData.touchpadTouch = go;
          ExecuteEvents.Execute<IPointerTouchpadTouchDownHandler>(eventData.touchpadTouch, eventData,
            (x, y) => x.OnPointerTouchpadTouchDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          eventData.triggerTouch = go;
          ExecuteEvents.Execute<IPointerTriggerTouchDownHandler>(eventData.triggerTouch, eventData,
            (x, y) => x.OnPointerTriggerTouchDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Add pairing.
      touchPairings[id] = go;
    }

    private void ExecuteTouch(EVRButtonId id) {
      if (touchPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadTouchHandler>(eventData.touchpadTouch, eventData,
            (x, y) => x.OnPointerTouchpadTouch(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchHandler>(eventData.triggerTouch, eventData,
            (x, y) => x.OnPointerTriggerTouch(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    private void ExecuteTouchUp(EVRButtonId id) {
      if (touchPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadTouchUpHandler>(eventData.touchpadTouch, eventData,
            (x, y) => x.OnPointerTouchpadTouchUp(eventData));
          eventData.touchpadTouch = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchUpHandler>(eventData.triggerTouch, eventData,
            (x, y) => x.OnPointerTriggerTouchUp(eventData));
          eventData.triggerTouch = null;
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Remove pairing.
      touchPairings[id] = null;
    }

    private void ExecuteGlobalPressDown(EVRButtonId id) {
      //Add paired list.
      pressReceivers[id] = Receiver.instances;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalAppMenuPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalAppMenuPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this))) {
              ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPressDown(eventData));
              ExecuteEvents.Execute<IGlobalGripClickHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripClick(eventData));
            }
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerPressDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    private void ExecuteGlobalPress(EVRButtonId id) {
      if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalAppMenuPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalAppMenuPress(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerPress(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    private void ExecuteGlobalPressUp(EVRButtonId id) {
      if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalAppMenuPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalAppMenuPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerPressUp(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
      //Remove paired list
      pressReceivers[id] = null;
    }

    private void ExecuteGlobalTouchDown(EVRButtonId id) {
      touchReceivers[id] = Receiver.instances;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadTouchDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouchDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerTouchDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerTouchDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    private void ExecuteGlobalTouch(EVRButtonId id) {
      if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadTouchHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouch(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerTouchHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerTouch(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    private void ExecuteGlobalTouchUp(EVRButtonId id) {
      if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadTouchUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouchUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))

              ExecuteEvents.Execute<IGlobalTriggerTouchUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerTouchUp(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Remove paired list.
      touchReceivers[id] = null;
    }

    private void ExecuteTriggerClick() {
      if (eventData.currentRaycast != null) {
        ExecuteEvents.Execute<IPointerTriggerClickHandler>(eventData.currentRaycast, eventData, (x, y) => {
          x.OnPointerTriggerClick(eventData);
        });
      }

      foreach (Receiver r in Receiver.instances) {
        if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
          ExecuteEvents.Execute<IGlobalTriggerClickHandler>(r.gameObject, eventData, (x, y) => {
            x.OnGlobalTriggerClick(eventData);
          });
      }
    }

    private bool GetPressDown(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetPressDown(button);
    }

    private bool GetPress(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetPress(button);
    }

    private bool GetPressUp(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetPressUp(button);
    }

    private bool GetTouchDown(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetTouchDown(button);
    }

    private bool GetTouch(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetTouch(button);
    }

    private bool GetTouchUp(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetTouchUp(button);
    }
    public class EventData : VREventData {
      /// <summary>
      /// The ViveControllerModule that manages the instance of EventData.
      /// </summary>
      public ViveControllerModule viveControllerModule {
        get; private set;
      }

      /// <summary>
      /// The SteamVR Tracked Object connected to the module.
      /// </summary>
      public SteamVR_TrackedObject steamVRTrackedObject {
        get; private set;
      }
      internal EventData(ViveControllerModule module, SteamVR_TrackedObject trackedObject)
        : base(module) {
        this.viveControllerModule = module;
        this.steamVRTrackedObject = trackedObject;
      }
      /// <summary>
      /// Reset the event data fields. 
      /// </summary>
      internal override void Reset() {
        base.Reset();
      }
    }
  }
}
