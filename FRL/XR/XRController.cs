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
      if (module) {
        module.System = system;

        //if GVR/Go, disable all modules that don't track this hand
        if (System == XRSystem.GearVROculusGo && Controllers.Count > 1) {
          GVRVerifyController(module);
        }
      }
    }

    private void GVRVerifyController(XRControllerModule module) {
#if OVR
      OVRInput.Controller controller = OVRInput.GetActiveController();
      CheckGVRController.Text += module.hand + " | " + controller + "\n";
      if (controller == OVRInput.Controller.None ||
         (module.hand == XRHand.Left && controller == OVRInput.Controller.RTrackedRemote) ||
         (module.hand == XRHand.Right && controller == OVRInput.Controller.LTrackedRemote)) {
        Debug.Log("Removing " + gameObject.name);
        gameObject.SetActive(false);
        this.enabled = false;
        module.enabled = false;
      }
#endif
      }
  }
}

