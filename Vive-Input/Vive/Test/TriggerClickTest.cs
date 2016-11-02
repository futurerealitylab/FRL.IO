using UnityEngine;
using System.Collections;
using FRL.IO;
using System;

public class TriggerClickTest : MonoBehaviour,IPointerTriggerClickHandler, IGlobalTriggerClickHandler {

  public void OnGlobalTriggerClick(ViveControllerModule.EventData eventData) {
    Debug.Log("Global Trigger Click from module: " + eventData.viveControllerModule.name);
  }

  void IPointerTriggerClickHandler.OnPointerTriggerClick(ViveControllerModule.EventData eventData) {
    Debug.Log("Pointer Trigger Click from module: " + eventData.viveControllerModule.name);
  }
}
