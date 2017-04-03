using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public class VREventData : PointerEventData {

    public Vector2 touchpadAxis {
      get;
      internal set;
    }

    public Vector2 triggerAxis {
      get;
      internal set;
    }

    /// <summary>
    /// The GameObject bound to the current press context of the Touchpad button.
    /// </summary>
    public GameObject touchpadPress {
      get;
      internal set;
    }

    /// <summary>
    /// The GameObject bound to the current press context of the Trigger button.
    /// </summary>
    public GameObject triggerPress {
      get;
      internal set;
    }

    /// <summary>
    /// The GameObject bound to the current touch context of the Touchpad button.
    /// </summary>
    public GameObject touchpadTouch {
      get;
      internal set;
    }

    /// <summary>
    /// The GameObject bound to the current touch context of the Trigger button.
    /// </summary>
    public GameObject triggerTouch {
      get;
      internal set;
    }

    /// <summary>
    /// The GameObject bound to the current press context of the Application Menu button.
    /// </summary>
    public GameObject appMenuPress {
      get;
      internal set;
    }

    /// <summary>
    /// The GameObject bound to the current press context of the Grip button.
    /// </summary>
    public GameObject gripPress {
      get;
      internal set;
    }

    internal VREventData(BaseInputModule module) : base(module) { }

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

