
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

    private EventData _eventData;

    protected override VREventData eventData {
      get {
        return _eventData;
      }
    }

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

      _eventData = new EventData(this, controller);

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
      return _eventData;
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

      float previousX = _eventData.triggerAxis;

      _eventData.touchpadAxis = GetTouchpadAxis();
      _eventData.triggerAxis = GetTriggerAxis();

      //Click
      if (previousX != 1.0f && _eventData.triggerAxis == 1f) {
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
      GameObject go = _eventData.currentRaycast;
      if (go == null)
        return;

      //If there's a receiver component, only cast to it if it's this module.
      Receiver r = go.GetComponent<Receiver>();
      if (r != null && r.module != null && r.module != this)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          _eventData.appMenuPress = go;
          ExecuteEvents.Execute<IPointerAppMenuPressDownHandler>(_eventData.appMenuPress, _eventData,
            (x, y) => x.OnPointerAppMenuPressDown(_eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          _eventData.gripPress = go;
          ExecuteEvents.Execute<IPointerGripPressDownHandler>(_eventData.gripPress, _eventData,
            (x, y) => x.OnPointerGripPressDown(_eventData));
          ExecuteEvents.Execute<IPointerGripClickHandler>(_eventData.gripPress, _eventData,
              (x, y) => x.OnPointerGripClick(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          _eventData.touchpadPress = go;
          ExecuteEvents.Execute<IPointerTouchpadPressDownHandler>(_eventData.touchpadPress, _eventData,
            (x, y) => x.OnPointerTouchpadPressDown(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          _eventData.triggerPress = go;
          ExecuteEvents.Execute<IPointerTriggerPressDownHandler>(_eventData.triggerPress, _eventData,
            (x, y) => x.OnPointerTriggerPressDown(_eventData));
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
          ExecuteEvents.Execute<IPointerAppMenuPressHandler>(_eventData.appMenuPress, _eventData,
            (x, y) => x.OnPointerAppMenuPress(_eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IPointerGripPressHandler>(_eventData.gripPress, _eventData,
            (x, y) => x.OnPointerGripPress(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressHandler>(_eventData.touchpadPress, _eventData,
            (x, y) => x.OnPointerTouchpadPress(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressHandler>(_eventData.triggerPress, _eventData,
            (x, y) => x.OnPointerTriggerPress(_eventData));
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
          ExecuteEvents.Execute<IPointerAppMenuPressUpHandler>(_eventData.appMenuPress, _eventData,
            (x, y) => x.OnPointerAppMenuPressUp(_eventData));
          _eventData.appMenuPress = null;
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IPointerGripPressUpHandler>(_eventData.gripPress, _eventData,
            (x, y) => x.OnPointerGripPressUp(_eventData));
          _eventData.gripPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressUpHandler>(_eventData.touchpadPress, _eventData,
            (x, y) => x.OnPointerTouchpadPressUp(_eventData));
          _eventData.touchpadPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressUpHandler>(_eventData.triggerPress, _eventData,
            (x, y) => x.OnPointerTriggerPressUp(_eventData));
          _eventData.triggerPress = null;
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Remove pairing.
      pressPairings[id] = null;
    }

    private void ExecuteTouchDown(EVRButtonId id) {
      GameObject go = _eventData.currentRaycast;
      if (go == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          _eventData.touchpadTouch = go;
          ExecuteEvents.Execute<IPointerTouchpadTouchDownHandler>(_eventData.touchpadTouch, _eventData,
            (x, y) => x.OnPointerTouchpadTouchDown(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          _eventData.triggerTouch = go;
          ExecuteEvents.Execute<IPointerTriggerTouchDownHandler>(_eventData.triggerTouch, _eventData,
            (x, y) => x.OnPointerTriggerTouchDown(_eventData));
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
          ExecuteEvents.Execute<IPointerTouchpadTouchHandler>(_eventData.touchpadTouch, _eventData,
            (x, y) => x.OnPointerTouchpadTouch(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchHandler>(_eventData.triggerTouch, _eventData,
            (x, y) => x.OnPointerTriggerTouch(_eventData));
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
          ExecuteEvents.Execute<IPointerTouchpadTouchUpHandler>(_eventData.touchpadTouch, _eventData,
            (x, y) => x.OnPointerTouchpadTouchUp(_eventData));
          _eventData.touchpadTouch = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchUpHandler>(_eventData.triggerTouch, _eventData,
            (x, y) => x.OnPointerTriggerTouchUp(_eventData));
          _eventData.triggerTouch = null;
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
              ExecuteEvents.Execute<IGlobalAppMenuPressDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalAppMenuPressDown(_eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this))) {
              ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalGripPressDown(_eventData));
              ExecuteEvents.Execute<IGlobalGripClickHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalGripClick(_eventData));
            }
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadPressDown(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTriggerPressDown(_eventData));
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
              ExecuteEvents.Execute<IGlobalAppMenuPressHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalAppMenuPress(_eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalGripPress(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadPress(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTriggerPress(_eventData));
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
              ExecuteEvents.Execute<IGlobalAppMenuPressUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalAppMenuPressUp(_eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalGripPressUp(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTouchpadPressUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadPressUp(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerPressUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTriggerPressUp(_eventData));
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
              ExecuteEvents.Execute<IGlobalTouchpadTouchDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadTouchDown(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerTouchDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTriggerTouchDown(_eventData));
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
              ExecuteEvents.Execute<IGlobalTouchpadTouchHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadTouch(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalTriggerTouchHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTriggerTouch(_eventData));
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
              ExecuteEvents.Execute<IGlobalTouchpadTouchUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadTouchUp(_eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))

              ExecuteEvents.Execute<IGlobalTriggerTouchUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTriggerTouchUp(_eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Remove paired list.
      touchReceivers[id] = null;
    }

    private void ExecuteTriggerClick() {
      if (_eventData.currentRaycast != null) {
        ExecuteEvents.Execute<IPointerTriggerClickHandler>(_eventData.currentRaycast, _eventData, (x, y) => {
          x.OnPointerTriggerClick(_eventData);
        });
      }

      foreach (Receiver r in Receiver.instances) {
        if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
          ExecuteEvents.Execute<IGlobalTriggerClickHandler>(r.gameObject, _eventData, (x, y) => {
            x.OnGlobalTriggerClick(_eventData);
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
