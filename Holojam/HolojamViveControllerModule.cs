#if HOLOJAM
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;
using System.Collections;
using Holojam.Vive;

namespace FRL.IO {
  [RequireComponent(typeof(ViveControllerReceiver))]
  public class HolojamViveControllerModule : PointerInputModule {

    private Dictionary<EVRButtonId, GameObject> pressPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<Receiver>> pressReceivers = new Dictionary<EVRButtonId, List<Receiver>>();
    private Dictionary<EVRButtonId, GameObject> touchPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<Receiver>> touchReceivers = new Dictionary<EVRButtonId, List<Receiver>>();

    private new EventData eventData;

    private ViveControllerReceiver controller;

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

    protected override void Awake() {
      base.Awake();
      controller = this.GetComponent<ViveControllerReceiver>();

      eventData = new EventData(this);

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

      //Why did I add this? May be breaking something. Can't remember. Commenting out for now.
      //if (!Holojam.Tools.BuildManager.IsMasterClient())
      //  return;

      this.HandleButtons();
    }

    public HolojamViveControllerModule.EventData GetEventData() {
      if (!hasBeenProcessed) {
        Process();
      }
      return eventData;
    }

    void HandleButtons() {

      eventData.touchpadAxis = controller.TouchpadAxis;
      eventData.triggerAxis = controller.TriggerAxis;

      //Press
      foreach (EVRButtonId button in pressIds) {
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

      //Touch
      foreach (EVRButtonId button in touchIds) {
        if (GetTouchDown(button)) {
          ExecuteTouchDown(button);
          ExecuteGlobalTouchDown(button);
        } else if (GetTouch(button)) {
          ExecuteTouch(button);
          ExecuteGlobalTouch(button);
        } else if (GetTouchUp(button)) {
          ExecuteTouchUp(button);
          ExecuteGlobalTouchUp(button);
        }
      }

      if(GetClick(EVRButtonId.k_EButton_SteamVR_Trigger))
        ExecuteTriggerClick();
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
              ExecuteEvents.Execute<IGlobalApplicationMenuPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (Receiver r in pressReceivers[id])
            if (r.gameObject.activeInHierarchy && (!r.module || r.module.Equals(this)))
              ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPressDown(eventData));
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
              ExecuteEvents.Execute<IGlobalApplicationMenuPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPress(eventData));
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
              ExecuteEvents.Execute<IGlobalApplicationMenuPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPressUp(eventData));
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

    private bool GetPressDown(EVRButtonId button) {
      return controller.GetPressDown(button);
    }

    private bool GetPress(EVRButtonId button) {
      return controller.GetPress(button);
    }

    private bool GetPressUp(EVRButtonId button) {
      return controller.GetPressUp(button);
    }

    private bool GetTouchDown(EVRButtonId button) {
      return controller.GetTouchDown(button);
    }

    private bool GetTouch(EVRButtonId button) {
      return controller.GetTouch(button);
    }

    private bool GetTouchUp(EVRButtonId button) {
      return controller.GetTouchUp(button);
    }

    private bool GetClick(EVRButtonId button) {
      return controller.GetClick(button);
    }

    public class EventData : VREventData {

      /// <summary>
      /// The ViveControllerModule that manages the instance of ViveEventData.
      /// </summary>
      public HolojamViveControllerModule viveControllerModule {
        get; private set;
      }

      internal EventData(HolojamViveControllerModule module)
        : base(module) {
        this.viveControllerModule = module;
      }

      /// <summary>
      /// Reset the event data fields. 
      /// </summary>
      /// <remarks>
      /// There is currently a warning because this hides AbstractEventData.Reset. This will be removed when
      /// we no longer rely on Unity's event system paradigm.
      /// </remarks>
      internal override void Reset() {
        base.Reset();
        currentRaycast = null;
        previousRaycast = null;
        touchpadAxis = Vector2.zero;
        triggerAxis = Vector2.zero;
        appMenuPress = null;
        gripPress = null;
        touchpadPress = null;
        triggerPress = null;
        touchpadTouch = null;
        triggerTouch = null;
        worldNormal = Vector3.zero;
        worldPosition = Vector3.zero;
      }
    }
  }
}
#endif
