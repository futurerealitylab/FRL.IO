using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public class DaydreamControllerModule : PointerInputModule {
    private EventData _eventData;

    protected override VREventData eventData {
      get {
        return _eventData;
      }
    }

#if UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)

    private Dictionary<Button, GameObject> pairings = new Dictionary<Button, GameObject>();
    private Dictionary<Button, List<Receiver>> receivers = new Dictionary<Button, List<Receiver>>();

    private enum Button {
      CLICK,TOUCH,APPBUTTON
    }

    private Button[] buttonTypes = new Button[] {
      Button.CLICK, Button.TOUCH, Button.APPBUTTON
    };



    protected override void Awake() {
      _eventData = new EventData(this);

      foreach(Button b in buttonTypes) {
        pairings.Add(b, null);
        receivers.Add(b, null);
      }
    }

    protected override void OnDisable() {
      base.OnDisable();

      foreach(Button b in buttonTypes) {
        ExecuteButtonUp(b);
        ExecuteGlobalButtonUp(b);
      }

      _eventData.Reset();
    }

    // Update is called once per frame
    void Update() {
      if (!hasBeenProcessed) {
        transform.localRotation = GvrController.Orientation;
        Process();
      }
    }

    private void LateUpdate() {
      hasBeenProcessed = false;
    }

    public EventData GetEventData() {
      Update();
      return (EventData)_eventData;
    }

    protected override void Process() {
      base.Process();
      this.HandleButtons();
    }



    void HandleButtons() {
      _eventData.touchpadAxis = GetTouchpadAxis();

      foreach(Button button in buttonTypes) {
        if (GetButtonDown(button)) {
          ExecuteButtonDown(button);
          ExecuteGlobalButtonDown(button);
        } else if (GetButton(button)) {
          ExecuteButton(button);
          ExecuteGlobalButton(button);
        } else if (GetButtonUp(button)) {
          ExecuteButtonUp(button);
          ExecuteGlobalButtonUp(button);
        }
      }
    }

    private void ExecuteButtonDown(Button button) {
      GameObject go = _eventData.currentRaycast;
      if (go == null)
        return;

      Receiver r = go.GetComponent<Receiver>();
      if (r != null && r.module != null && r.module != this)
        return;

      switch (button) {
        case Button.CLICK:
          _eventData.touchpadPress = go;
          ExecuteEvents.Execute<IPointerTouchpadPressDownHandler>(_eventData.touchpadPress, _eventData,
            (x, y) => x.OnPointerTouchpadPressDown(_eventData));
          break;
        case Button.TOUCH:
          _eventData.touchpadTouch = go;
          ExecuteEvents.Execute<IPointerTouchpadTouchDownHandler>(_eventData.touchpadTouch, _eventData,
            (x, y) => x.OnPointerTouchpadTouchDown(_eventData));
          break;
        case Button.APPBUTTON:
          _eventData.appMenuPress = go;
          ExecuteEvents.Execute<IPointerAppMenuPressDownHandler>(_eventData.appMenuPress, _eventData,
            (x, y) => x.OnPointerAppMenuPressDown(_eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal Daydream Button.");
      }

      pairings[button] = go;
    }

    private void ExecuteButton(Button button) {
      if (pairings[button] == null) return;

      switch (button) {
        case Button.CLICK:
          ExecuteEvents.Execute<IPointerTouchpadPressHandler>(_eventData.touchpadPress, _eventData,
            (x, y) => x.OnPointerTouchpadPress(_eventData));
          break;
        case Button.TOUCH:
          ExecuteEvents.Execute<IPointerTouchpadTouchHandler>(_eventData.touchpadTouch, _eventData,
            (x, y) => x.OnPointerTouchpadTouch(_eventData));
          break;
        case Button.APPBUTTON:
          ExecuteEvents.Execute<IPointerAppMenuPressHandler>(_eventData.appMenuPress, _eventData,
            (x, y) => x.OnPointerAppMenuPress(_eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal Daydream Button.");
      }
    }

    private void ExecuteButtonUp(Button button) {
      if (pairings[button] == null) return;

      switch (button) {
        case Button.APPBUTTON:
          ExecuteEvents.Execute<IPointerAppMenuPressUpHandler>(_eventData.appMenuPress, _eventData,
            (x, y) => x.OnPointerAppMenuPressUp(_eventData));
          _eventData.appMenuPress = null;
          break;
        case Button.CLICK:
          ExecuteEvents.Execute<IPointerTouchpadPressUpHandler>(_eventData.touchpadPress, _eventData,
            (x, y) => x.OnPointerTouchpadPressUp(_eventData));
          _eventData.touchpadPress = null;
          break;
        case Button.TOUCH:
          ExecuteEvents.Execute<IPointerTouchpadTouchUpHandler>(_eventData.touchpadTouch, _eventData,
            (x, y) => x.OnPointerTouchpadTouchUp(_eventData));
          _eventData.touchpadTouch = null;
          break;
        default:
          throw new System.Exception("Unknown/Illegal Daydream Button.");
      }

      //Remove pairing.
      pairings[button] = null;
    }

    private void ExecuteGlobalButtonDown(Button button) {
      //Add paired list.
      receivers[button] = Receiver.instances;

      switch (button) {
        case Button.APPBUTTON:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalApplicationMenuPressDown(_eventData));
          break;
        case Button.CLICK:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadPressDown(_eventData));
          break;
        case Button.TOUCH:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchDownHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadTouchDown(_eventData));
          break;

        default:
          throw new System.Exception("Unknown/Illegal Daydream Button.");
      }
    }

    private void ExecuteGlobalButton(Button button) {
      if (receivers[button] == null) return;

      switch (button) {
        case Button.APPBUTTON:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalApplicationMenuPress(_eventData));
          break;
        case Button.CLICK:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadPress(_eventData));
          break;
        case Button.TOUCH:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadTouch(_eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal Daydream Button.");
      }
    }

    private void ExecuteGlobalButtonUp(Button button) {
      if (receivers[button] == null) return;

      switch (button) {
        case Button.APPBUTTON:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalApplicationMenuPressUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalApplicationMenuPressUp(_eventData));
          break;
        case Button.CLICK:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadPressUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadPressUp(_eventData));
          break;
        case Button.TOUCH:
          foreach (Receiver r in receivers[button])
            if (!r.module || r.module.Equals(this))
              ExecuteEvents.Execute<IGlobalTouchpadTouchUpHandler>(r.gameObject, _eventData,
                (x, y) => x.OnGlobalTouchpadTouchUp(_eventData));
          break;
        default:
          throw new System.Exception("Unknown/Illegal Daydream Button.");
      }

      //Remove paired list.
      receivers[button] = null;
    }

    /// <summary>
    /// GVR Touchpad is normally (0,0) at top left and (1,1) at top right.
    /// Convert to (-1,-1) bottom left and (1,1) top right.
    /// </summary>
    /// <returns>The touch position on the touchpad.</returns>
    private Vector2 GetTouchpadAxis() {
      if (!GvrController.IsTouching) return Vector2.zero;

      Vector2 axis = GvrController.TouchPos;

      axis = new Vector2(axis.x, -1f * axis.y);

      axis = (axis - new Vector2(0.5f, -0.5f)) * 2f;
      //axis = (axis - new Vector2(-0.5f, -0.5f)) * 2f;

      return axis;
    }


    private bool GetButtonDown(Button button) {
      switch (button) {
        case Button.CLICK:
          return GvrController.ClickButtonDown;
        case Button.TOUCH:
          return GvrController.TouchDown;
        case Button.APPBUTTON:
          return GvrController.AppButtonDown;
        default:
          return false;
      }
    }

    private bool GetButton(Button button) {
      switch (button) {
        case Button.CLICK:
          return GvrController.ClickButton;
        case Button.TOUCH:
          return GvrController.IsTouching;
        case Button.APPBUTTON:
          return GvrController.AppButton;
        default:
          return false;
      }
    }

    private bool GetButtonUp(Button button) {
      switch (button) {
        case Button.CLICK:
          return GvrController.ClickButtonUp;
        case Button.TOUCH:
          return GvrController.TouchUp;
        case Button.APPBUTTON:
          return GvrController.AppButtonUp;
        default:
          return false;
      }
    }
#endif
    public class EventData : VREventData {

      public DaydreamControllerModule daydreamControllerModule {
        get; internal set;
      }

      internal EventData(DaydreamControllerModule module) : base(module) {
        this.daydreamControllerModule = module;
      }

      internal override void Reset() {
        base.Reset();
      }
    }
  }
}

