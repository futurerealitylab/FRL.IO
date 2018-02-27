using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL {
  public class XRManager : MonoBehaviour {

    private bool supportExternalSDKs = false;
    private Dictionary<string, bool> supportedExternalSDKs = new Dictionary<string, bool>() {
      { "WAVE", false },
      { "OVR", false },
      { "STEAM_VR", false },
      { "DAYDREAM", false }
    };

    private static XRManager instance;

    public static XRSystem CurrentSystem { get { return instance.System; } }

    public bool SupportExternalSDKs { 
      get { return supportExternalSDKs; }
      set { supportExternalSDKs = value; }
    }

    public Dictionary<string, bool> SupportedExternalSDKs {
      get { return supportedExternalSDKs; }
    }

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
        Debug.LogError("Cannot switch to " + system + " without OVR SDK!");
        return;
      }
#endif

#if DAYDREAM
      if (system == XRSystem.Daydream) {
        if (!GetComponent<GvrControllerInput>()) {
          Debug.Log("Adding GvrControllerInput component to XRManager gameObject.");
          gameObject.AddComponent<GvrControllerInput>();
        }
        if (!GetComponent<GvrHeadset>()) {
          Debug.Log("Adding GvrHeadset component to XRManager gameObject.");
          gameObject.AddComponent<GvrHeadset>();
        }
      }
#else
      if (system == XRSystem.Daydream) {
        Debug.LogError("Cannot switch to " + system + " without Daydream SDK!");
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

