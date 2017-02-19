using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.IO;
using System;

[RequireComponent(typeof(Receiver))]
[RequireComponent(typeof(Collider))]
public class Holdable : MonoBehaviour, IGlobalPressDownHandler {


  public ButtonType button;
  public bool expectHolder = true;

  private new Collider collider;
  private Rigidbody rbody;

  private BaseInputModule holdingModule;

  private void Awake() {
    collider = this.GetComponent<Collider>();
    rbody = this.GetComponent<Rigidbody>();
  }

  private void Update() {
    if (holdingModule) {
      transform.position = holdingModule.transform.position;
      transform.rotation = holdingModule.transform.rotation;
    }
  }

  private void OnDisable() {
    if (holdingModule != null) {
      ToggleHold(holdingModule);
    }
  }


  private void ToggleHold(BaseInputModule module) {
    if (holdingModule) {
      //Release
      if (rbody) {
        rbody.isKinematic = false;
      }
      holdingModule = null;
      collider.isTrigger = false;
    } else {
      //Bind
      holdingModule = module;
      collider.isTrigger = true;
      if (rbody) {
        rbody.isKinematic = true;
      }
    }
  }

  private void TryHold(BaseInputModule module, ButtonType b) {
    //Only try to hold object if it's within the bounds of the collider.
    //If the object is already being held, ignore this event call.
    if (collider.bounds.Contains(module.transform.position) && holdingModule == null
        && button == b) {
      //Check for a Holder if one is expected.
      if (!expectHolder || (expectHolder && module.GetComponent<Holder>() != null
          && module.GetComponent<Holder>().isActiveAndEnabled)) {
        ToggleHold(module);
      }
    }
  }

  public void OnGlobalApplicationMenuPressDown(BaseEventData eventData) {
    TryHold(eventData.module, ButtonType.AppMenu);
  }

  public void OnGlobalGripPressDown(BaseEventData eventData) {
    TryHold(eventData.module, ButtonType.Grip);
  }

  public void OnGlobalTouchpadPressDown(BaseEventData eventData) {
    TryHold(eventData.module, ButtonType.TouchpadPress);
  }

  public void OnGlobalTriggerPressDown(BaseEventData eventData) {
    TryHold(eventData.module, ButtonType.TriggerPress);
  }
}
