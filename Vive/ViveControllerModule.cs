
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
    

    private EventData castEventData {
      get { return (EventData)eventData; }
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
        SteamVR_Controller.Input((int) controller.index).TriggerHapticPulse(strength);
        yield return null;
      }
    }

    // Duration in seconds, strength is a value from 0 to 3999.
    public void TriggerHapticPulse(float duration, ushort strength) {
      StartCoroutine(Pulse(duration, strength));
    }

    public ViveControllerModule.EventData GetEventData() {
      Update();
      return castEventData;
    }

    void HandleButtons() {
      int index = (int) controller.index;

      float previousX = castEventData.triggerAxis.x;

      castEventData.touchpadAxis = SteamVR_Controller.Input(index).GetAxis(axisIds[0]);
      castEventData.triggerAxis = SteamVR_Controller.Input(index).GetAxis(axisIds[1]);

      //Click
      if (previousX != 1.0f && castEventData.triggerAxis.x == 1f) {
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
          castEventData.appMenuPress = go;
          ExecuteEvents.Execute<IPointerAppMenuPressDownHandler>(castEventData.appMenuPress, eventData,
            (x, y) => x.OnPointerAppMenuPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          castEventData.gripPress = go;
          ExecuteEvents.Execute<IPointerGripPressDownHandler>(castEventData.gripPress, eventData,
            (x, y) => x.OnPointerGripPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          castEventData.touchpadPress = go;
          ExecuteEvents.Execute<IPointerTouchpadPressDownHandler>(castEventData.touchpadPress, eventData,
            (x, y) => x.OnPointerTouchpadPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          castEventData.triggerPress = go;
          ExecuteEvents.Execute<IPointerTriggerPressDownHandler>(castEventData.triggerPress, eventData,
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
          ExecuteEvents.Execute<IPointerAppMenuPressHandler>(castEventData.appMenuPress, eventData,
            (x, y) => x.OnPointerAppMenuPress(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IPointerGripPressHandler>(castEventData.gripPress, eventData,
            (x, y) => x.OnPointerGripPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressHandler>(castEventData.touchpadPress, eventData,
            (x, y) => x.OnPointerTouchpadPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressHandler>(castEventData.triggerPress, eventData,
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
          ExecuteEvents.Execute<IPointerAppMenuPressUpHandler>(castEventData.appMenuPress, eventData,
            (x, y) => x.OnPointerAppMenuPressUp(eventData));
          castEventData.appMenuPress = null;
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IPointerGripPressUpHandler>(castEventData.gripPress, eventData,
            (x, y) => x.OnPointerGripPressUp(eventData));
          castEventData.gripPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<IPointerTouchpadPressUpHandler>(castEventData.touchpadPress, eventData,
            (x, y) => x.OnPointerTouchpadPressUp(eventData));
          castEventData.touchpadPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerPressUpHandler>(castEventData.triggerPress, eventData,
            (x, y) => x.OnPointerTriggerPressUp(eventData));
          castEventData.triggerPress = null;
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
          castEventData.touchpadTouch = go;
          ExecuteEvents.Execute<IPointerTouchpadTouchDownHandler>(castEventData.touchpadTouch, eventData,
            (x, y) => x.OnPointerTouchpadTouchDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          castEventData.triggerTouch = go;
          ExecuteEvents.Execute<IPointerTriggerTouchDownHandler>(castEventData.triggerTouch, eventData,
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
          ExecuteEvents.Execute<IPointerTouchpadTouchHandler>(castEventData.touchpadTouch, eventData,
            (x, y) => x.OnPointerTouchpadTouch(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchHandler>(castEventData.triggerTouch, eventData,
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
          ExecuteEvents.Execute<IPointerTouchpadTouchUpHandler>(castEventData.touchpadTouch, eventData,
            (x, y) => x.OnPointerTouchpadTouchUp(eventData));
          castEventData.touchpadTouch = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<IPointerTriggerTouchUpHandler>(castEventData.triggerTouch, eventData,
            (x, y) => x.OnPointerTriggerTouchUp(eventData));
          castEventData.triggerTouch = null;
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
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
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
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPress(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalGripPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
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
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalGripPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
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
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouchDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
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
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouch(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
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
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouchUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (Receiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
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

    public class EventData : PointerEventData {

      /// <summary>
      /// The ViveControllerModule that manages the instance of ViveEventData.
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


      /// <summary>
      /// The current touchpad axis values of the controller connected to the module.
      /// </summary>
      public Vector2 touchpadAxis {
        get; internal set;
      }

      /// <summary>
      /// The current trigger axis value of the controller connected to the module.
      /// </summary>
      public Vector2 triggerAxis {
        get; internal set;
      }

      /// <summary>
      /// The GameObject bound to the current press context of the Application Menu button.
      /// </summary>
      public GameObject appMenuPress {
        get; internal set;
      }

      /// <summary>
      /// The GameObject bound to the current press context of the Grip button.
      /// </summary>
      public GameObject gripPress {
        get; internal set;
      }

      /// <summary>
      /// The GameObject bound to the current press context of the Touchpad button.
      /// </summary>
      public GameObject touchpadPress {
        get; internal set;
      }

      /// <summary>
      /// The GameObject bound to the current press context of the Trigger button.
      /// </summary>
      public GameObject triggerPress {
        get; internal set;
      }

      /// <summary>
      /// The GameObject bound to the current touch context of the Touchpad button.
      /// </summary>
      public GameObject touchpadTouch {
        get; internal set;
      }

      /// <summary>
      /// The GameObject bound to the current touch context of the Trigger button.
      /// </summary>
      public GameObject triggerTouch {
        get; internal set;
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

        touchpadAxis = Vector2.zero;
        triggerAxis = Vector2.zero;
        appMenuPress = null;
        gripPress = null;
        touchpadPress = null;
        triggerPress = null;
        touchpadTouch = null;
        triggerTouch = null;
      }
    }
  }
}
