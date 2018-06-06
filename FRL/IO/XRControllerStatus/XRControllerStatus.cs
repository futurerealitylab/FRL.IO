using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public abstract class XRControllerStatus {

    public XRHand hand;

    public XRControllerStatus(XRHand hand = XRHand.None) {
      this.hand = hand;
    }

    protected float pTriggerAxis, pGripAxis, cTriggerAxis, cGripAxis;
    protected Vector3 pTouchpadAxis, pThumbstickAxis, cTouchpadAxis, cThumbstickAxis;

    protected Vector3 pPos, cPos;
    protected Quaternion pRot, cRot;
    protected Vector3 pVel, cVel, pAcc, cAcc;

    public Vector3 Position { get { return cPos; } }
    public Quaternion Rotation { get { return cRot; } }
    public Vector3 Velocity { get { return cVel; } }
    public Vector3 Acceleration { get { return cAcc; } }
    public float GripAxis { get { return cGripAxis; } }
    public float TriggerAxis { get { return cTriggerAxis; } }
    public Vector2 ThumbstickAxis { get { return cThumbstickAxis; } }
    public Vector2 TouchpadAxis { get { return cTouchpadAxis; } }

    public virtual bool IsTracked {
      get {
        return pPos != cPos || pRot != cRot;
      }
    }

    public void Generate() {
      //Save previous status.
      pPos = cPos;
      pRot = cRot;
      pVel = cVel;
      pAcc = cAcc;
      pTriggerAxis = cTriggerAxis;
      pGripAxis = cGripAxis;
      pTouchpadAxis = cTouchpadAxis;
      pThumbstickAxis = cThumbstickAxis;

      //Generate current status.
      GenerateCurrentStatus();
   
      //Generate velocity and Acceleration.
      GenerateVelocityAndAcceleration();
    }

    protected virtual void GenerateVelocityAndAcceleration() {
      cVel = (cPos - pPos) / Time.deltaTime;
      cAcc = (cVel - pVel) / Time.deltaTime;
    }

    public virtual void HapticPulse(AnimationCurve curve, float time) {
      return;
    }

    /// <summary>
    /// These samples will be played at whatever Hz the controller supports.
    /// </summary>
    public virtual void HapticPulse(byte[] samples) {
      return;
    }

    public abstract bool GetClick(XRButton button);
    public abstract bool GetPressDown(XRButton button);
    public abstract bool GetPress(XRButton button);
    public abstract bool GetPressUp(XRButton button);
    public abstract bool GetTouchDown(XRButton button);
    public abstract bool GetTouch(XRButton button);
    public abstract bool GetTouchUp(XRButton button);

    protected abstract void GenerateCurrentStatus();

    public override string ToString() {
      string s = "";

      s += "Controller: " + System.Enum.GetName(typeof(XRHand), hand) + "\n";
      s += "\tIs Tracked: " + IsTracked.ToString() + "\n";
      s += "\tPosition: " + Position.ToString() + "\n";
      s += "\tRotation: " + Rotation.ToString() + "\n";
      s += "\tVelocity: " + Velocity.ToString() + "\n";
      s += "\tAcceleration: " + Acceleration.ToString() + "\n";
      s += "\tGripAxis: " + GripAxis.ToString() + "\n";
      s += "\tTriggerAxis: " + TriggerAxis.ToString() + "\n";
      s += "\tThumbstickAxis: " + ThumbstickAxis.ToString() + "\n";
      s += "\tTouchpadAxis: " + TouchpadAxis.ToString() + "\n";

      return s;
    }
  }

}

