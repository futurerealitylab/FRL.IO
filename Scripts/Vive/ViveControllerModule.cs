
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

namespace FRL.IO {
  [RequireComponent(typeof(SteamVR_TrackedObject))]
  public class ViveControllerModule : BaseInputModule {

    /////Public/////
    //References
    //Primitives
    [Tooltip("Optional tag for limiting interaction.")]
    public string interactTag;
    [Range(0, float.MaxValue)]
    [Tooltip("Interaction range of the module.")]
    public float interactDistance = 10f;


    /// <summary>
    /// Pairing dictionaries are used to keep track of the gameObject that is bound to the context
    /// of the press or touch. For example, if a gameObject is pointed at and successfully receives
    /// an OnGripDown, that gameObject is kept in context of that grip press cycle (down,held,up).
    /// 
    /// Receiver dictionaries are used to keep track of which GlobalReceivers existed at the time of execution.
    /// They are populated on "down", and are emptied on "up". A gameObject must exist before the context cycle exists
    /// for it to receive input during that context cycle. 
    /// </summary>
    private Dictionary<EVRButtonId, GameObject> pressPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<GlobalReceiver>> pressReceivers = new Dictionary<EVRButtonId, List<GlobalReceiver>>();
    private Dictionary<EVRButtonId, GameObject> touchPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<GlobalReceiver>> touchReceivers = new Dictionary<EVRButtonId, List<GlobalReceiver>>();

    /// <summary>
    /// The attached controller. 
    /// </summary>
    private SteamVR_TrackedObject controller;

    /// <summary>
    /// The eventData associated with this module. Updated every frame.
    /// </summary>
    private EventData eventData;

    /// <summary>
    /// Raycast fields used for physics raycasting.
    /// </summary>
    private List<RaycastHit> hits = new List<RaycastHit>();
    private Ray ray;

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

    /// <summary>
    /// Gets all necessary bindings, and initializes all dictionaries.
    /// </summary>
    protected void Awake() {

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
    
    /// <summary>
    /// Executes "Up" on all buttons, so that all current contexts are 
    /// ended. Also updates the CurrentObject to be null, so IPointerExitHandler
    /// is correctly executed.
    /// </summary>
    protected void OnDisable() {

      foreach (EVRButtonId button in pressIds) {
        this.ExecutePressUp(button);
        this.ExecuteGlobalPressUp(button);
      }

      foreach (EVRButtonId button in touchIds) {
        this.ExecuteTouchUp(button);
        this.ExecuteGlobalTouchUp(button);
      }

      eventData.currentRaycast = null;
      this.UpdateCurrentObject();
      eventData.Reset();
    }

    /// <summary>
    /// Executes all necessary functions for the module.
    /// </summary>
    void Update() {
      this.Raycast();
      this.UpdateCurrentObject();
      this.HandleButtons();
    }

    /// <summary>
    /// Attempts to find the SteamVR_RenderModel and hide it by disabling it.
    /// Probably could be more sophisticated.
    /// </summary>
    public void HideModel() {
      SteamVR_RenderModel model = GetComponentInChildren<SteamVR_RenderModel> ();
      if (model) {
        model.gameObject.SetActive (false);
      }
    }

    /// <summary>
    /// Attempts to find the SteamVR_RenderModel and show it by enabling it.
    /// Probably could be more sophisticated.
    /// </summary>
    public void ShowModel() {
      SteamVR_RenderModel model = GetComponentInChildren<SteamVR_RenderModel> ();
      if (model) {
        model.gameObject.SetActive (true);
      }
    }


    /// <summary>
    /// Helper IEnumerator function for performing a pulse on the controller.
    /// </summary>
    /// <param name="duration">The duration of the pulse.</param>
    /// <param name="strength">The strength of the pulse.</param>
    /// <returns></returns>
    private IEnumerator Pulse (float duration, ushort strength) {
      float startTime = Time.realtimeSinceStartup;
      while (Time.realtimeSinceStartup - startTime < duration) {
        SteamVR_Controller.Input((int) controller.index).TriggerHapticPulse (strength);
        yield return null;
      }
    }

    /// <summary>
    /// Triggers a haptic pulse on the connected controller, for a given duration and strength.
    /// </summary>
    /// <param name="duration">The duration of the pulse.</param>
    /// <param name="strength">The strength of the pulse.</param>
    public void TriggerHapticPulse(float duration, ushort strength) {
      StartCoroutine (Pulse (duration, strength));
    }

    /// <summary>
    /// Raycasts from the position of the module, for the given interaction distance.
    /// If the interact tag field is populated, that filter will also be applied.
    /// </summary>
    private void Raycast() {
      hits.Clear();

      //cast ray
      Vector3 v = transform.position;
      Quaternion q = transform.rotation;
      ray = new Ray(v, q * Vector3.forward);
      hits.AddRange(Physics.RaycastAll(ray, interactDistance));
      eventData.previousRaycast = eventData.currentRaycast;

      if (hits.Count == 0) {
        eventData.SetCurrentRaycast(null, Vector3.zero, Vector3.zero);
        return;
      }
	
      //find the closest object.
      RaycastHit minHit = hits[0];
      for (int i = 0; i < hits.Count; i++) {
        if (hits[i].distance < minHit.distance) {
          minHit = hits[i];
        }
      }

      //make sure the closest object is able to be interacted with.
      if (interactTag != null && interactTag.Length > 1 
        && !minHit.transform.tag.Equals(interactTag)) {
        eventData.SetCurrentRaycast(null, Vector3.zero, Vector3.zero);
      } else {
        eventData.SetCurrentRaycast(
          minHit.transform.gameObject, minHit.normal, minHit.point);
      }
    }

    /// <summary>
    /// Updates the current object being raycasted to.
    /// </summary>
    void UpdateCurrentObject() {
      this.HandlePointerExitAndEnter(eventData);
    }

    /// <summary>
    /// Executes IPointerEnterHandler and IPointerExitHandler on
    /// the previous and current raycast objects, if necessary.
    /// </summary>
    /// <param name="eventData"></param>
    void HandlePointerExitAndEnter(EventData eventData) {
      if (eventData.previousRaycast != eventData.currentRaycast) {
        ExecuteEvents.Execute<IPointerEnterHandler>(
          eventData.currentRaycast, eventData, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute<IPointerExitHandler>(
          eventData.previousRaycast, eventData, ExecuteEvents.pointerExitHandler);
      }
    }

    /// <summary>
    /// Executes all pointer and global press and touch calls for the controller.
    /// </summary>
    void HandleButtons() {
      int index = (int) controller.index;

      //Update the eventData's information about the touchpad and trigger axes.
      eventData.touchpadAxis = SteamVR_Controller.Input(index).GetAxis(axisIds[0]);
      eventData.triggerAxis = SteamVR_Controller.Input(index).GetAxis(axisIds[1]);

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

    /// <summary>
    /// Executes the button "press down" function on an object being pointed at.
    /// Also creates the pairing of gameObject -> button for the button press context.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecutePressDown(EVRButtonId id) {
      GameObject go = eventData.currentRaycast;
      if (go == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          eventData.applicationMenuPress = go;
          ExecuteEvents.Execute<IApplicationMenuPressDownHandler>(eventData.applicationMenuPress, eventData,
            (x, y) => x.OnApplicationMenuPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          eventData.gripPress = go;
          ExecuteEvents.Execute<IGripPressDownHandler>(eventData.gripPress, eventData,
            (x, y) => x.OnGripPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          eventData.touchpadPress = go;
          ExecuteEvents.Execute<ITouchpadPressDownHandler>(eventData.touchpadPress, eventData,
            (x, y) => x.OnTouchpadPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          eventData.triggerPress = go;
          ExecuteEvents.Execute<ITriggerPressDownHandler>(eventData.triggerPress, eventData,
            (x, y) => x.OnTriggerPressDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Add pairing.
      pressPairings[id] = go;
    }

    /// <summary>
    /// Executes the corresponding button "press held" function on the gameObject that is paired
    /// in the current button press context.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecutePress(EVRButtonId id) {
      if (pressPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          ExecuteEvents.Execute<IApplicationMenuPressHandler>(eventData.applicationMenuPress, eventData,
            (x, y) => x.OnApplicationMenuPress(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IGripPressHandler>(eventData.gripPress, eventData,
            (x, y) => x.OnGripPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<ITouchpadPressHandler>(eventData.touchpadPress, eventData,
            (x, y) => x.OnTouchpadPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<ITriggerPressHandler>(eventData.triggerPress, eventData,
            (x, y) => x.OnTriggerPress(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    /// <summary>
    /// Executes the button "press up" function on the gameObject that is paired
    /// in the current button press context. Also removes the pairing.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecutePressUp(EVRButtonId id) {
      if (pressPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          ExecuteEvents.Execute<IApplicationMenuPressUpHandler>(eventData.applicationMenuPress, eventData,
            (x, y) => x.OnApplicationMenuPressUp(eventData));
          eventData.applicationMenuPress = null;
          break;
        case EVRButtonId.k_EButton_Grip:
          ExecuteEvents.Execute<IGripPressUpHandler>(eventData.gripPress, eventData,
            (x, y) => x.OnGripPressUp(eventData));
          eventData.gripPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<ITouchpadPressUpHandler>(eventData.touchpadPress, eventData,
            (x, y) => x.OnTouchpadPressUp(eventData));
          eventData.touchpadPress = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<ITriggerPressUpHandler>(eventData.triggerPress, eventData,
            (x, y) => x.OnTriggerPressUp(eventData));
          eventData.triggerPress = null;
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Remove pairing.
      pressPairings[id] = null;
    }

    /// <summary>
    /// Executes the button touch "touch down" function on an object being pointed at.
    /// Also creates the pairing of gameObject -> button for the button touch context.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteTouchDown(EVRButtonId id) {
      GameObject go = eventData.currentRaycast;
      if (go == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          eventData.touchpadTouch = go;
          ExecuteEvents.Execute<ITouchpadTouchDownHandler>(eventData.touchpadTouch, eventData,
            (x, y) => x.OnTouchpadTouchDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          eventData.triggerTouch = go;
          ExecuteEvents.Execute<ITriggerTouchDownHandler>(eventData.triggerTouch, eventData,
            (x, y) => x.OnTriggerTouchDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Add pairing.
      touchPairings[id] = go;
    }

    /// <summary>
    /// Executes the button "touch held" function on the gameObject that is paired
    /// in the current button press context. 
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteTouch(EVRButtonId id) {
      if (touchPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<ITouchpadTouchHandler>(eventData.touchpadTouch, eventData,
            (x, y) => x.OnTouchpadTouch(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<ITriggerTouchHandler>(eventData.triggerTouch, eventData,
            (x, y) => x.OnTriggerTouch(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    /// <summary>
    /// Executes the button "touch up" function on the gameObject that is paired
    /// in the current button press context. Also removes the pairing.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteTouchUp(EVRButtonId id) {
      if (touchPairings[id] == null)
        return;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          ExecuteEvents.Execute<ITouchpadTouchUpHandler>(eventData.touchpadTouch, eventData,
            (x, y) => x.OnTouchpadTouchUp(eventData));
          eventData.touchpadTouch = null;
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          ExecuteEvents.Execute<ITriggerTouchUpHandler>(eventData.triggerTouch, eventData,
            (x, y) => x.OnTriggerTouchUp(eventData));
          eventData.triggerTouch = null;
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }

      //Remove pairing.
      touchPairings[id] = null;
    }

    /// <summary>
    /// Executes the global button "press down" on all active global receivers.
    /// Also saves a list of globalReceivers that existed at press down.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteGlobalPressDown(EVRButtonId id) {
      //Add paired list.
      pressReceivers[id] = GlobalReceiver.instances;

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalGripPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPressDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTriggerPressDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerPressDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    /// <summary>
    /// Executes the global button "press held" on all global receivers 
    /// that were active when the button was initially pressed.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteGlobalPress(EVRButtonId id) {
      if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPress(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalGripPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPress(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTriggerPressHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerPress(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    /// <summary>
    /// Executes the global button "press up" on all global receivers 
    /// that were active when the button was initially pressed.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteGlobalPressUp(EVRButtonId id) {
      if (pressReceivers[id] == null || pressReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_ApplicationMenu:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalApplicationMenuPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_Grip:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalGripPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalGripPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (GlobalReceiver r in pressReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadPressUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (GlobalReceiver r in pressReceivers[id])
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

    /// <summary>
    /// Executes the global button "touch down" on all active global receivers.
    /// Also saves a list of globalReceivers that existed at touch down.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteGlobalTouchDown(EVRButtonId id) {
      touchReceivers[id] = GlobalReceiver.instances;

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (GlobalReceiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouchDown(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (GlobalReceiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTriggerTouchDownHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerTouchDown(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    /// <summary>
    /// Executes the global button "touch held" on all global receivers 
    /// that were active when the button was initially touched.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteGlobalTouch(EVRButtonId id) {
      if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (GlobalReceiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouch(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (GlobalReceiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTriggerTouchHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTriggerTouch(eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal EVRButtonId.");
      }
    }

    /// <summary>
    /// Executes the global button "touch up" on all global receivers 
    /// that were active when the button was initially touched.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    private void ExecuteGlobalTouchUp(EVRButtonId id) {
      if (touchReceivers[id] == null || touchReceivers[id].Count == 0) {
        return;
      }

      switch (id) {
        case EVRButtonId.k_EButton_SteamVR_Touchpad:
          foreach (GlobalReceiver r in touchReceivers[id])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchUpHandler>(r.gameObject, eventData,
                (x, y) => x.OnGlobalTouchpadTouchUp(eventData));
          break;
        case EVRButtonId.k_EButton_SteamVR_Trigger:
          foreach (GlobalReceiver r in touchReceivers[id])
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

    /// <summary>
    /// Returns whether or not the EVRButtonId button has been pressed down this frame
    /// on the controller with the index.
    /// </summary>
    /// <param name="index">The index of the controller.</param>
    /// <param name="button">The EVRButtonId of the button.</param>
    /// <returns></returns>
    private bool GetPressDown(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetPressDown(button);
    }

    /// <summary>
    /// Returns whether or not the EVRButtonId button has been pressed this frame
    /// on the controller with the index.
    /// </summary>
    /// <param name="index">The index of the controller.</param>
    /// <param name="button">The EVRButtonId of the button.</param>
    /// <returns></returns>
    private bool GetPress(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetPress(button);
    }

    /// <summary>
    /// Returns whether or not the EVRButtonId button has been released this frame.
    /// on the controller with the index.
    /// </summary>
    /// <param name="index">The index of the controller.</param>
    /// <param name="button">The EVRButtonId of the button.</param>
    /// <returns></returns>
    private bool GetPressUp(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetPressUp(button);
    }

    /// <summary>
    /// Returns whether or not the EVRButtonId button has been touched down this frame
    /// on the controller with the index.
    /// </summary>
    /// <param name="index">The index of the controller.</param>
    /// <param name="button">The EVRButtonId of the button.</param>
    /// <returns></returns>
    private bool GetTouchDown(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetTouchDown(button);
    }

    /// <summary>
    /// Returns whether or not the EVRButtonId button has been touched this frame
    /// on the controller with the index.
    /// </summary>
    /// <param name="index">The index of the controller.</param>
    /// <param name="button">The EVRButtonId of the button.</param>
    /// <returns></returns>
    private bool GetTouch(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetTouch(button);
    }

    /// <summary>
    /// Returns whether or not the EVRButtonId button has been touch released this frame
    /// on the controller with the index.
    /// </summary>
    /// <param name="index">The index of the controller.</param>
    /// <param name="button">The EVRButtonId of the button.</param>
    /// <returns></returns>
    private bool GetTouchUp(int index, EVRButtonId button) {
      return SteamVR_Controller.Input(index).GetTouchUp(button);
    }
    

    /// <summary>
    /// EventData contains all relevant information to the module at any given frame.
    /// It is passed along in and IViveHandler function call.
    /// </summary>
    public class EventData : BaseEventData {

      /// <summary>
      /// The SteamVR Tracked Object connected to the module.
      /// </summary>
      public SteamVR_TrackedObject steamVRTrackedObject {
        get; private set;
      }

      /// <summary>
      /// The GameObject currently hit by a raycast from the module.
      /// </summary>
      public GameObject currentRaycast {
        get; internal set;
      }

      /// <summary>
      /// The GameObject previously hit by a raycast from the module.
      /// </summary>
      public GameObject previousRaycast {
        get; internal set;
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
      public GameObject applicationMenuPress {
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

      /// <summary>
      /// The world normal of the current raycast, if it exists. Otherwise, this will equal Vector3.zero.
      /// </summary>
      public Vector3 worldNormal {
        get; internal set;
      }

      /// <summary>
      /// The world position of the current raycast, if it exists. Otherwise, this will equal Vector3.zero.
      /// </summary>
      public Vector3 worldPosition {
        get; internal set;
      }

      internal EventData(ViveControllerModule module, SteamVR_TrackedObject trackedObject) : base(module) {
        this.steamVRTrackedObject = trackedObject;
      }

                              /// <summary>
                              /// Reset the event data fields. 
                              /// </summary>
                              /// <remarks>
                              /// There is currently a warning because this hides AbstractEventData.Reset. This will be removed when
                              /// we no longer rely on Unity's event system paradigm.
                              /// </remarks>
      internal void Reset() {
        currentRaycast = null;
        previousRaycast = null;
        touchpadAxis = Vector2.zero;
        triggerAxis = Vector2.zero;
        applicationMenuPress = null;
        gripPress = null;
        touchpadPress = null;
        triggerPress = null;
        touchpadTouch = null;
        triggerTouch = null;
        worldNormal = Vector3.zero;
        worldPosition = Vector3.zero;
      }

      internal void SetCurrentRaycast(GameObject go, Vector3 normal, Vector3 position) {
        this.currentRaycast = go;
        this.worldNormal = normal;
        this.worldPosition = position;
      }
    }
  }
}
