using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FRL {
  public class XRManager : MonoBehaviour {

    private static XRManager instance;
    public static XRSystem CurrentSystem { get { return instance.System; } }


    private List<string> allSDKs = new List<string>() {
      "WAVE", "OVR", "STEAM_VR", "DAYDREAM"
    };

    private List<string> sdkNames = new List<string>() {
      "Wave [Vive Focus]", "OVR [GearVR/Oculus Go and CV1]", "SteamVR [Vive Haptics]", "Daydream [Mirage Solo]"
    };

    private List<bool> enabledSDKs = new List<bool>() {
      false, false, false, false
    };

    public List<string> AllSDKs { get { return allSDKs; } }
    public List<bool> EnabledSDKs { get { return enabledSDKs; } }
    public List<string> SDKNames { get { return sdkNames; } }

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
      if (!(ovr = GetComponent<OVRManager>())) {
        Debug.Log("Adding OVRManager to XRManager gameObject.");
        ovr = this.gameObject.AddComponent<OVRManager>();
        ovr.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
      }
      ovr.enabled = (system == XRSystem.CV1 || system == XRSystem.GearVROculusGo);
#else
      if (system == XRSystem.CV1 || system == XRSystem.GearVROculusGo) {
        Debug.LogError("Cannot switch to " + system + " without OVR SDK!");
        return;
      }
#endif

#if DAYDREAM
      GvrControllerInput controller;
      if (!(controller = GetComponent<GvrControllerInput>())) {
        Debug.Log("Adding GvrControllerInput component to XRManager gameObject.");
        controller = gameObject.AddComponent<GvrControllerInput>();
      }
      controller.enabled = system == XRSystem.Daydream;

      GvrHeadset headset;
      if (!(headset = GetComponent<GvrHeadset>())) {
        Debug.Log("Adding GvrHeadset component to XRManager gameObject.");
        headset = gameObject.AddComponent<GvrHeadset>();
      }
      headset.enabled = system == XRSystem.Daydream;
#else
      if (system == XRSystem.Daydream) {
        Debug.LogError("Cannot switch to " + system + " without Daydream SDK!");
        return;
      }
#endif

#if WAVE
      Camera cam = GetComponentInChildren<Camera>();

      WaveVR_Render render;
      if (!(render = cam.GetComponent<WaveVR_Render>())) {
        Debug.Log("Adding WaveVR_Render to Main Camera gameObject.");
        render = cam.gameObject.AddComponent<WaveVR_Render>();
        render.origin = wvr.WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround;

      }
      render.enabled = system == XRSystem.ViveFocus;

      WaveVR_DevicePoseTracker tracker;
      if (!(tracker = cam.GetComponent<WaveVR_DevicePoseTracker>())) {
        Debug.Log("Adding WaveVR_DevicePoseTracker to Main Camera gameObject.");
        tracker = cam.gameObject.AddComponent<WaveVR_DevicePoseTracker>();
        tracker.type = wvr.WVR_DeviceType.WVR_DeviceType_HMD;
        tracker.trackPosition = true;
        tracker.trackRotation = true;
        tracker.timing = WaveVR_DevicePoseTracker.TrackingEvent.WhenNewPoses;
      }
      tracker.enabled = system == XRSystem.ViveFocus;
#else
      if (system == XRSystem.ViveFocus) {
        Debug.LogError("Cannot switch to " + system + " without Wave SDK!");
        return;
      }
#endif


      XRDevice[] devices = GetComponentsInChildren<XRDevice>();
      foreach (XRDevice device in devices) {
        device.System = system;
      }
      _system = system;
      Debug.Log("Switched to " + system);
    }

    void Start() {
      SwitchToSystem(System);
    }
  }
}

