using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL {
  public class XRManager : MonoBehaviour {

    private static XRManager instance;

    public static XRSystem CurrentSystem { get { return instance.System; } }

    public bool switchBuildTargetOnChange;

    [SerializeField]
    [HideInInspector]
    private XRSystem _system;
    public XRSystem System {
      get { return _system; }
    }

    private void Awake() {
      if (instance) {
        Debug.LogError("There can only be one XRManager. Duplicate removed from: " + this.name);
        Destroy(this);
        return;
      }
      instance = this;
    }

    public void SwitchToSystem(XRSystem system) {

#if OVR
      OVRManager ovr;
      if (ovr = GetComponent<OVRManager>()) {
        ovr.enabled = (system == XRSystem.CV1 || System == XRSystem.GearVR);
      } else {
        Debug.LogError("Please attach an OVRManager to the XRManager Gameobject!");
        return;
      }
#else
      if (system == XRSystem.CV1 || system == XRSystem.GearVR) {
        Debug.LogError("Cannot switch to " + system + " without OVR support!");
        return;
      }
#endif

      Debug.Log("Switching to system: " + system);
      _system = system;
      XRDevice[] devices = GetComponentsInChildren<XRDevice>();
      foreach (XRDevice device in devices) {
        device.System = system;
      }
    }

    void Start() {
      SwitchToSystem(System);
    }
  }
}

