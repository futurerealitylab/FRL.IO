using UnityEngine;
using System.Collections;
using FRL.IO;
using System.Collections.Generic;
using System;

public class GripComponentToggler : MonoBehaviour, IGlobalGripPressDownHandler {

  public List<MonoBehaviour> behaviours = new List<MonoBehaviour>();
  private int currentComponentIndex = 0;

  void Start() {
    EnableBehaviour(currentComponentIndex);
  }

  private void EnableBehaviour(int index) {
    for (int i = 0; i < behaviours.Count; i++) {
      if (i == index) {
        behaviours[i].enabled = true;
      } else {
        behaviours[i].enabled = false;
      }
    }
  }

  void IGlobalGripPressDownHandler.OnGlobalGripPressDown(ViveControllerModule.EventData eventData) {
    currentComponentIndex = (currentComponentIndex + 1) % behaviours.Count;
    EnableBehaviour(currentComponentIndex);
  }
}
