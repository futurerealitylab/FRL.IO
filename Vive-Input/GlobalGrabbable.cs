using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FRL.IO;

//Require a GlobalReceiver component and a Collider component on this gameObject.
[RequireComponent(typeof(GlobalReceiver))]
[RequireComponent(typeof(Collider))]
public class GlobalGrabbable : MonoBehaviour,IGlobalTriggerPressSetHandler {

  public bool expectGrabber = true;

  private GlobalReceiver receiver;

  private Collider collider;
  private Rigidbody rbody;

  private Vector3 offset = Vector3.zero;
  private Quaternion rotOffset = Quaternion.identity;

  private BaseInputModule grabbingModule;

  private List<Vector3> savedPositions = new List<Vector3>();
  private int savedPosCount = 60;

  void Awake() {
    //Get the GlobalReceiver component on this gameObject.
    receiver = this.GetComponent<GlobalReceiver>();
    //Get the Collider component on this gameObject.
    collider = this.GetComponent<Collider>();
    rbody = this.GetComponent<Rigidbody>();
  }

  void OnDisable() {
    if (grabbingModule != null) {
      Release(grabbingModule);
    }
  }


  void Grab(BaseInputModule module) {
    Debug.Log(name + " grabbed by: " + module.name);
    //Bind the module to this object.
    grabbingModule = module;
    //Save the offset between the module and this object. Undo the current rotation of the module
    offset = transform.position - grabbingModule.transform.position;
    offset = Quaternion.Inverse(grabbingModule.transform.rotation) * offset;
    rotOffset = Quaternion.Inverse(grabbingModule.transform.rotation) * transform.rotation;
    savedPositions.Add(transform.position);

    collider.isTrigger = true;

    if (rbody) {
      rbody.isKinematic = true;
    }
  }

  void Hold(BaseInputModule module) {
    Debug.Log(name + " held by: " + module.name);
    this.transform.position = grabbingModule.transform.position + grabbingModule.transform.rotation * offset;
    this.transform.rotation = grabbingModule.transform.rotation * rotOffset;

    savedPositions.Add(transform.position);
    if (savedPositions.Count > savedPosCount) {
      savedPositions.RemoveAt(savedPosCount - 1);
    }
  }

  void Release(BaseInputModule module) {
    Debug.Log(name + " released by: " + module.name);

    if (rbody) {
      rbody.isKinematic = false;

      Vector3 force = Vector3.zero;
      for (int i = 1; i < savedPositions.Count; i++) {
        Vector3 delta = savedPositions[i] - savedPositions[i - 1];

        //Ignore spurious changes (sudden jumps caused by external scripts or states)
        if (delta.magnitude > 1f) {
          continue;
        }

        force += delta;
      }

      rbody.AddForceAtPosition(force, grabbingModule.transform.position, ForceMode.Impulse);
      savedPositions.Clear();
    }

    offset = Vector3.zero;
    grabbingModule = null;
    collider.isTrigger = false;
  }


  /// <summary>
  /// This function is called when the trigger is initially pressed. Called once per press context.
  /// </summary>
  /// <param name="eventData">The corresponding event data for the module.</param>
  public void OnGlobalTriggerPressDown(ViveControllerModule.EventData eventData) {
    //Only "grab" the object if it's within the bounds of the object.
    //If the object has already been grabbed, ignore this event call.
    if (collider.bounds.Contains(eventData.module.transform.position) && grabbingModule == null) {
      //Check for a GlobalGrabber if this object should expect one.
      if (!expectGrabber || (expectGrabber && eventData.module.GetComponent<GlobalGrabber>() != null
        && eventData.module.GetComponent<GlobalGrabber>().isActiveAndEnabled)) {
        Grab(eventData.module);
      }
    }
  }

  /// <summary>
  /// This function is called every frame between the initial press and release of the trigger.
  /// </summary>
  /// <param name="eventData">The corresponding event data for the module.</param>
  public void OnGlobalTriggerPress(ViveControllerModule.EventData eventData) {
    //Only accept this call if it's from the module currently grabbing this object.
    if (grabbingModule == eventData.module) {
      //Check for a GlobalGrabber if this object should expect one.
      if (!expectGrabber) {
        Hold(eventData.module);
      } else if (expectGrabber) {
        GlobalGrabber grabber = eventData.module.GetComponent<GlobalGrabber>();
        if (grabber != null && grabber.isActiveAndEnabled) {
          Hold(eventData.module);
        } else {
          Release(eventData.module);
        }
      }
    }
  }


  /// <summary>
  /// This function is called when the trigger is released. Called once per press context.
  /// </summary>
  /// <param name="eventData">The corresponding event data for the module.</param>
  public void OnGlobalTriggerPressUp(ViveControllerModule.EventData eventData) {
    //If the grabbing module releases it's trigger, unbind it from this object.
    if (grabbingModule == eventData.module) {
      Release(eventData.module);
    }
  }
}
