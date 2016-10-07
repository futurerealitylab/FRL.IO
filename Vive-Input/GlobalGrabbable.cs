using UnityEngine;
using System.Collections;
using FRL.IO;

//Require a GlobalReceiver component and a Collider component on this gameObject.
[RequireComponent(typeof(GlobalReceiver))]
[RequireComponent(typeof(Collider))]
public class GlobalGrabbable : MonoBehaviour,IGlobalTriggerPressSetHandler {

  private GlobalReceiver receiver;

  private Collider collider;

  private Vector3 offset = Vector3.zero;

  private ViveControllerModule grabbingModule;

  void Awake() {
    //Get the GlobalReceiver component on this gameObject.
    receiver = this.GetComponent<GlobalReceiver>();
    //Get the Collider component on this gameObject.
    collider = this.GetComponent<Collider>();
  }

  /// <summary>
  /// This function is called when the trigger is initially pressed. Called once per press context.
  /// </summary>
  /// <param name="eventData">The corresponding event data for the module.</param>
  public void OnGlobalTriggerPressDown(ViveControllerModule.EventData eventData) {
    //Only "grab" the object if it's within the bounds of the object.
    //If the object has already been grabbed, ignore this event call.
    if (collider.bounds.Contains(eventData.module.transform.position) && grabbingModule == null) {
      //Bind the module to this object.
      grabbingModule = eventData.module;
      //Save the offset between the module and this object. Undo the current rotation of the module
      offset = transform.position = grabbingModule.transform.position;
      offset = Quaternion.Inverse(grabbingModule.transform.rotation) * offset;
    }
  }

  /// <summary>
  /// This function is called every frame between the initial press and release of the trigger.
  /// </summary>
  /// <param name="eventData">The corresponding event data for the module.</param>
  public void OnGlobalTriggerPress(ViveControllerModule.EventData eventData) {
    //Only accept this call if it's from the module currently grabbing this object.
    if (grabbingModule == eventData.module) {
      this.transform.position = grabbingModule.transform.position + grabbingModule.transform.rotation * offset;
    }
  }


  /// <summary>
  /// This function is called when the trigger is released. Called once per press context.
  /// </summary>
  /// <param name="eventData">The corresponding event data for the module.</param>
  public void OnGlobalTriggerPressUp(ViveControllerModule.EventData eventData) {
    //If the grabbing module releases it's trigger, unbind it from this object.
    if (grabbingModule == eventData.module) {
      offset = Vector3.zero;
      grabbingModule = null;
    }
  }
}
