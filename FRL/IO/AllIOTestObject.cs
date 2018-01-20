using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FRL.IO {
  [RequireComponent(typeof(Receiver))]
  public class AllIOTestObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerStayHandler,
    IPointerMenuHandler, IPointerGripHandler, IPointerTouchpadHandler, IPointerTriggerHandler, IGlobalGripHandler,
    IGlobalTriggerHandler, IGlobalMenuHandler, IGlobalTouchpadHandler, IPointerAHandler, IGlobalAHandler, IPointerBHandler,
    IGlobalBHandler, IPointerXHandler, IGlobalXHandler, IPointerYHandler, IGlobalYHandler, IPointerThumbstickHandler, IGlobalThumbstickHandler {

    private bool pStay, pMenuPress, pGripPress, pTouchpadPress, pTriggerPress, pAPress, pBPress, pXPress, pYPress, pThumbstickPress;
    private bool gStay, gMenuPress, gGripPress, gTouchpadPress, gTriggerPress, gAPress, gBPress, gXPress, gYPress, gThumbstickPress;
    private bool pMenuTouch, pGripTouch, pTouchpadTouch, pTriggerTouch, pATouch, pBTouch, pXTouch, pYTouch, pThumbstickTouch;
    private bool gMenuTouch, gGripTouch, gTouchpadTouch, gTriggerTouch, gATouch, gBTouch, gXTouch, gYTouch, gThumbstickTouch;

    public Text uiText;

    void Update() {
      string text = "AllIOTestObject:\n";
      if (pMenuPress) text += "\tpMenuPress";
      if (pGripPress) text += "\tpGripPress";
      if (pTouchpadPress) text += "\tpTouchpadPress";
      if (pTriggerPress) text += "\tpTriggerPress";
      text += "\n";
      if (pAPress) text += "\tpAPress";
      if (pBPress) text += "\tpBPress";
      if (pXPress) text += "\tpXPress";
      if (pYPress) text += "\tpYPress";
      if (pThumbstickPress) text += "\tpThumbstickPress";
      text += "\n";
      if (gMenuPress) text += "\tgMenuPress";
      if (gGripPress) text += "\tgGripPress";
      if (gTouchpadPress) text += "\tgTouchpadPress";
      if (gTriggerPress) text += "\tgTriggerPress";
      text += "\n";
      if (gAPress) text += "\tgAPress";
      if (gBPress) text += "\tgBPress";
      if (gXPress) text += "\tgXPress";
      if (gYPress) text += "\tgYPress";
      if (gThumbstickPress) text += "\tgThumbstickPress";
      text += "\n";
      if (pMenuTouch) text += "\tpMenuTouch";
      if (pGripTouch) text += "\tpGripTouch";
      if (pTouchpadTouch) text += "\tpTouchpadTouch";
      if (pTriggerTouch) text += "\tpTriggerTouch";
      text += "\n";
      if (pATouch) text += "\tpATouch";
      if (pBTouch) text += "\tpBTouch";
      if (pXTouch) text += "\tpXTouch";
      if (pYTouch) text += "\tpYTouch";
      if (pThumbstickTouch) text += "\tpThumbstickTouch";
      text += "\n";
      if (gMenuTouch) text += "\tgMenuTouch";
      if (gGripTouch) text += "\tgGripTouch";
      if (gTouchpadTouch) text += "\tgTouchpadTouch";
      if (gTriggerTouch) text += "\tgTriggerTouch";
      text += "\n";
      if (gATouch) text += "\tgATouch";
      if (gBTouch) text += "\tgBTouch";
      if (gXTouch) text += "\tgXTouch";
      if (gYTouch) text += "\tgYTouch";
      if (gThumbstickTouch) text += "\tgThumbstickTouch";
      text += "\n";

      uiText.text = text;
    } 


    void IGlobalMenuPressHandler.OnGlobalMenuPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalMenuPress from " + eventData.module.name);
    }

    void IGlobalMenuPressDownHandler.OnGlobalMenuPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalMenuPressDown from " + eventData.module.name);
      gMenuPress = true;
    }

    void IGlobalMenuPressUpHandler.OnGlobalMenuPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalMenuPressUp from " + eventData.module.name);
      gMenuPress = false;
    }

    void IGlobalAPressHandler.OnGlobalAPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAPress from " + eventData.module.name);
    }

    void IGlobalAPressDownHandler.OnGlobalAPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAPressDown from " + eventData.module.name);
      gAPress = true;
    }

    void IGlobalAPressUpHandler.OnGlobalAPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalAPressUp from " + eventData.module.name);
      gAPress = false;
    }

    void IGlobalATouchHandler.OnGlobalATouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalATouch from " + eventData.module.name);
    }

    void IGlobalATouchDownHandler.OnGlobalATouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalATouchDown from " + eventData.module.name);
      gATouch = true;
    }

    void IGlobalATouchUpHandler.OnGlobalATouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalATouchUp from " + eventData.module.name);
      gATouch = false;
    }

    void IGlobalBPressHandler.OnGlobalBPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBPress from " + eventData.module.name);
    }

    void IGlobalBPressDownHandler.OnGlobalBPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBPressDown from " + eventData.module.name);
      gBPress = true;
    }

    void IGlobalBPressUpHandler.OnGlobalBPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBPressUp from " + eventData.module.name);
      gBPress = false;
    }

    void IGlobalBTouchHandler.OnGlobalBTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBTouch from " + eventData.module.name);
    }

    void IGlobalBTouchDownHandler.OnGlobalBTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBTouchDown from " + eventData.module.name);
      gBTouch = true;
    }

    void IGlobalBTouchUpHandler.OnGlobalBTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalBTouchUp from " + eventData.module.name);
      gBTouch = false;
    }

    void IGlobalGripClickHandler.OnGlobalGripClick(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripClick from " + eventData.module.name);
    }

    void IGlobalGripPressHandler.OnGlobalGripPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripPress from " + eventData.module.name);
    }

    void IGlobalGripPressDownHandler.OnGlobalGripPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripPressDown from " + eventData.module.name);
      gGripPress = true;
    }

    void IGlobalGripPressUpHandler.OnGlobalGripPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripPressUp from " + eventData.module.name);
      gGripPress = false;
    }

    void IGlobalGripTouchHandler.OnGlobalGripTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripTouch from " + eventData.module.name);
    }

    void IGlobalGripTouchDownHandler.OnGlobalGripTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripTouchDown from " + eventData.module.name);
      gGripTouch = true;
    }

    void IGlobalGripTouchUpHandler.OnGlobalGripTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalGripTouchUp from " + eventData.module.name);
      gGripTouch = false;
    }

    void IGlobalThumbstickPressHandler.OnGlobalThumbstickPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickPress from " + eventData.module.name);
    }

    void IGlobalThumbstickPressDownHandler.OnGlobalThumbstickPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickPressDown from " + eventData.module.name);
      gThumbstickPress = true;
    }

    void IGlobalThumbstickPressUpHandler.OnGlobalThumbstickPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickPressUp from " + eventData.module.name);
      gThumbstickPress = false;
    }

    void IGlobalThumbstickTouchHandler.OnGlobalThumbstickTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickTouch from " + eventData.module.name);
    }

    void IGlobalThumbstickTouchDownHandler.OnGlobalThumbstickTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickTouchDown from " + eventData.module.name);
      gThumbstickTouch = true;
    }

    void IGlobalThumbstickTouchUpHandler.OnGlobalThumbstickTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalThumbstickTouchUp from " + eventData.module.name);
      gThumbstickTouch = false;
    }

    void IGlobalTouchpadPressHandler.OnGlobalTouchpadPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadPress from " + eventData.module.name);
    }

    void IGlobalTouchpadPressDownHandler.OnGlobalTouchpadPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadPressDown from " + eventData.module.name);
      gTouchpadPress = true;
    }

    void IGlobalTouchpadPressUpHandler.OnGlobalTouchpadPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadPressUp from " + eventData.module.name);
      gTouchpadPress = false;
    }

    void IGlobalTouchpadTouchHandler.OnGlobalTouchpadTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadTouch from " + eventData.module.name);
    }

    void IGlobalTouchpadTouchDownHandler.OnGlobalTouchpadTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadTouchDown from " + eventData.module.name);
      gTouchpadTouch = true;
    }

    void IGlobalTouchpadTouchUpHandler.OnGlobalTouchpadTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTouchpadTouchUp from " + eventData.module.name);
      gTouchpadTouch = false;
    }

    void IGlobalTriggerPressHandler.OnGlobalTriggerPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerPress from " + eventData.module.name);
    }

    void IGlobalTriggerPressDownHandler.OnGlobalTriggerPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerPressDown from " + eventData.module.name);
      gTriggerPress = true;
    }

    void IGlobalTriggerPressUpHandler.OnGlobalTriggerPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerPressUp from " + eventData.module.name);
      gTriggerPress = false;
    }

    void IGlobalTriggerTouchHandler.OnGlobalTriggerTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerTouch from " + eventData.module.name);
    }

    void IGlobalTriggerTouchDownHandler.OnGlobalTriggerTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerTouchDown from " + eventData.module.name);
      gTriggerTouch = true;
    }

    void IGlobalTriggerTouchUpHandler.OnGlobalTriggerTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalTriggerTouchUp from " + eventData.module.name);
      gTriggerTouch = false;
    }

    void IGlobalXPressHandler.OnGlobalXPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXPress from " + eventData.module.name);
    }

    void IGlobalXPressDownHandler.OnGlobalXPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXPressDown from " + eventData.module.name);
      gXPress = true;
    }

    void IGlobalXPressUpHandler.OnGlobalXPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXPressUp from " + eventData.module.name);
      gXPress = false;
    }

    void IGlobalXTouchHandler.OnGlobalXTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXTouch from " + eventData.module.name);
    }

    void IGlobalXTouchDownHandler.OnGlobalXTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXTouchDown from " + eventData.module.name);
      gXTouch = true;
    }

    void IGlobalXTouchUpHandler.OnGlobalXTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalXTouchUp from " + eventData.module.name);
      gXTouch = false;
    }

    void IGlobalYPressHandler.OnGlobalYPress(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYPress from " + eventData.module.name);
    }

    void IGlobalYPressDownHandler.OnGlobalYPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYPressDown from " + eventData.module.name);
      gYPress = true;
    }

    void IGlobalYPressUpHandler.OnGlobalYPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYPressUp from " + eventData.module.name);
      gYPress = false;
    }

    void IGlobalYTouchHandler.OnGlobalYTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYTouch from " + eventData.module.name);
    }

    void IGlobalYTouchDownHandler.OnGlobalYTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYTouchDown from " + eventData.module.name);
      gYTouch = true;
    }

    void IGlobalYTouchUpHandler.OnGlobalYTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalYTouchUp from " + eventData.module.name);
      gYTouch = false;
    }

    void IPointerMenuPressHandler.OnPointerMenuPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerMenuPress from " + eventData.module.name);
    }

    void IPointerMenuPressDownHandler.OnPointerMenuPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerMenuPressDown from " + eventData.module.name);
      pMenuPress = true;
    }

    void IPointerMenuPressUpHandler.OnPointerMenuPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerMenuPressUp from " + eventData.module.name);
      pMenuPress = false;
    }

    void IPointerAPressHandler.OnPointerAPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerAPress from " + eventData.module.name);
    }

    void IPointerAPressDownHandler.OnPointerAPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerAPressDown from " + eventData.module.name);
      pAPress = true;
    }

    void IPointerAPressUpHandler.OnPointerAPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerAPressUp from " + eventData.module.name);
      pAPress = false;
    }

    void IPointerATouchHandler.OnPointerATouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerATouch from " + eventData.module.name);
    }

    void IPointerATouchDownHandler.OnPointerATouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerATouchDown from " + eventData.module.name);
      pATouch = true;
    }

    void IPointerATouchUpHandler.OnPointerATouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerATouchUp from " + eventData.module.name);
      pATouch = false;
    }

    void IPointerBPressHandler.OnPointerBPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerBPress from " + eventData.module.name);
    }

    void IPointerBPressDownHandler.OnPointerBPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerBPressDown from " + eventData.module.name);
      pBPress = true;
    }

    void IPointerBPressUpHandler.OnPointerBPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerBPressUp from " + eventData.module.name);
      pBPress = false;
    }

    void IPointerBTouchHandler.OnPointerBTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerBTouch from " + eventData.module.name);
    }

    void IPointerBTouchDownHandler.OnPointerBTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerBTouchDown from " + eventData.module.name);
      pBTouch = true;
    }

    void IPointerBTouchUpHandler.OnPointerBTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerBTouchUp from " + eventData.module.name);
      pBTouch = false;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
      Debug.Log(this.name + " received OnPointerEnter " + eventData.module.name);
      pStay = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
      Debug.Log(this.name + " received OnPointerExit from " + eventData.module.name);
      pStay = false;
    }

    void IPointerGripClickHandler.OnPointerGripClick(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripClick from " + eventData.module.name);
    }

    void IPointerGripPressHandler.OnPointerGripPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripPress from " + eventData.module.name);
    }

    void IPointerGripPressDownHandler.OnPointerGripPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripPressDown from " + eventData.module.name);
      pGripPress = true;
    }

    void IPointerGripPressUpHandler.OnPointerGripPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripPressUp from " + eventData.module.name);
      pGripPress = false;
    }

    void IPointerGripTouchHandler.OnPointerGripTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripTouch from " + eventData.module.name);
    }

    void IPointerGripTouchDownHandler.OnPointerGripTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripTouchDown from " + eventData.module.name);
      pGripTouch = true;
    }

    void IPointerGripTouchUpHandler.OnPointerGripTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerGripTouchUp from " + eventData.module.name);
      pGripTouch = false;
    }

    void IPointerStayHandler.OnPointerStay(PointerEventData eventData) {
      Debug.Log(this.name + " received OnPointerStay from " + eventData.module.name);
    }

    void IPointerThumbstickPressHandler.OnPointerThumbstickPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickPress from " + eventData.module.name);
    }

    void IPointerThumbstickPressDownHandler.OnPointerThumbstickPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickPressDown from " + eventData.module.name);
      pThumbstickPress = true;
    }

    void IPointerThumbstickPressUpHandler.OnPointerThumbstickPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickPressUp from " + eventData.module.name);
      pThumbstickPress = false;
    }

    void IPointerThumbstickTouchHandler.OnPointerThumbstickTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickTouch from " + eventData.module.name);
    }

    void IPointerThumbstickTouchDownHandler.OnPointerThumbstickTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickTouchDown from " + eventData.module.name);
      pThumbstickTouch = true;
    }

    void IPointerThumbstickTouchUpHandler.OnPointerThumbstickTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerThumbstickTouchUp from " + eventData.module.name);
      pThumbstickTouch = false;
    }

    void IPointerTouchpadPressHandler.OnPointerTouchpadPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadPress from " + eventData.module.name);
    }

    void IPointerTouchpadPressDownHandler.OnPointerTouchpadPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadPressDown from " + eventData.module.name);
      pTouchpadPress = true;
    }

    void IPointerTouchpadPressUpHandler.OnPointerTouchpadPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadPressUp from " + eventData.module.name);
      pTouchpadPress = false;
    }

    void IPointerTouchpadTouchHandler.OnPointerTouchpadTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadTouch from " + eventData.module.name);
    }

    void IPointerTouchpadTouchDownHandler.OnPointerTouchpadTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadTouchDown from " + eventData.module.name);
      pTouchpadTouch = true;
    }

    void IPointerTouchpadTouchUpHandler.OnPointerTouchpadTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTouchpadTouchUp from " + eventData.module.name);
      pTouchpadTouch = false;
    }

    void IPointerTriggerPressHandler.OnPointerTriggerPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerPress from " + eventData.module.name);
    }

    void IPointerTriggerPressDownHandler.OnPointerTriggerPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerPressDown from " + eventData.module.name);
      pTriggerPress = true;
    }

    void IPointerTriggerPressUpHandler.OnPointerTriggerPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerPressUp from " + eventData.module.name);
      pTriggerPress = false;
    }

    void IPointerTriggerTouchHandler.OnPointerTriggerTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerTouch from " + eventData.module.name);
    }

    void IPointerTriggerTouchDownHandler.OnPointerTriggerTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerTouchDown from " + eventData.module.name);
      pTriggerTouch = true;
    }

    void IPointerTriggerTouchUpHandler.OnPointerTriggerTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerTriggerTouchUp from " + eventData.module.name);
      pTriggerTouch = false;
    }

    void IPointerXPressHandler.OnPointerXPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerXPress from " + eventData.module.name);
    }

    void IPointerXPressDownHandler.OnPointerXPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerXPressDown from " + eventData.module.name);
      pXPress = true;
    }

    void IPointerXPressUpHandler.OnPointerXPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerXPressUp from " + eventData.module.name);
      pXPress = false;
    }

    void IPointerXTouchHandler.OnPointerXTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerXTouch from " + eventData.module.name);
    }

    void IPointerXTouchDownHandler.OnPointerXTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerXTouchDown from " + eventData.module.name);
      pXTouch = true;
    }

    void IPointerXTouchUpHandler.OnPointerXTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerXTouchUp from " + eventData.module.name);
      pXTouch = false;
    }

    void IPointerYPressHandler.OnPointerYPress(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerYPress from " + eventData.module.name);
    }

    void IPointerYPressDownHandler.OnPointerYPressDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerYPressDown from " + eventData.module.name);
      pYPress = true;
    }

    void IPointerYPressUpHandler.OnPointerYPressUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerYPressUp from " + eventData.module.name);
      pYPress = false;
    }

    void IPointerYTouchHandler.OnPointerYTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerYTouch from " + eventData.module.name);
    }

    void IPointerYTouchDownHandler.OnPointerYTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerYTouchDown from " + eventData.module.name);
      pYTouch = true;
    }

    void IPointerYTouchUpHandler.OnPointerYTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerYTouchUp from " + eventData.module.name);
      pYTouch = false;
    }

    void IPointerMenuTouchDownHandler.OnPointerMenuTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerMenuTouchDown from " + eventData.module.name);
    }

    void IPointerMenuTouchHandler.OnPointerMenuTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerMenuTouch from " + eventData.module.name);
    }

    void IPointerMenuTouchUpHandler.OnPointerMenuTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerMenuTouchUp from " + eventData.module.name);
    }

    void IPointerMenuClickHandler.OnPointerMenuClick(XREventData eventData) {
      Debug.Log(this.name + " received OnPointerMenuTouchClick from " + eventData.module.name);
    }

    void IGlobalMenuTouchDownHandler.OnGlobalMenuTouchDown(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalMenuTouchDown from " + eventData.module.name);
    }

    void IGlobalMenuTouchHandler.OnGlobalMenuTouch(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalMenuTouch from " + eventData.module.name);

    }

    void IGlobalMenuTouchUpHandler.OnGlobalMenuTouchUp(XREventData eventData) {
      Debug.Log(this.name + " received OnGlobalMenuTouchUp from " + eventData.module.name);
    }
  }
}
