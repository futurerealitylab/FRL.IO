//using UnityEngine;
//using System.Collections;
//using FRL.IO;
//using System;

////Require a Collider component on this gameObject.
//[RequireComponent(typeof(Collider))]
//public class PointerGrabbable : MonoBehaviour, IPointerTriggerPressSetHandler {



//  private Vector3 hitOffset = Vector3.zero;
//  private Vector3 moduleOffset = Vector3.zero;
//  private Quaternion deltaRotation = Quaternion.identity;

//  private ViveControllerModule grabbingModule;
  

//  void Awake() {

//  }

//  /// <summary>
//  /// This function is called when the trigger is initially pressed. Called once per press context.
//  /// </summary>
//  /// <param name="eventData">The corresponding event data for the module.</param>
//  public void OnPointerTriggerPressDown(ViveControllerModule.EventData eventData) {
//    //If the object has already been grabbed, ignore this event call.
//    if (grabbingModule == null) {
//      //Bind the module to this object.
//      grabbingModule = eventData.module;
//      //Save the offset between the module and this object. Undo the current rotation of the module
//      hitOffset = this.transform.position - eventData.worldPosition;
//      hitOffset = Quaternion.Inverse(this.transform.rotation) * hitOffset;

//      moduleOffset = eventData.worldPosition - grabbingModule.transform.position;
//      moduleOffset = Quaternion.Inverse(grabbingModule.transform.rotation) * moduleOffset;

//      deltaRotation = Quaternion.Inverse(grabbingModule.transform.rotation) * this.transform.rotation;
//    }
//  }

//  /// <summary>
//  /// This function is called every frame between the initial press and release of the trigger.
//  /// </summary>
//  /// <param name="eventData">The corresponding event data for the module.</param>
//  public void OnPointerTriggerPress(ViveControllerModule.EventData eventData) {
//    //Only accept this call if it's from the module currently grabbing this object.
//    if (grabbingModule == eventData.module) {
//      this.transform.position = grabbingModule.transform.position + grabbingModule.transform.rotation * moduleOffset;
//      this.transform.rotation = grabbingModule.transform.rotation * deltaRotation;
//      this.transform.position += this.transform.rotation * hitOffset;
//    }
//  }


//  /// <summary>
//  /// This function is called when the trigger is released. Called once per press context.
//  /// </summary>
//  /// <param name="eventData">The corresponding event data for the module.</param>
//  public void OnPointerTriggerPressUp(ViveControllerModule.EventData eventData) {
//    //If the grabbing module releases it's trigger, unbind it from this object.
//    if (grabbingModule == eventData.module) {
//      hitOffset = Vector3.zero;
//      grabbingModule = null;
//    }
//  }
//}
