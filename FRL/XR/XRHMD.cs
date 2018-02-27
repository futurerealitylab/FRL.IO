using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using FRL.Network;
using UnityEngine.XR;

namespace FRL {
  public class XRHMD : XRDevice {

    public static XRHMD instance;
    public Transform standaloneCameraGoal;
    public Transform gearVRCameraGoal;

    public int trackingNumber = 1;

    //private XRSensorFusion sensorFusion;
    private XRMouseLook mouseLook;

    public string TrackingLabel {
      get { return "T" + trackingNumber.ToString(); }
    }

    private new Camera camera;
    public Camera Camera {
      get { return camera; }
    }

    public Vector3 TrackedPosition {
      get { return camera.transform.position; }
    }

    public Quaternion TrackedRotation {
      get { return camera.transform.rotation; }
    }

    protected virtual void Awake() {
      if (instance) {
        Debug.LogWarning("Only one instance of XRSystemHMD allowed.");
        return;
      }
      instance = this;
      camera = GetComponentInChildren<Camera>();
      UpdateTrackingNumber(PlayerPrefs.GetInt("TrackingNumber", -1));
    }

    protected virtual void OnDestroy() {
      if (instance == this) instance = null;
    }

    protected override void OnSystemSwitch(XRSystem system) {
      if (system == XRSystem.Standalone && standaloneCameraGoal) {
        this.transform.localPosition = standaloneCameraGoal.localPosition;
        this.transform.localRotation = standaloneCameraGoal.localRotation;
      } else if (System == XRSystem.GearVR && gearVRCameraGoal) {
        this.transform.localPosition = gearVRCameraGoal.localPosition;
        this.transform.localRotation = gearVRCameraGoal.localRotation;
      } else {
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
      }

      //if (sensorFusion = GetComponent<XRSensorFusion>())
      //  sensorFusion.enabled = system == XRSystem.GearVR;
      if (mouseLook = GetComponent<XRMouseLook>())
        mouseLook.enabled = system == XRSystem.Standalone;
#if WAVE
      WaveVR_Render waveRender = GetComponentInChildren<WaveVR_Render>();
      if (waveRender) waveRender.enabled = system == XRSystem.ViveFocus;
      WaveVR_DevicePoseTracker waveTracker = GetComponentInChildren<WaveVR_DevicePoseTracker>();
      if (waveTracker) waveTracker.enabled = system == XRSystem.ViveFocus;
#endif
    }

    public void UpdateTrackingNumber(int num) {
      this.trackingNumber = num;
      PlayerPrefs.SetInt("TrackingNumber",num);
    }

    public void UpdateTrackingNumber(string s) {
      this.UpdateTrackingNumber(int.Parse(s));
    }

    protected override void Update() {
      base.Update();
      switch (System) {
        case XRSystem.Standalone:
          UpdateStandalone();
          break;
        case XRSystem.CV1:
          UpdateCV1();
          break;
        case XRSystem.GearVR:
          UpdateGearVR();
          break;
      }
    }

    void UpdateStandalone() {
      float x = Input.GetAxis("Horizontal");
      float y = Input.GetAxis("Vertical");
      bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

      float speed = shift ? 3f : 1f;
      transform.Translate(new Vector3(x * speed * Time.deltaTime, 0f, y * speed * Time.deltaTime));
    }

    void UpdateCV1() {
      isTracked = InputTracking.GetLocalPosition(XRNode.Head) != Vector3.zero &&
        InputTracking.GetLocalRotation(XRNode.Head) != Quaternion.identity;
    }

    void UpdateGearVR() {
      isTracked = true;
    }
  }
}

