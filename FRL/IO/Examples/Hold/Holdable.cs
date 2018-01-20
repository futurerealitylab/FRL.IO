using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.IO;
using System;

[RequireComponent(typeof(Receiver))]
[RequireComponent(typeof(Collider))]
public class Holdable : MonoBehaviour, IGlobalGripPressDownHandler, IGlobalTouchpadPressDownHandler, IGlobalTriggerPressDownHandler, IGlobalMenuPressDownHandler {


  public ButtonType button;
  public bool expectHolder = true;

  private new Collider collider;
  private Rigidbody rbody;

  public BaseInputModule holdingModule {
    get; private set;
  }

  public Action<bool> OnToggleHold;

  private void Awake() {
    collider = this.GetComponent<Collider>();
    rbody = this.GetComponent<Rigidbody>();
  }

  private void Update() {
    if (holdingModule) {
      if (rbody) {
        rbody.MovePosition(holdingModule.transform.position);
        rbody.MoveRotation(holdingModule.transform.rotation);
      } else {
        transform.position = holdingModule.transform.position;
        transform.rotation = holdingModule.transform.rotation;
      }
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
        //rbody.isKinematic = false;
      }
      holdingModule = null;
      //collider.isTrigger = false;
      if (OnToggleHold != null)
        OnToggleHold(false);
    } else {
      //Bind
      holdingModule = module;
      //collider.isTrigger = true;
      if (rbody) {
        //rbody.isKinematic = true;
      }
      if (OnToggleHold != null)
        OnToggleHold(true);
    }
  }

  private void TryHold(BaseInputModule module, ButtonType b) {
    //Only try to hold object if it's within the bounds of the collider.
    //If the object is already being held, ignore this event call.
    if (collider.bounds.Contains(module.transform.position) && button == b
      && (holdingModule == null || holdingModule == module)) {
      //Check for a Holder if one is expected.
      if (!expectHolder || (expectHolder && module.GetComponent<Holder>() != null
          && module.GetComponent<Holder>().isActiveAndEnabled)) {
        ToggleHold(module);
      }
    }
  }

  public void OnGlobalMenuPressDown(XREventData eventData) {
    TryHold(eventData.module, ButtonType.Menu);
  }

  public void OnGlobalGripPressDown(XREventData eventData) {
    TryHold(eventData.module, ButtonType.Grip);
  }

  public void OnGlobalTouchpadPressDown(XREventData eventData) {
    TryHold(eventData.module, ButtonType.TouchpadPress);
  }

  public void OnGlobalTriggerPressDown(XREventData eventData) {
    TryHold(eventData.module, ButtonType.TriggerPress);
  }
}
