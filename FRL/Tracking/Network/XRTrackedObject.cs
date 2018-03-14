using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.Network;
using FRL.Utility;
using UnityEngine.UI;

public class XRTrackedObject : MonoBehaviour {

  public enum Mode { Position, Rotation, Both }

  public string label;
  public Mode mode = Mode.Both;
  public SmoothingType smoothing;

  public bool isTracked = false;
  public bool IMUSensorFusion = false;

  public Quaternion IMURot { get; private set; }

  [HideInInspector]
  public float lowPassFactor = 1f;

  private Vector3 lastPosition = Vector3.zero;
  private Quaternion lastRotation = Quaternion.identity;
  private bool lastTracked = false;
  private Quaternion lastIMURot;
  private Smoother smoother;

  public Transform cameraTransform;

  private Vector3 velocity;
  private Vector3 lastVelocity;
  private float maxMagnitude = 1f;

  private float lastTrackedTime = 0f;

  public bool imuOnly = false;

  // Use this for initialization
  void Start() {
    Input.gyro.enabled = true;
    lastPosition = transform.position;
    lastRotation = transform.rotation;
  }

  // Update is called once per frame
  void Update() {

    this.isTracked = XRNetworkClient.IsTracked(label);
    this.smoother = Smoother.GetSmoother(smoothing);

    IMURot = GetIMURotation();
    var newIMU = Quaternion.Slerp(lastIMURot, IMURot, lowPassFactor);
    lastIMURot = IMURot;
    IMURot = newIMU;

    velocity = (transform.position - lastPosition) / Time.deltaTime;
    velocity = Vector3.ClampMagnitude(velocity, maxMagnitude);

    imuOnly = !isTracked;

    if (mode != Mode.Rotation) transform.position = GetCurrentPosition();
    if (mode != Mode.Position) transform.rotation = GetCurrentRotation();

    lastPosition = transform.position;
    lastRotation = transform.rotation;
    lastTracked = isTracked;
    if (lastTracked) lastTrackedTime = Time.time;
    lastVelocity = velocity;
  }

  public Vector3 GetCurrentPosition() {
    var sourcePosition = XRNetworkClient.GetPosition(label);

    if (isTracked) {
      if (lastTracked && smoother != null)
        return smoother.Smooth(sourcePosition, ref lastPosition, Time.deltaTime);
      else
        return sourcePosition;
    } else
      return lastPosition;// + Vector3.Lerp(lastPosition, lastPosition + lastVelocity, Time.time - lastTrackedTime);
  }

  public Quaternion GetCurrentRotation() {
    
    var sourceRotation = XRNetworkClient.GetRotation(label);
    Quaternion inv = Quaternion.Inverse(IMURot);

    //If we're using sensor fusion, Camera rotation should be IMU rotation.
    if (IMUSensorFusion && cameraTransform) cameraTransform.localRotation = IMURot;
    else if (cameraTransform) cameraTransform.localRotation = Quaternion.identity;

    if (this.isTracked) {
      if (IMUSensorFusion) {
        //Sensor fusion. Calculate the y-rotation difference, and return.
        sourceRotation *= inv;
        if (smoother != null) sourceRotation = smoother.Smooth(sourceRotation, ref lastRotation, Time.deltaTime);
        return Quaternion.AngleAxis(sourceRotation.eulerAngles.y, Vector3.up);
      } else {
        //No sensor fusion. Smooth based on smoother.
        if (smoother != null) sourceRotation = smoother.Smooth(sourceRotation, ref lastRotation, Time.deltaTime);
        return sourceRotation;
      }
    } else {
      //Untracked. Return last rotation.
      return lastRotation;
    }
  }

  public Quaternion GetIMURotation() {
    return Quaternion.Euler(90f, 0f, 90f) * GyroToUnity(Input.gyro.attitude);
  }

  private static Quaternion GyroToUnity(Quaternion q) {
    return new Quaternion(q.x, q.y, -q.z, -q.w);
  }
}
