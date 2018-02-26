using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.IO;

namespace FRL {
  public class XRController : XRDevice {

    private static List<XRController> _controllers = new List<XRController>();
    public static List<XRController> Controllers {
      get { return _controllers; }
    }

    protected virtual void OnEnable() {
      _controllers.Add(this);
    }

    protected virtual void OnDisable() {
      _controllers.Remove(this);
    }

    protected override void Update() {
      base.Update();
      XRControllerModule module = GetComponent<XRControllerModule>();
      if (module != null) {
        this.isTracked = module.IsTracked;
      } else {
        this.isTracked = false;
      }
    }

    protected override void OnSystemSwitch(XRSystem system) {
      XRControllerModule module = GetComponent<XRControllerModule>();
      if (module) module.System = system;
    }
  }
}

