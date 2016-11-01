using UnityEngine;
using System.Collections;
using FRL.IO;
using System;

[RequireComponent(typeof(GlobalReceiver))]
[RequireComponent(typeof(Collider))]
public class MovableFurniture : MonoBehaviour, IGlobalTriggerPressSetHandler {//, IGlobalTouchpadTouchSetHandler {

  private static float rotationWeight = 360f;

  private Collider collider;
  private GlobalReceiver receiver;

  private BaseInputModule movingModule;
  private Vector3 movingOffset;
  private Vector2 prevTouch;

  void Awake() {
    collider = this.GetComponent<Collider>();
    receiver = this.GetComponent<GlobalReceiver>();
  }


  void IGlobalTriggerPressHandler.OnGlobalTriggerPress(ViveControllerModule.EventData eventData) {
    if (eventData.module == movingModule) {
      //Vector3 rcPos = transform.position + transform.rotation * Vector3.up;
      //RaycastHit hit;
      //Ray ray = new Ray(rcPos, transform.up * -1f);
      //if (Physics.Raycast(ray, out hit, 1f)) {
      //  transform.position = hit.point;
      // transform.up = hit.normal;
      //  movingOffset = transform.position - movingModule.transform.position;
      //} else {
        transform.position = movingModule.transform.position + movingOffset;
      //}
    }
  }

  void IGlobalTriggerPressDownHandler.OnGlobalTriggerPressDown(ViveControllerModule.EventData eventData) {
    if (movingModule == null && collider.bounds.Contains(eventData.module.transform.position)) {
      Debug.Log(eventData.module.name + " is in the bounds of object: " + this.name);
      movingModule = eventData.module;
      movingOffset = transform.position - movingModule.transform.position;
    }
  }

  void IGlobalTriggerPressUpHandler.OnGlobalTriggerPressUp(ViveControllerModule.EventData eventData) {
    if (movingModule == eventData.module) {
      movingModule = null;
      movingOffset = Vector3.zero;
    }
  }

  //void IGlobalTouchpadTouchHandler.OnGlobalTouchpadTouch(ViveControllerModule.EventData eventData) {
  //  if (movingModule != null && eventData.module == movingModule && !prevTouch.Equals(Vector2.zero)) {
  //    this.transform.Rotate(new Vector3(0f, (eventData.touchpadAxis.x - prevTouch.x) * Time.deltaTime * rotationWeight,0f),Space.World);
  //    prevTouch = eventData.touchpadAxis;
  //  }
  //}

  //void IGlobalTouchpadTouchDownHandler.OnGlobalTouchpadTouchDown(ViveControllerModule.EventData eventData) {
  //  if (movingModule != null && eventData.module == movingModule) {
  //    prevTouch = eventData.touchpadAxis;
  //  }
  //}

  //void IGlobalTouchpadTouchUpHandler.OnGlobalTouchpadTouchUp(ViveControllerModule.EventData eventData) {
  //  if (movingModule != null && eventData.module == movingModule) {
  //    prevTouch = Vector2.zero;
  //  }
  //}
}
