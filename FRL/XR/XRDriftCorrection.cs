using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL {
  public class XRDriftCorrection : MonoBehaviour {

    public enum Axis { XZ, XYZ }

    public Axis axis = Axis.XZ;
    public float correctionSpeed = 0.025f;
    public XRHMD hmd;
    public Camera head;

    public Transform goal;
    public Vector3 offset;

    private void Start() {
      if (!hmd) hmd = GetComponent<XRHMD>();
      if (!head) head = Camera.main;
    }

    // Update is called once per frame
    void Update() {
      if (!goal || !head || !hmd) return;

      Vector3 currentPosition = head.transform.position;
      Vector3 goalPosition = goal.transform.position + goal.transform.rotation * offset;
      Vector3 diff = goalPosition - currentPosition;

      if (axis == Axis.XZ) diff.y = 0;

      //if (diff.magnitude > 1f)
      //  hmd.transform.Translate(diff);
      //else
      hmd.transform.Translate(diff.normalized * correctionSpeed * Time.deltaTime);
    }

    public void SetNewGoal(Transform goal) {
      if (!goal) return;
      this.goal = goal;
    }
  }
}

