using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FRL.IO;
using System;

//Require a GlobalReceiver component and a Collider component on this gameObject.
[RequireComponent(typeof(Receiver))]
[RequireComponent(typeof(Collider))]
public class Grabbable : MonoBehaviour, IGlobalTriggerPressSetHandler, IPointerTriggerPressSetHandler {

  /// <summary>
  /// Expect a Grabber.cs component on the grabbing controller.
  /// </summary>
  public bool expectGrabber = true;

  public bool pointerGrab = true;
  public bool colliderGrab = true;
  
  private Collider collider;
  private Rigidbody rbody;

  private Vector3 offset = Vector3.zero;
  private Quaternion rotOffset = Quaternion.identity;

  private BaseInputModule grabbingModule;

  private List<Vector3> savedPositions = new List<Vector3>();
  private int savedPosCount = 60;

  void Awake() {
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
    this.transform.position = grabbingModule.transform.position + grabbingModule.transform.rotation * offset;
    this.transform.rotation = grabbingModule.transform.rotation * rotOffset;

    savedPositions.Add(transform.position);
    if (savedPositions.Count > savedPosCount) {
      savedPositions.RemoveAt(savedPosCount - 1);
    }
  }

  void Release(BaseInputModule module) {
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

      rbody.AddForce(force, ForceMode.Impulse);
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
  public void OnGlobalTriggerPressDown(BaseEventData eventData) {
    //Only "grab" the object if it's within the bounds of the object.
    //If the object has already been grabbed, ignore this event call.
    if (collider.bounds.Contains(eventData.module.transform.position) && grabbingModule == null && colliderGrab) {
      //Check for a GlobalGrabber if this object should expect one.
      if (!expectGrabber || (expectGrabber && eventData.module.GetComponent<Grabber>() != null
        && eventData.module.GetComponent<Grabber>().isActiveAndEnabled)) {
        Grab(eventData.module);
      }
    }
  }

  /// <summary>
  /// This function is called every frame between the initial press and release of the trigger.
  /// </summary>
  /// <param name="eventData">The corresponding event data for the module.</param>
  public void OnGlobalTriggerPress(BaseEventData eventData) {
    //Only accept this call if it's from the module currently grabbing this object.
    if (grabbingModule == eventData.module) {
      //Check for a GlobalGrabber if this object should expect one.
      if (!expectGrabber) {
        Hold(eventData.module);
      } else if (expectGrabber) {
        Grabber grabber = eventData.module.GetComponent<Grabber>();
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
  public void OnGlobalTriggerPressUp(BaseEventData eventData) {
    
    //If the grabbing module releases it's trigger, unbind it from this object.
    if (grabbingModule == eventData.module) {
      Release(eventData.module);
    }
  }

  void IPointerTriggerPressDownHandler.OnPointerTriggerPressDown(PointerEventData eventData) {
    //Only "grab" the object if it's within the bounds of the object.
    //If the object has already been grabbed, ignore this event call.
    if (grabbingModule == null && pointerGrab) {
      //Check for a GlobalGrabber if this object should expect one.
      if (!expectGrabber || (expectGrabber && eventData.module.GetComponent<Grabber>() != null
        && eventData.module.GetComponent<Grabber>().isActiveAndEnabled)) {
        Grab(eventData.module);
      }
    }
  }

  void IPointerTriggerPressHandler.OnPointerTriggerPress(PointerEventData eventData) {
    //Only accept this call if it's from the module currently grabbing this object.
    if (grabbingModule == eventData.module) {
      //Check for a GlobalGrabber if this object should expect one.
      if (!expectGrabber) {
        Hold(eventData.module);
      } else if (expectGrabber) {
        Grabber grabber = eventData.module.GetComponent<Grabber>();
        if (grabber != null && grabber.isActiveAndEnabled) {
          Hold(eventData.module);
        } else {
          Release(eventData.module);
        }
      }
    }
  }

  void IPointerTriggerPressUpHandler.OnPointerTriggerPressUp(PointerEventData eventData) {
    //If the grabbing module releases it's trigger, unbind it from this object.
    if (grabbingModule == eventData.module) {
      Release(eventData.module);
    }
  }
}
