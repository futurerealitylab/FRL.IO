
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;
using System.Collections;

namespace FRL.IO {
  public class ViveControllerModule : BaseInputModule {

    [Tooltip("Test input mode. Arrow and letter keys can be used to aim and emulate button actions.")]
    public bool testInput = false;
    [Tooltip("Test ray. Basic ray that goes from slightly in front of the gameObject to either the interaction distance, or the ray hit.")]
    public bool testRay = false;

    [Tooltip("Optional tag for limiting interaction.")]
    public string interactTag;
    [Range(0, float.MaxValue)]
    [Tooltip("Interaction range of the module.")]
    public float interactDistance = 10f;

    private Dictionary<EVRButtonId, GameObject> pressPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<Receiver>> pressReceivers = new Dictionary<EVRButtonId, List<Receiver>>();
    private Dictionary<EVRButtonId, GameObject> touchPairings = new Dictionary<EVRButtonId, GameObject>();
    private Dictionary<EVRButtonId, List<Receiver>> touchReceivers = new Dictionary<EVRButtonId, List<Receiver>>();
    private SteamVR_TrackedObject controller;
    private EventData eventData;

    private List<RaycastHit> hits = new List<RaycastHit>();
    private Ray ray;

    private bool hasBeenProcessed = false;


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


    private Dictionary<KeyCode, EVRButtonId> keysToPressIds = new Dictionary<KeyCode, EVRButtonId> {
      {KeyCode.Q,EVRButtonId.k_EButton_ApplicationMenu},
      {KeyCode.G,EVRButtonId.k_EButton_Grip},
      {KeyCode.P,EVRButtonId.k_EButton_SteamVR_Touchpad},
      {KeyCode.T,EVRButtonId.k_EButton_SteamVR_Trigger}
    };

    private Dictionary<KeyCode, EVRButtonId> keysToTouchIds = new Dictionary<KeyCode, EVRButtonId> {
      {KeyCode.O,EVRButtonId.k_EButton_SteamVR_Touchpad},
      {KeyCode.R,EVRButtonId.k_EButton_SteamVR_Trigger}
    };


    private LineRenderer line;

    protected void Awake() {
      controller = this.GetComponent<SteamVR_TrackedObject>();

      if (!controller) {
        testInput = true;
      }

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
      this.DestroyTestRay();
    }

    void Update() {
      if (!hasBeenProcessed) {
        Process();
      }
    }

    void LateUpdate() {
      hasBeenProcessed = false;
    }

    void Process() {
      this.Raycast();
      this.UpdateCurrentObject();
      hasBeenProcessed = true;

      if (testInput) {
        this.RotateGameObjectByArrows();
      } else {
        this.HandleButtons();
      }

      this.HandleTestInput();
      this.HandleTestRay();
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
      if (!hasBeenProcessed) {
        Process();
      }
      return eventData;
    }

    private void Raycast() {
      hits.Clear();

      //CAST RAY
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

    void UpdateCurrentObject() {
      this.HandlePointerExitAndEnter(eventData);
    }

    void HandlePointerExitAndEnter(EventData eventData) {
      if (eventData.previousRaycast != eventData.currentRaycast) {
        ExecuteEvents.Execute<IPointerEnterHandler>(
          eventData.currentRaycast, eventData, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute<IPointerExitHandler>(
          eventData.previousRaycast, eventData, ExecuteEvents.pointerExitHandler);
      } else if (eventData.currentRaycast != null) {
        ExecuteEvents.Execute<IPointerStayHandler>(
          eventData.currentRaycast, eventData, (x, y) => {
            x.OnPointerStay(eventData);
          });
      }
    }

    void RotateGameObjectByArrows() {
      if (Input.GetKey(KeyCode.LeftArrow)) {
        transform.Rotate(Vector3.down * 90f * Time.deltaTime);
      }
      if (Input.GetKey(KeyCode.RightArrow)) {
        transform.Rotate(Vector3.up * 90f * Time.deltaTime);
      }
      if (Input.GetKey(KeyCode.UpArrow)) {
        transform.Rotate(Vector3.left * 90f * Time.deltaTime);
      }
      if (Input.GetKey(KeyCode.DownArrow)) {
        transform.Rotate(Vector3.right * 90f * Time.deltaTime);
      }
    }

    void HandleTestRay() {
      if (testRay && line == null) {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = Color.cyan;
        line.SetWidth(0.01f, 0.01f);
      } else if (!testRay && line != null) {
        DestroyTestRay();
      }

      //Handle the line
      if (line != null) {
        Vector3 startPoint = this.transform.position + this.transform.forward * 0.05f + this.transform.up * -0.01f;

        line.SetVertexCount(2);
        line.SetPosition(0, startPoint);
        if (eventData.currentRaycast != null) {
          line.SetPosition(1, eventData.worldPosition);
        } else {
          line.SetPosition(1, startPoint + this.transform.forward * interactDistance);
        }
      }
    }

    void DestroyTestRay() {
      Destroy(line);
      line = null;
    }

    void HandleTestInput() {
      //Translate the mousepad to be the touchpad.
      Vector2 axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
      eventData.touchpadAxis = axis;

      //Handle "press" keys
      foreach (KeyValuePair<KeyCode, EVRButtonId> kvp in keysToPressIds) {
        if (Input.GetKeyDown(kvp.Key)) {
          ExecutePressDown(kvp.Value);
          ExecuteGlobalPressDown(kvp.Value);
          if (kvp.Value == EVRButtonId.k_EButton_SteamVR_Trigger) {
            ExecuteTriggerClick();
          }
        } else if (Input.GetKey(kvp.Key)) {
          ExecutePress(kvp.Value);
          ExecuteGlobalPress(kvp.Value);
        } else if (Input.GetKeyUp(kvp.Key)) {
          ExecutePressUp(kvp.Value);
          ExecutePressUp(kvp.Value);
        }
      }

      //Handle "touch" keys
      foreach (KeyValuePair<KeyCode, EVRButtonId> kvp in keysToTouchIds) {
        if (Input.GetKeyDown(kvp.Key)) {
          ExecuteTouchDown(kvp.Value);
          ExecuteGlobalTouchDown(kvp.Value);
        } else if (Input.GetKey(kvp.Key)) {
          ExecuteTouch(kvp.Value);
          ExecuteGlobalTouchDown(kvp.Value);
        } else if (Input.GetKeyUp(kvp.Key)) {
          ExecuteTouchUp(kvp.Value);
          ExecuteGlobalTouchUp(kvp.Value);
        }
      }
    }

    void HandleButtons() {
      int index = (int) controller.index;

      float previousX = eventData.triggerAxis.x;

      eventData.touchpadAxis = SteamVR_Controller.Input(index).GetAxis(axisIds[0]);
      eventData.triggerAxis = SteamVR_Controller.Input(index).GetAxis(axisIds[1]);

      //Click
      if (previousX != 1.0f && eventData.triggerAxis.x == 1f) {
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
      /// <remarks>
      /// There is currently a warning because this hides AbstractEventData.Reset. This will be removed when
      /// we no longer rely on Unity's event system paradigm.
      /// </remarks>
      internal void Reset() {
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

      internal void SetCurrentRaycast(GameObject go, Vector3 normal, Vector3 position) {
        this.currentRaycast = go;
        this.worldNormal = normal;
        this.worldPosition = position;
      }
    }
  }
}
