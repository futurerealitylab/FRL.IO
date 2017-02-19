using UnityEngine;
using System.Collections.Generic;
using System;

namespace FRL.IO {
  public class BehaviourToggler : MonoBehaviour, IGlobalTriggerPressDownHandler, IGlobalApplicationMenuPressDownHandler, 
    IGlobalGripPressDownHandler, IGlobalTouchpadPressDownHandler, IGlobalTriggerTouchDownHandler, IGlobalTouchpadTouchDownHandler {

    public ButtonType button = ButtonType.Grip;
    public List<MonoBehaviour> behaviours = new List<MonoBehaviour>();
    private int currentIndex = 0;

    void Toggle() {
      currentIndex = (currentIndex + 1) % behaviours.Count;
      for (int i = 0; i < behaviours.Count; i++) {
        if (i == currentIndex) {
          behaviours[i].enabled = true;
        } else {
          behaviours[i].enabled = false;
        }
      }
    }

    void IGlobalApplicationMenuPressDownHandler.OnGlobalApplicationMenuPressDown(BaseEventData eventData) {
      if (button == ButtonType.AppMenu)
        Toggle();
    }

    void IGlobalGripPressDownHandler.OnGlobalGripPressDown(BaseEventData eventData) {
      if (button == ButtonType.Grip)
        Toggle();
    }

    void IGlobalTouchpadPressDownHandler.OnGlobalTouchpadPressDown(BaseEventData eventData) {
      if (button == ButtonType.TouchpadPress)
        Toggle();
    }

    void IGlobalTouchpadTouchDownHandler.OnGlobalTouchpadTouchDown(BaseEventData eventData) {
      if (button == ButtonType.TouchpadTouch)
        Toggle();
    }

    void IGlobalTriggerPressDownHandler.OnGlobalTriggerPressDown(BaseEventData eventData) {
      if (button == ButtonType.TriggerPress)
        Toggle();
    }

    void IGlobalTriggerTouchDownHandler.OnGlobalTriggerTouchDown(BaseEventData eventData) {
      if (button == ButtonType.TriggerTouch)
        Toggle();
    }

  }
}

