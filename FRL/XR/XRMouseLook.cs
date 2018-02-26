using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL {
  [RequireComponent(typeof(XRHMD))]
  public class XRMouseLook : MonoBehaviour {

    private XRHMD HMD;
    private MouseLook mouseLook = new MouseLook();

    // Use this for initialization
    void Start() {
      HMD = GetComponent<XRHMD>();
      mouseLook.Init(HMD.transform, HMD.Camera.transform);
    }

    // Update is called once per frame
    void Update() {
      mouseLook.LookRotation(HMD.transform, HMD.Camera.transform);
    }
  }
}
