using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  [RequireComponent(typeof(Receiver))]
  public class AllIOTestObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerStayHandler,
    IPointerAppMenuHandler, IPointerGripHandler, IPointerTouchpadHandler, IPointerTriggerHandler, IGlobalGripHandler,
    IGlobalTriggerHandler, IGlobalAppMenuHandler, IGlobalTouchpadHandler, IPointerAHandler, IGlobalAHandler, IPointerBHandler,
    IGlobalBHandler, IPointerXHandler, IGlobalXHandler, IPointerYHandler, IGlobalYHandler, IPointerThumbstickHandler, IGlobalThumbstickHandler {

    void IGlobalAppMenuPressHandler.OnGlobalAppMenuPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAppMenuPress from " + eventData.module.name);
    }

    void IGlobalAppMenuPressDownHandler.OnGlobalAppMenuPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAppMenuPressDown from " + eventData.module.name);
    }

    void IGlobalAppMenuPressUpHandler.OnGlobalAppMenuPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAppMenuPressUp from " + eventData.module.name);
    }

    void IGlobalAPressHandler.OnGlobalAPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAPress from " + eventData.module.name);
    }

    void IGlobalAPressDownHandler.OnGlobalAPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAPressDown from " + eventData.module.name);
    }

    void IGlobalAPressUpHandler.OnGlobalAPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAPressUp from " + eventData.module.name);
    }

    void IGlobalATouchHandler.OnGlobalATouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalATouch from " + eventData.module.name);
    }

    void IGlobalATouchDownHandler.OnGlobalATouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalATouchDown from " + eventData.module.name);
    }

    void IGlobalATouchUpHandler.OnGlobalATouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalATouchUp from " + eventData.module.name);
    }

    void IGlobalBPressHandler.OnGlobalBPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBPress from " + eventData.module.name);
    }

    void IGlobalBPressDownHandler.OnGlobalBPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBPressDown from " + eventData.module.name);
    }

    void IGlobalBPressUpHandler.OnGlobalBPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBPressUp from " + eventData.module.name);
    }

    void IGlobalBTouchHandler.OnGlobalBTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBTouch from " + eventData.module.name);
    }

    void IGlobalBTouchDownHandler.OnGlobalBTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBTouchDown from " + eventData.module.name);
    }

    void IGlobalBTouchUpHandler.OnGlobalBTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBTouchUp from " + eventData.module.name);
    }

    void IGlobalGripClickHandler.OnGlobalGripClick(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripClick from " + eventData.module.name);
    }

    void IGlobalGripPressHandler.OnGlobalGripPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripPress from " + eventData.module.name);
    }

    void IGlobalGripPressDownHandler.OnGlobalGripPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripPressDown from " + eventData.module.name);
    }

    void IGlobalGripPressUpHandler.OnGlobalGripPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripPressUp from " + eventData.module.name);
    }

    void IGlobalGripTouchHandler.OnGlobalGripTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripTouch from " + eventData.module.name);
    }

    void IGlobalGripTouchDownHandler.OnGlobalGripTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripTouchDown from " + eventData.module.name);
    }

    void IGlobalGripTouchUpHandler.OnGlobalGripTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripTouchUp from " + eventData.module.name);
    }

    void IGlobalThumbstickPressHandler.OnGlobalThumbstickPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickPress from " + eventData.module.name);
    }

    void IGlobalThumbstickPressDownHandler.OnGlobalThumbstickPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickPressDown from " + eventData.module.name);
    }

    void IGlobalThumbstickPressUpHandler.OnGlobalThumbstickPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickPressUp from " + eventData.module.name);
    }

    void IGlobalThumbstickTouchHandler.OnGlobalThumbstickTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickTouch from " + eventData.module.name);
    }

    void IGlobalThumbstickTouchDownHandler.OnGlobalThumbstickTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickTouchDown from " + eventData.module.name);
    }

    void IGlobalThumbstickTouchUpHandler.OnGlobalThumbstickTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickTouchUp from " + eventData.module.name);
    }

    void IGlobalTouchpadPressHandler.OnGlobalTouchpadPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadPress from " + eventData.module.name);
    }

    void IGlobalTouchpadPressDownHandler.OnGlobalTouchpadPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadPressDown from " + eventData.module.name);
    }

    void IGlobalTouchpadPressUpHandler.OnGlobalTouchpadPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadPressUp from " + eventData.module.name);
    }

    void IGlobalTouchpadTouchHandler.OnGlobalTouchpadTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadTouch from " + eventData.module.name);
    }

    void IGlobalTouchpadTouchDownHandler.OnGlobalTouchpadTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadTouchDown from " + eventData.module.name);
    }

    void IGlobalTouchpadTouchUpHandler.OnGlobalTouchpadTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadTouchUp from " + eventData.module.name);
    }

    void IGlobalTriggerPressHandler.OnGlobalTriggerPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerPress from " + eventData.module.name);
    }

    void IGlobalTriggerPressDownHandler.OnGlobalTriggerPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerPressDown from " + eventData.module.name);
    }

    void IGlobalTriggerPressUpHandler.OnGlobalTriggerPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerPressUp from " + eventData.module.name);
    }

    void IGlobalTriggerTouchHandler.OnGlobalTriggerTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerTouch from " + eventData.module.name);
    }

    void IGlobalTriggerTouchDownHandler.OnGlobalTriggerTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerTouchDown from " + eventData.module.name);
    }

    void IGlobalTriggerTouchUpHandler.OnGlobalTriggerTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerTouchUp from " + eventData.module.name);
    }

    void IGlobalXPressHandler.OnGlobalXPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXPress from " + eventData.module.name);
    }

    void IGlobalXPressDownHandler.OnGlobalXPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXPressDown from " + eventData.module.name);
    }

    void IGlobalXPressUpHandler.OnGlobalXPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXPressUp from " + eventData.module.name);
    }

    void IGlobalXTouchHandler.OnGlobalXTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXTouch from " + eventData.module.name);
    }

    void IGlobalXTouchDownHandler.OnGlobalXTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXTouchDown from " + eventData.module.name);
    }

    void IGlobalXTouchUpHandler.OnGlobalXTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXTouchUp from " + eventData.module.name);
    }

    void IGlobalYPressHandler.OnGlobalYPress(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYPress from " + eventData.module.name);
    }

    void IGlobalYPressDownHandler.OnGlobalYPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYPressDown from " + eventData.module.name);
    }

    void IGlobalYPressUpHandler.OnGlobalYPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYPressUp from " + eventData.module.name);
    }

    void IGlobalYTouchHandler.OnGlobalYTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYTouch from " + eventData.module.name);
    }

    void IGlobalYTouchDownHandler.OnGlobalYTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYTouchDown from " + eventData.module.name);
    }

    void IGlobalYTouchUpHandler.OnGlobalYTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYTouchUp from " + eventData.module.name);
    }

    void IPointerAppMenuPressHandler.OnPointerAppMenuPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerAppMenuPress from " + eventData.module.name);
    }

    void IPointerAppMenuPressDownHandler.OnPointerAppMenuPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerAppMenuPressDown from " + eventData.module.name);
    }

    void IPointerAppMenuPressUpHandler.OnPointerAppMenuPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerAppMenuPressUp from " + eventData.module.name);
    }

    void IPointerAPressHandler.OnPointerAPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerAPress from " + eventData.module.name);
    }

    void IPointerAPressDownHandler.OnPointerAPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerAPressDown from " + eventData.module.name);
    }

    void IPointerAPressUpHandler.OnPointerAPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerAPressUp from " + eventData.module.name);
    }

    void IPointerATouchHandler.OnPointerATouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerATouch from " + eventData.module.name);
    }

    void IPointerATouchDownHandler.OnPointerATouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerATouchDown from " + eventData.module.name);
    }

    void IPointerATouchUpHandler.OnPointerATouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerATouchUp from " + eventData.module.name);
    }

    void IPointerBPressHandler.OnPointerBPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerBPress from " + eventData.module.name);
    }

    void IPointerBPressDownHandler.OnPointerBPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerBPressDown from " + eventData.module.name);
    }

    void IPointerBPressUpHandler.OnPointerBPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerBPressUp from " + eventData.module.name);
    }

    void IPointerBTouchHandler.OnPointerBTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerBTouch from " + eventData.module.name);
    }

    void IPointerBTouchDownHandler.OnPointerBTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerBTouchDown from " + eventData.module.name);
    }

    void IPointerBTouchUpHandler.OnPointerBTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerBTouchUp from " + eventData.module.name);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
      Debug.Log(this.name + " received OnPointerEnter " + eventData.module.name);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
      Debug.Log(this.name + " received OnPointerExit from " + eventData.module.name);
    }

    void IPointerGripClickHandler.OnPointerGripClick(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripClick from " + eventData.module.name);
    }

    void IPointerGripPressHandler.OnPointerGripPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripPress from " + eventData.module.name);
    }

    void IPointerGripPressDownHandler.OnPointerGripPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripPressDown from " + eventData.module.name);
    }

    void IPointerGripPressUpHandler.OnPointerGripPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripPressUp from " + eventData.module.name);
    }

    void IPointerGripTouchHandler.OnPointerGripTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripTouch from " + eventData.module.name);
    }

    void IPointerGripTouchDownHandler.OnPointerGripTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripTouchDown from " + eventData.module.name);
    }

    void IPointerGripTouchUpHandler.OnPointerGripTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripTouchUp from " + eventData.module.name);
    }

    void IPointerStayHandler.OnPointerStay(PointerEventData eventData) {
      Debug.Log(this.name + " received OnPointerStay from " + eventData.module.name);
    }

    void IPointerThumbstickPressHandler.OnPointerThumbstickPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickPress from " + eventData.module.name);
    }

    void IPointerThumbstickPressDownHandler.OnPointerThumbstickPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickPressDown from " + eventData.module.name);
    }

    void IPointerThumbstickPressUpHandler.OnPointerThumbstickPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickPressUp from " + eventData.module.name);
    }

    void IPointerThumbstickTouchHandler.OnPointerThumbstickTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickTouch from " + eventData.module.name);
    }

    void IPointerThumbstickTouchDownHandler.OnPointerThumbstickTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickTouchDown from " + eventData.module.name);
    }

    void IPointerThumbstickTouchUpHandler.OnPointerThumbstickTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickTouchUp from " + eventData.module.name);
    }

    void IPointerTouchpadPressHandler.OnPointerTouchpadPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadPress from " + eventData.module.name);
    }

    void IPointerTouchpadPressDownHandler.OnPointerTouchpadPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadPressDown from " + eventData.module.name);
    }

    void IPointerTouchpadPressUpHandler.OnPointerTouchpadPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadPressUp from " + eventData.module.name);
    }

    void IPointerTouchpadTouchHandler.OnPointerTouchpadTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadTouch from " + eventData.module.name);
    }

    void IPointerTouchpadTouchDownHandler.OnPointerTouchpadTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadTouchDown from " + eventData.module.name);
    }

    void IPointerTouchpadTouchUpHandler.OnPointerTouchpadTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadTouchUp from " + eventData.module.name);
    }

    void IPointerTriggerPressHandler.OnPointerTriggerPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerPress from " + eventData.module.name);
    }

    void IPointerTriggerPressDownHandler.OnPointerTriggerPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerPressDown from " + eventData.module.name);
    }

    void IPointerTriggerPressUpHandler.OnPointerTriggerPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerPressUp from " + eventData.module.name);
    }

    void IPointerTriggerTouchHandler.OnPointerTriggerTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerTouch from " + eventData.module.name);
    }

    void IPointerTriggerTouchDownHandler.OnPointerTriggerTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerTouchDown from " + eventData.module.name);
    }

    void IPointerTriggerTouchUpHandler.OnPointerTriggerTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerTouchUp from " + eventData.module.name);
    }

    void IPointerXPressHandler.OnPointerXPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerXPress from " + eventData.module.name);
    }

    void IPointerXPressDownHandler.OnPointerXPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerXPressDown from " + eventData.module.name);
    }

    void IPointerXPressUpHandler.OnPointerXPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerXPressUp from " + eventData.module.name);
    }

    void IPointerXTouchHandler.OnPointerXTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerXTouch from " + eventData.module.name);
    }

    void IPointerXTouchDownHandler.OnPointerXTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerXTouchDown from " + eventData.module.name);
    }

    void IPointerXTouchUpHandler.OnPointerXTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerXTouchUp from " + eventData.module.name);
    }

    void IPointerYPressHandler.OnPointerYPress(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerYPress from " + eventData.module.name);
    }

    void IPointerYPressDownHandler.OnPointerYPressDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerYPressDown from " + eventData.module.name);
    }

    void IPointerYPressUpHandler.OnPointerYPressUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerYPressUp from " + eventData.module.name);
    }

    void IPointerYTouchHandler.OnPointerYTouch(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerYTouch from " + eventData.module.name);
    }

    void IPointerYTouchDownHandler.OnPointerYTouchDown(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerYTouchDown from " + eventData.module.name);
    }

    void IPointerYTouchUpHandler.OnPointerYTouchUp(VREventData eventData) {
      Debug.Log(this.name + " received OnPointerYTouchUp from " + eventData.module.name);
    }
  }
}
