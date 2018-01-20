namespace FRL.IO {
  public interface IEventSystemHandler {
  }

  public interface IPointerEnterHandler : IEventSystemHandler { void OnPointerEnter(PointerEventData eventData); }

  public interface IPointerExitHandler : IEventSystemHandler {
    void OnPointerExit(PointerEventData eventData);
  }

  public interface IPointerStayHandler : IEventSystemHandler {
    void OnPointerStay(PointerEventData eventData);
  }

  public interface IPointerDownHandler : IEventSystemHandler {
    void OnPointerDown(PointerEventData eventData);
  }

  public interface IPointerUpHandler : IEventSystemHandler {
    void OnPointerUp(PointerEventData eventData);
  }

  public interface IPointerClickHandler : IEventSystemHandler {
    void OnPointerClick(PointerEventData eventData);
  }

  public interface IBeginDragHandler : IEventSystemHandler {
    void OnBeginDrag(PointerEventData eventData);
  }

  public interface IInitializePotentialDragHandler : IEventSystemHandler {
    void OnInitializePotentialDrag(PointerEventData eventData);
  }

  public interface IDragHandler : IEventSystemHandler {
    void OnDrag(PointerEventData eventData);
  }

  public interface IEndDragHandler : IEventSystemHandler {
    void OnEndDrag(PointerEventData eventData);
  }

  public interface IDropHandler : IEventSystemHandler {
    void OnDrop(PointerEventData eventData);
  }

  public interface IScrollHandler : IEventSystemHandler {
    void OnScroll(PointerEventData eventData);
  }

  public interface IUpdateSelectedHandler : IEventSystemHandler {
    void OnUpdateSelected(BaseEventData eventData);
  }

  public interface ISelectHandler : IEventSystemHandler {
    void OnSelect(BaseEventData eventData);
  }

  public interface IDeselectHandler : IEventSystemHandler {
    void OnDeselect(BaseEventData eventData);
  }

  public interface ISubmitHandler : IEventSystemHandler {
    void OnSubmit(BaseEventData eventData);
  }

  public interface ICancelHandler : IEventSystemHandler {
    void OnCancel(BaseEventData eventData);
  }

  /// <summary>
  /// POINTER HANDLERS
  /// </summary>

  //APPLICATION MENU HANDLER
  public interface IPointerMenuHandler : IPointerMenuPressSetHandler, IPointerMenuTouchSetHandler, IPointerMenuClickHandler { }
  public interface IPointerMenuPressSetHandler : IPointerMenuPressDownHandler, IPointerMenuPressHandler, IPointerMenuPressUpHandler { }
  public interface IPointerMenuTouchSetHandler : IPointerMenuTouchDownHandler, IPointerMenuTouchHandler, IPointerMenuTouchUpHandler { }

  public interface IPointerMenuPressDownHandler : IEventSystemHandler {
    void OnPointerMenuPressDown(XREventData eventData);
  }

  public interface IPointerMenuPressHandler : IEventSystemHandler {
    void OnPointerMenuPress(XREventData eventData);
  }
  public interface IPointerMenuPressUpHandler : IEventSystemHandler {
    void OnPointerMenuPressUp(XREventData eventData);
  }

  public interface IPointerMenuTouchDownHandler : IEventSystemHandler {
    void OnPointerMenuTouchDown(XREventData eventData);
  }

  public interface IPointerMenuTouchHandler : IEventSystemHandler {
    void OnPointerMenuTouch(XREventData eventData);
  }
  public interface IPointerMenuTouchUpHandler : IEventSystemHandler {
    void OnPointerMenuTouchUp(XREventData eventData);
  }

  public interface IPointerMenuClickHandler : IEventSystemHandler {
    void OnPointerMenuClick(XREventData eventData);
  }

  //GRIP HANDLER
  public interface IPointerGripHandler : IPointerGripPressSetHandler, IPointerGripTouchSetHandler, IPointerGripClickHandler { }
  public interface IPointerGripPressSetHandler : IPointerGripPressDownHandler, IPointerGripPressHandler, IPointerGripPressUpHandler { }
  public interface IPointerGripTouchSetHandler : IPointerGripTouchDownHandler, IPointerGripTouchHandler, IPointerGripTouchUpHandler { }

  public interface IPointerGripPressDownHandler : IEventSystemHandler {
    void OnPointerGripPressDown(XREventData eventData);
  }

  public interface IPointerGripPressHandler : IEventSystemHandler {
    void OnPointerGripPress(XREventData eventData);
  }
  public interface IPointerGripPressUpHandler : IEventSystemHandler {
    void OnPointerGripPressUp(XREventData eventData);
  }

  public interface IPointerGripTouchDownHandler : IEventSystemHandler {
    void OnPointerGripTouchDown(XREventData eventData);
  }

  public interface IPointerGripTouchHandler : IEventSystemHandler {
    void OnPointerGripTouch(XREventData eventData);
  }
  public interface IPointerGripTouchUpHandler : IEventSystemHandler {
    void OnPointerGripTouchUp(XREventData eventData);
  }

  public interface IPointerGripClickHandler : IEventSystemHandler {
    void OnPointerGripClick(XREventData eventData);
  }

  //TOUCHPAD HANDLER
  public interface IPointerTouchpadHandler : IPointerTouchpadPressSetHandler, IPointerTouchpadTouchSetHandler { }
  public interface IPointerTouchpadPressSetHandler : IPointerTouchpadPressDownHandler, IPointerTouchpadPressHandler, IPointerTouchpadPressUpHandler { }
  public interface IPointerTouchpadTouchSetHandler : IPointerTouchpadTouchDownHandler, IPointerTouchpadTouchHandler, IPointerTouchpadTouchUpHandler { }

  public interface IPointerTouchpadPressDownHandler : IEventSystemHandler {
    void OnPointerTouchpadPressDown(XREventData eventData);
  }

  public interface IPointerTouchpadPressHandler : IEventSystemHandler {
    void OnPointerTouchpadPress(XREventData eventData);
  }

  public interface IPointerTouchpadPressUpHandler : IEventSystemHandler {
    void OnPointerTouchpadPressUp(XREventData eventData);
  }

  public interface IPointerTouchpadTouchDownHandler : IEventSystemHandler {
    void OnPointerTouchpadTouchDown(XREventData eventData);
  }

  public interface IPointerTouchpadTouchHandler : IEventSystemHandler {
    void OnPointerTouchpadTouch(XREventData eventData);
  }

  public interface IPointerTouchpadTouchUpHandler : IEventSystemHandler {
    void OnPointerTouchpadTouchUp(XREventData eventData);
  }

  //TRIGGER HANDLER
  public interface IPointerTriggerHandler : IPointerTriggerPressSetHandler, IPointerTriggerTouchSetHandler { }
  public interface IPointerTriggerPressSetHandler : IPointerTriggerPressDownHandler, IPointerTriggerPressHandler, IPointerTriggerPressUpHandler { }
  public interface IPointerTriggerTouchSetHandler : IPointerTriggerTouchDownHandler, IPointerTriggerTouchHandler, IPointerTriggerTouchUpHandler { }

  public interface IPointerTriggerPressDownHandler : IEventSystemHandler {
    void OnPointerTriggerPressDown(XREventData eventData);
  }

  public interface IPointerTriggerPressHandler : IEventSystemHandler {
    void OnPointerTriggerPress(XREventData eventData);
  }

  public interface IPointerTriggerPressUpHandler : IEventSystemHandler {
    void OnPointerTriggerPressUp(XREventData eventData);
  }

  public interface IPointerTriggerTouchDownHandler : IEventSystemHandler {
    void OnPointerTriggerTouchDown(XREventData eventData);
  }

  public interface IPointerTriggerTouchHandler : IEventSystemHandler {
    void OnPointerTriggerTouch(XREventData eventData);
  }

  public interface IPointerTriggerTouchUpHandler : IEventSystemHandler {
    void OnPointerTriggerTouchUp(XREventData eventData);
  }

  public interface IPointerTriggerClickHandler : IEventSystemHandler {
    void OnPointerTriggerClick(XREventData eventData);
  }


  /// <summary>
  /// GLOBAL HANDLERS
  /// </summary>

  /// GLOBAL GRIP HANDLER
  public interface IGlobalGripHandler : IGlobalGripPressSetHandler, IGlobalGripTouchSetHandler, IGlobalGripClickHandler { }
  public interface IGlobalGripPressSetHandler : IGlobalGripPressDownHandler, IGlobalGripPressHandler, IGlobalGripPressUpHandler { }
  public interface IGlobalGripTouchSetHandler : IGlobalGripTouchDownHandler, IGlobalGripTouchHandler, IGlobalGripTouchUpHandler { }

  public interface IGlobalGripPressDownHandler : IEventSystemHandler {
    void OnGlobalGripPressDown(XREventData eventData);
  }

  public interface IGlobalGripPressHandler : IEventSystemHandler {
    void OnGlobalGripPress(XREventData eventData);
  }

  public interface IGlobalGripPressUpHandler : IEventSystemHandler {
    void OnGlobalGripPressUp(XREventData eventData);
  }

  public interface IGlobalGripTouchDownHandler : IEventSystemHandler {
    void OnGlobalGripTouchDown(XREventData eventData);
  }

  public interface IGlobalGripTouchHandler : IEventSystemHandler {
    void OnGlobalGripTouch(XREventData eventData);
  }

  public interface IGlobalGripTouchUpHandler : IEventSystemHandler {
    void OnGlobalGripTouchUp(XREventData eventData);
  }

  public interface IGlobalGripClickHandler : IEventSystemHandler {
    void OnGlobalGripClick(XREventData eventData);
  }

  //GLOBAL TRIGGER HANDLER
  public interface IGlobalTriggerHandler : IGlobalTriggerPressSetHandler, IGlobalTriggerTouchSetHandler { }
  public interface IGlobalTriggerPressSetHandler : IGlobalTriggerPressDownHandler, IGlobalTriggerPressHandler, IGlobalTriggerPressUpHandler { }
  public interface IGlobalTriggerTouchSetHandler : IGlobalTriggerTouchDownHandler, IGlobalTriggerTouchHandler, IGlobalTriggerTouchUpHandler { }

  public interface IGlobalTriggerPressDownHandler : IEventSystemHandler {
    void OnGlobalTriggerPressDown(XREventData eventData);
  }

  public interface IGlobalTriggerPressHandler : IEventSystemHandler {
    void OnGlobalTriggerPress(XREventData eventData);
  }

  public interface IGlobalTriggerPressUpHandler : IEventSystemHandler {
    void OnGlobalTriggerPressUp(XREventData eventData);
  }

  public interface IGlobalTriggerTouchDownHandler : IEventSystemHandler {
    void OnGlobalTriggerTouchDown(XREventData eventData);
  }

  public interface IGlobalTriggerTouchHandler : IEventSystemHandler {
    void OnGlobalTriggerTouch(XREventData eventData);
  }

  public interface IGlobalTriggerTouchUpHandler : IEventSystemHandler {
    void OnGlobalTriggerTouchUp(XREventData eventData);
  }

  public interface IGlobalTriggerClickHandler : IEventSystemHandler {
    void OnGlobalTriggerClick(XREventData eventData);
  }

  //GLOBAL APPLICATION MENU
  public interface IGlobalMenuHandler : IGlobalMenuPressSetHandler, IGlobalMenuTouchSetHandler { }
  public interface IGlobalMenuPressSetHandler : IGlobalMenuPressDownHandler, IGlobalMenuPressHandler, IGlobalMenuPressUpHandler { }
  public interface IGlobalMenuTouchSetHandler : IGlobalMenuTouchDownHandler, IGlobalMenuTouchHandler, IGlobalMenuTouchUpHandler { }

  public interface IGlobalMenuPressDownHandler : IEventSystemHandler {
    void OnGlobalMenuPressDown(XREventData eventData);
  }

  public interface IGlobalMenuPressHandler : IEventSystemHandler {
    void OnGlobalMenuPress(XREventData eventData);
  }

  public interface IGlobalMenuPressUpHandler : IEventSystemHandler {
    void OnGlobalMenuPressUp(XREventData eventData);
  }

  public interface IGlobalMenuTouchDownHandler : IEventSystemHandler {
    void OnGlobalMenuTouchDown(XREventData eventData);
  }

  public interface IGlobalMenuTouchHandler : IEventSystemHandler {
    void OnGlobalMenuTouch(XREventData eventData);
  }

  public interface IGlobalMenuTouchUpHandler : IEventSystemHandler {
    void OnGlobalMenuTouchUp(XREventData eventData);
  }


  //GLOBAL TOUCHPAD 
  public interface IGlobalTouchpadHandler : IGlobalTouchpadPressSetHandler, IGlobalTouchpadTouchSetHandler { }
  public interface IGlobalTouchpadPressSetHandler : IGlobalTouchpadPressDownHandler, IGlobalTouchpadPressHandler, IGlobalTouchpadPressUpHandler { }
  public interface IGlobalTouchpadTouchSetHandler : IGlobalTouchpadTouchDownHandler, IGlobalTouchpadTouchHandler, IGlobalTouchpadTouchUpHandler { }

  public interface IGlobalTouchpadPressDownHandler : IEventSystemHandler {
    void OnGlobalTouchpadPressDown(XREventData eventData);
  }

  public interface IGlobalTouchpadPressHandler : IEventSystemHandler {
    void OnGlobalTouchpadPress(XREventData eventData);
  }

  public interface IGlobalTouchpadPressUpHandler : IEventSystemHandler {
    void OnGlobalTouchpadPressUp(XREventData eventData);
  }

  public interface IGlobalTouchpadTouchDownHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouchDown(XREventData eventData);
  }

  public interface IGlobalTouchpadTouchHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouch(XREventData eventData);
  }

  public interface IGlobalTouchpadTouchUpHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouchUp(XREventData eventData);
  }

  /// <summary>
  /// A BUTTON
  /// </summary>

  public interface IPointerAHandler : IPointerAPressSetHandler, IPointerATouchSetHandler { }
  public interface IPointerAPressSetHandler : IPointerAPressDownHandler, IPointerAPressHandler, IPointerAPressUpHandler { }
  public interface IPointerATouchSetHandler : IPointerATouchDownHandler, IPointerATouchHandler, IPointerATouchUpHandler { }

  public interface IPointerAPressDownHandler : IEventSystemHandler {
    void OnPointerAPressDown(XREventData eventData);
  }

  public interface IPointerAPressHandler : IEventSystemHandler {
    void OnPointerAPress(XREventData eventData);
  }

  public interface IPointerAPressUpHandler : IEventSystemHandler {
    void OnPointerAPressUp(XREventData eventData);
  }

  public interface IPointerATouchDownHandler : IEventSystemHandler {
    void OnPointerATouchDown(XREventData eventData);
  }

  public interface IPointerATouchHandler : IEventSystemHandler {
    void OnPointerATouch(XREventData eventData);
  }

  public interface IPointerATouchUpHandler : IEventSystemHandler {
    void OnPointerATouchUp(XREventData eventData);
  }


  public interface IGlobalAHandler : IGlobalAPressSetHandler, IGlobalATouchSetHandler { }
  public interface IGlobalAPressSetHandler : IGlobalAPressDownHandler, IGlobalAPressHandler, IGlobalAPressUpHandler { }
  public interface IGlobalATouchSetHandler : IGlobalATouchDownHandler, IGlobalATouchHandler, IGlobalATouchUpHandler { }

  public interface IGlobalAPressDownHandler : IEventSystemHandler {
    void OnGlobalAPressDown(XREventData eventData);
  }

  public interface IGlobalAPressHandler : IEventSystemHandler {
    void OnGlobalAPress(XREventData eventData);
  }

  public interface IGlobalAPressUpHandler : IEventSystemHandler {
    void OnGlobalAPressUp(XREventData eventData);
  }

  public interface IGlobalATouchDownHandler : IEventSystemHandler {
    void OnGlobalATouchDown(XREventData eventData);
  }

  public interface IGlobalATouchHandler : IEventSystemHandler {
    void OnGlobalATouch(XREventData eventData);
  }

  public interface IGlobalATouchUpHandler : IEventSystemHandler {
    void OnGlobalATouchUp(XREventData eventData);
  }

  /// <summary>
  /// B BUTTON
  /// </summary>

  public interface IPointerBHandler : IPointerBPressSetHandler, IPointerBTouchSetHandler { }
  public interface IPointerBPressSetHandler : IPointerBPressDownHandler, IPointerBPressHandler, IPointerBPressUpHandler { }
  public interface IPointerBTouchSetHandler : IPointerBTouchDownHandler, IPointerBTouchHandler, IPointerBTouchUpHandler { }

  public interface IPointerBPressDownHandler : IEventSystemHandler {
    void OnPointerBPressDown(XREventData eventData);
  }

  public interface IPointerBPressHandler : IEventSystemHandler {
    void OnPointerBPress(XREventData eventData);
  }

  public interface IPointerBPressUpHandler : IEventSystemHandler {
    void OnPointerBPressUp(XREventData eventData);
  }

  public interface IPointerBTouchDownHandler : IEventSystemHandler {
    void OnPointerBTouchDown(XREventData eventData);
  }

  public interface IPointerBTouchHandler : IEventSystemHandler {
    void OnPointerBTouch(XREventData eventData);
  }

  public interface IPointerBTouchUpHandler : IEventSystemHandler {
    void OnPointerBTouchUp(XREventData eventData);
  }

  public interface IGlobalBHandler : IGlobalBPressSetHandler, IGlobalBTouchSetHandler { }
  public interface IGlobalBPressSetHandler : IGlobalBPressDownHandler, IGlobalBPressHandler, IGlobalBPressUpHandler { }
  public interface IGlobalBTouchSetHandler : IGlobalBTouchDownHandler, IGlobalBTouchHandler, IGlobalBTouchUpHandler { }

  public interface IGlobalBPressDownHandler : IEventSystemHandler {
    void OnGlobalBPressDown(XREventData eventData);
  }

  public interface IGlobalBPressHandler : IEventSystemHandler {
    void OnGlobalBPress(XREventData eventData);
  }

  public interface IGlobalBPressUpHandler : IEventSystemHandler {
    void OnGlobalBPressUp(XREventData eventData);
  }

  public interface IGlobalBTouchDownHandler : IEventSystemHandler {
    void OnGlobalBTouchDown(XREventData eventData);
  }

  public interface IGlobalBTouchHandler : IEventSystemHandler {
    void OnGlobalBTouch(XREventData eventData);
  }

  public interface IGlobalBTouchUpHandler : IEventSystemHandler {
    void OnGlobalBTouchUp(XREventData eventData);
  }

  /// <summary>
  /// X BUTTON
  /// </summary>


  public interface IPointerXHandler : IPointerXPressSetHandler, IPointerXTouchSetHandler { }
  public interface IPointerXPressSetHandler : IPointerXPressDownHandler, IPointerXPressHandler, IPointerXPressUpHandler { }
  public interface IPointerXTouchSetHandler : IPointerXTouchDownHandler, IPointerXTouchHandler, IPointerXTouchUpHandler { }

  public interface IPointerXPressDownHandler : IEventSystemHandler {
    void OnPointerXPressDown(XREventData eventData);
  }

  public interface IPointerXPressHandler : IEventSystemHandler {
    void OnPointerXPress(XREventData eventData);
  }

  public interface IPointerXPressUpHandler : IEventSystemHandler {
    void OnPointerXPressUp(XREventData eventData);
  }

  public interface IPointerXTouchDownHandler : IEventSystemHandler {
    void OnPointerXTouchDown(XREventData eventData);
  }

  public interface IPointerXTouchHandler : IEventSystemHandler {
    void OnPointerXTouch(XREventData eventData);
  }

  public interface IPointerXTouchUpHandler : IEventSystemHandler {
    void OnPointerXTouchUp(XREventData eventData);
  }

  public interface IGlobalXHandler : IGlobalXPressSetHandler, IGlobalXTouchSetHandler { }
  public interface IGlobalXPressSetHandler : IGlobalXPressDownHandler, IGlobalXPressHandler, IGlobalXPressUpHandler { }
  public interface IGlobalXTouchSetHandler : IGlobalXTouchDownHandler, IGlobalXTouchHandler, IGlobalXTouchUpHandler { }

  public interface IGlobalXPressDownHandler : IEventSystemHandler {
    void OnGlobalXPressDown(XREventData eventData);
  }

  public interface IGlobalXPressHandler : IEventSystemHandler {
    void OnGlobalXPress(XREventData eventData);
  }

  public interface IGlobalXPressUpHandler : IEventSystemHandler {
    void OnGlobalXPressUp(XREventData eventData);
  }

  public interface IGlobalXTouchDownHandler : IEventSystemHandler {
    void OnGlobalXTouchDown(XREventData eventData);
  }

  public interface IGlobalXTouchHandler : IEventSystemHandler {
    void OnGlobalXTouch(XREventData eventData);
  }

  public interface IGlobalXTouchUpHandler : IEventSystemHandler {
    void OnGlobalXTouchUp(XREventData eventData);
  }


  /// <summary>
  /// Y BUTTON
  /// </summary>

  public interface IPointerYHandler : IPointerYPressSetHandler, IPointerYTouchSetHandler { }
  public interface IPointerYPressSetHandler : IPointerYPressDownHandler, IPointerYPressHandler, IPointerYPressUpHandler { }
  public interface IPointerYTouchSetHandler : IPointerYTouchDownHandler, IPointerYTouchHandler, IPointerYTouchUpHandler { }

  public interface IPointerYPressDownHandler : IEventSystemHandler {
    void OnPointerYPressDown(XREventData eventData);
  }

  public interface IPointerYPressHandler : IEventSystemHandler {
    void OnPointerYPress(XREventData eventData);
  }

  public interface IPointerYPressUpHandler : IEventSystemHandler {
    void OnPointerYPressUp(XREventData eventData);
  }

  public interface IPointerYTouchDownHandler : IEventSystemHandler {
    void OnPointerYTouchDown(XREventData eventData);
  }

  public interface IPointerYTouchHandler : IEventSystemHandler {
    void OnPointerYTouch(XREventData eventData);
  }

  public interface IPointerYTouchUpHandler : IEventSystemHandler {
    void OnPointerYTouchUp(XREventData eventData);
  }

  public interface IGlobalYHandler : IGlobalYPressSetHandler, IGlobalYTouchSetHandler { }
  public interface IGlobalYPressSetHandler : IGlobalYPressDownHandler, IGlobalYPressHandler, IGlobalYPressUpHandler { }
  public interface IGlobalYTouchSetHandler : IGlobalYTouchDownHandler, IGlobalYTouchHandler, IGlobalYTouchUpHandler { }

  public interface IGlobalYPressDownHandler : IEventSystemHandler {
    void OnGlobalYPressDown(XREventData eventData);
  }

  public interface IGlobalYPressHandler : IEventSystemHandler {
    void OnGlobalYPress(XREventData eventData);
  }

  public interface IGlobalYPressUpHandler : IEventSystemHandler {
    void OnGlobalYPressUp(XREventData eventData);
  }

  public interface IGlobalYTouchDownHandler : IEventSystemHandler {
    void OnGlobalYTouchDown(XREventData eventData);
  }

  public interface IGlobalYTouchHandler : IEventSystemHandler {
    void OnGlobalYTouch(XREventData eventData);
  }

  public interface IGlobalYTouchUpHandler : IEventSystemHandler {
    void OnGlobalYTouchUp(XREventData eventData);
  }


  public interface IPointerThumbstickHandler : IPointerThumbstickPressSetHandler, IPointerThumbstickTouchSetHandler { }
  public interface IPointerThumbstickPressSetHandler : IPointerThumbstickPressDownHandler, IPointerThumbstickPressHandler, IPointerThumbstickPressUpHandler { }
  public interface IPointerThumbstickTouchSetHandler : IPointerThumbstickTouchDownHandler, IPointerThumbstickTouchHandler, IPointerThumbstickTouchUpHandler { }

  public interface IPointerThumbstickPressDownHandler : IEventSystemHandler {
    void OnPointerThumbstickPressDown(XREventData eventData);
  }

  public interface IPointerThumbstickPressHandler : IEventSystemHandler {
    void OnPointerThumbstickPress(XREventData eventData);
  }

  public interface IPointerThumbstickPressUpHandler : IEventSystemHandler {
    void OnPointerThumbstickPressUp(XREventData eventData);
  }

  public interface IPointerThumbstickTouchDownHandler : IEventSystemHandler {
    void OnPointerThumbstickTouchDown(XREventData eventData);
  }

  public interface IPointerThumbstickTouchHandler : IEventSystemHandler {
    void OnPointerThumbstickTouch(XREventData eventData);
  }

  public interface IPointerThumbstickTouchUpHandler : IEventSystemHandler {
    void OnPointerThumbstickTouchUp(XREventData eventData);
  }

  public interface IGlobalThumbstickHandler : IGlobalThumbstickPressSetHandler, IGlobalThumbstickTouchSetHandler { }
  public interface IGlobalThumbstickPressSetHandler : IGlobalThumbstickPressDownHandler, IGlobalThumbstickPressHandler, IGlobalThumbstickPressUpHandler { }
  public interface IGlobalThumbstickTouchSetHandler : IGlobalThumbstickTouchDownHandler, IGlobalThumbstickTouchHandler, IGlobalThumbstickTouchUpHandler { }

  public interface IGlobalThumbstickPressDownHandler : IEventSystemHandler {
    void OnGlobalThumbstickPressDown(XREventData eventData);
  }

  public interface IGlobalThumbstickPressHandler : IEventSystemHandler {
    void OnGlobalThumbstickPress(XREventData eventData);
  }

  public interface IGlobalThumbstickPressUpHandler : IEventSystemHandler {
    void OnGlobalThumbstickPressUp(XREventData eventData);
  }

  public interface IGlobalThumbstickTouchDownHandler : IEventSystemHandler {
    void OnGlobalThumbstickTouchDown(XREventData eventData);
  }

  public interface IGlobalThumbstickTouchHandler : IEventSystemHandler {
    void OnGlobalThumbstickTouch(XREventData eventData);
  }

  public interface IGlobalThumbstickTouchUpHandler : IEventSystemHandler {
    void OnGlobalThumbstickTouchUp(XREventData eventData);
  }

  public interface IPointerForwardSetHandler : IPointerForwardDownHandler, IPointerForwardHandler, IPointerForwardUpHandler { }
  public interface IGlobalForwardSetHandler : IGlobalForwardDownHandler, IGlobalForwardHandler, IGlobalForwardUpHandler { }

  public interface IPointerForwardDownHandler : IEventSystemHandler {
    void OnPointerForwardDown(XREventData eventData);
  }

  public interface IPointerForwardHandler : IEventSystemHandler {
    void OnPointerForward(XREventData eventData);
  }

  public interface IPointerForwardUpHandler : IEventSystemHandler {
    void OnPointerForwardUp(XREventData eventData);
  }

  public interface IGlobalForwardDownHandler : IEventSystemHandler {
    void OnGlobalForwardDown(XREventData eventData);
  }

  public interface IGlobalForwardHandler : IEventSystemHandler {
    void OnGlobalForward(XREventData eventData);
  }

  public interface IGlobalForwardUpHandler : IEventSystemHandler {
    void OnGlobalForwardUp(XREventData eventData);
  }

  public interface IPointerBackSetHandler : IPointerBackDownHandler, IPointerBackHandler, IPointerBackUpHandler { }
  public interface IGlobalBackSetHandler : IGlobalBackDownHandler, IGlobalBackHandler, IGlobalBackUpHandler { }

  public interface IPointerBackDownHandler : IEventSystemHandler {
    void OnPointerBackDown(XREventData eventData);
  }

  public interface IPointerBackHandler : IEventSystemHandler {
    void OnPointerBack(XREventData eventData);
  }

  public interface IPointerBackUpHandler : IEventSystemHandler {
    void OnPointerBackUp(XREventData eventData);
  }

  public interface IGlobalBackDownHandler : IEventSystemHandler {
    void OnGlobalBackDown(XREventData eventData);
  }

  public interface IGlobalBackHandler : IEventSystemHandler {
    void OnGlobalBack(XREventData eventData);
  }

  public interface IGlobalBackUpHandler : IEventSystemHandler {
    void OnGlobalBackUp(XREventData eventData);
  }

  public interface IPointerLeftSetHandler : IPointerLeftDownHandler, IPointerLeftHandler, IPointerLeftUpHandler { }
  public interface IGlobalLeftSetHandler : IGlobalLeftDownHandler, IGlobalLeftHandler, IGlobalLeftUpHandler { }

  public interface IPointerLeftDownHandler : IEventSystemHandler {
    void OnPointerLeftDown(XREventData eventData);
  }

  public interface IPointerLeftHandler : IEventSystemHandler {
    void OnPointerLeft(XREventData eventData);
  }

  public interface IPointerLeftUpHandler : IEventSystemHandler {
    void OnPointerLeftUp(XREventData eventData);
  }

  public interface IGlobalLeftDownHandler : IEventSystemHandler {
    void OnGlobalLeftDown(XREventData eventData);
  }

  public interface IGlobalLeftHandler : IEventSystemHandler {
    void OnGlobalLeft(XREventData eventData);
  }

  public interface IGlobalLeftUpHandler : IEventSystemHandler {
    void OnGlobalLeftUp(XREventData eventData);
  }

  public interface IPointerRightSetHandler : IPointerRightDownHandler, IPointerRightHandler, IPointerRightUpHandler { }
  public interface IGlobalRightSetHandler : IGlobalRightDownHandler, IGlobalRightHandler, IGlobalRightUpHandler { }

  public interface IPointerRightDownHandler : IEventSystemHandler {
    void OnPointerRightDown(XREventData eventData);
  }

  public interface IPointerRightHandler : IEventSystemHandler {
    void OnPointerRight(XREventData eventData);
  }

  public interface IPointerRightUpHandler : IEventSystemHandler {
    void OnPointerRightUp(XREventData eventData);
  }

  public interface IGlobalRightDownHandler : IEventSystemHandler {
    void OnGlobalRightDown(XREventData eventData);
  }

  public interface IGlobalRightHandler : IEventSystemHandler {
    void OnGlobalRight(XREventData eventData);
  }

  public interface IGlobalRightUpHandler : IEventSystemHandler {
    void OnGlobalRightUp(XREventData eventData);
  }
}