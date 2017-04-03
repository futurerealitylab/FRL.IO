

/// <summary>
/// DIRECTLY USED FROM UNITY-TECHNOLOGIES SOURCE CODE.
/// </summary>
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
  public interface IPointerAppMenuHandler : IPointerAppMenuPressDownHandler, IPointerAppMenuPressHandler, IPointerAppMenuPressUpHandler { }

  public interface IPointerAppMenuPressDownHandler : IEventSystemHandler {
    void OnPointerAppMenuPressDown(VREventData eventData);
  }

  public interface IPointerAppMenuPressHandler : IEventSystemHandler {
    void OnPointerAppMenuPress(VREventData eventData);
  }

  public interface IPointerAppMenuPressUpHandler : IEventSystemHandler {
    void OnPointerAppMenuPressUp(VREventData eventData);
  }

  //GRIP HANDLER
  public interface IPointerGripHandler : IPointerGripPressDownHandler, IPointerGripPressHandler, IPointerGripPressUpHandler { }

  public interface IPointerGripPressDownHandler : IEventSystemHandler {
    void OnPointerGripPressDown(VREventData eventData);
  }

  public interface IPointerGripPressHandler : IEventSystemHandler {
    void OnPointerGripPress(VREventData eventData);
  }
  public interface IPointerGripPressUpHandler : IEventSystemHandler {
    void OnPointerGripPressUp(VREventData eventData);
  }

  //TOUCHPAD HANDLER
  public interface IPointerTouchpadHandler : IPointerTouchpadPressSetHandler, IPointerTouchpadTouchSetHandler { }
  public interface IPointerTouchpadPressSetHandler : IPointerTouchpadPressDownHandler, IPointerTouchpadPressHandler, IPointerTouchpadPressUpHandler { }
  public interface IPointerTouchpadTouchSetHandler : IPointerTouchpadTouchDownHandler, IPointerTouchpadTouchHandler, IPointerTouchpadTouchUpHandler { }

  public interface IPointerTouchpadPressDownHandler : IEventSystemHandler {
    void OnPointerTouchpadPressDown(VREventData eventData);
  }

  public interface IPointerTouchpadPressHandler : IEventSystemHandler {
    void OnPointerTouchpadPress(VREventData eventData);
  }

  public interface IPointerTouchpadPressUpHandler : IEventSystemHandler {
    void OnPointerTouchpadPressUp(VREventData eventData);
  }

  public interface IPointerTouchpadTouchDownHandler : IEventSystemHandler {
    void OnPointerTouchpadTouchDown(VREventData eventData);
  }

  public interface IPointerTouchpadTouchHandler : IEventSystemHandler {
    void OnPointerTouchpadTouch(VREventData eventData);
  }

  public interface IPointerTouchpadTouchUpHandler : IEventSystemHandler {
    void OnPointerTouchpadTouchUp(VREventData eventData);
  }

  //TRIGGER HANDLER
  public interface IPointerTriggerHandler : IPointerTriggerPressSetHandler, IPointerTriggerTouchSetHandler { }
  public interface IPointerTriggerPressSetHandler : IPointerTriggerPressDownHandler, IPointerTriggerPressHandler, IPointerTriggerPressUpHandler { }
  public interface IPointerTriggerTouchSetHandler : IPointerTriggerTouchDownHandler, IPointerTriggerTouchHandler, IPointerTriggerTouchUpHandler { }

  public interface IPointerTriggerPressDownHandler : IEventSystemHandler {
    void OnPointerTriggerPressDown(VREventData eventData);
  }

  public interface IPointerTriggerPressHandler : IEventSystemHandler {
    void OnPointerTriggerPress(VREventData eventData);
  }

  public interface IPointerTriggerPressUpHandler : IEventSystemHandler {
    void OnPointerTriggerPressUp(VREventData eventData);
  }

  public interface IPointerTriggerTouchDownHandler : IEventSystemHandler {
    void OnPointerTriggerTouchDown(VREventData eventData);
  }

  public interface IPointerTriggerTouchHandler : IEventSystemHandler {
    void OnPointerTriggerTouch(VREventData eventData);
  }

  public interface IPointerTriggerTouchUpHandler : IEventSystemHandler {
    void OnPointerTriggerTouchUp(VREventData eventData);
  }

  public interface IPointerTriggerClickHandler : IEventSystemHandler {
    void OnPointerTriggerClick(VREventData eventData);
  }


  /// <summary>
  /// GLOBAL HANDLERS
  /// </summary>

  /// GLOBAL GRIP HANDLER
  public interface IGlobalGripHandler : IGlobalGripPressDownHandler, IGlobalGripPressHandler, IGlobalGripPressUpHandler { }

  public interface IGlobalGripPressDownHandler : IEventSystemHandler {
    void OnGlobalGripPressDown(VREventData eventData);
  }

  public interface IGlobalGripPressHandler : IEventSystemHandler {
    void OnGlobalGripPress(VREventData eventData);
  }

  public interface IGlobalGripPressUpHandler : IEventSystemHandler {
    void OnGlobalGripPressUp(VREventData eventData);
  }

  //GLOBAL TRIGGER HANDLER
  public interface IGlobalTriggerHandler : IGlobalTriggerPressSetHandler, IGlobalTriggerTouchSetHandler { }
  public interface IGlobalTriggerPressSetHandler : IGlobalTriggerPressDownHandler, IGlobalTriggerPressHandler, IGlobalTriggerPressUpHandler { }
  public interface IGlobalTriggerTouchSetHandler : IGlobalTriggerTouchDownHandler, IGlobalTriggerTouchHandler, IGlobalTriggerTouchUpHandler { }

  public interface IGlobalTriggerPressDownHandler : IEventSystemHandler {
    void OnGlobalTriggerPressDown(VREventData eventData);
  }

  public interface IGlobalTriggerPressHandler : IEventSystemHandler {
    void OnGlobalTriggerPress(VREventData eventData);
  }

  public interface IGlobalTriggerPressUpHandler : IEventSystemHandler {
    void OnGlobalTriggerPressUp(VREventData eventData);
  }

  public interface IGlobalTriggerTouchDownHandler : IEventSystemHandler {
    void OnGlobalTriggerTouchDown(VREventData eventData);
  }

  public interface IGlobalTriggerTouchHandler : IEventSystemHandler {
    void OnGlobalTriggerTouch(VREventData eventData);
  }

  public interface IGlobalTriggerTouchUpHandler : IEventSystemHandler {
    void OnGlobalTriggerTouchUp(VREventData eventData);
  }

  public interface IGlobalTriggerClickHandler : IEventSystemHandler {
    void OnGlobalTriggerClick(VREventData eventData);
  }

  //GLOBAL APPLICATION MENU
  public interface IGlobalApplicationMenuHandler : IGlobalApplicationMenuPressDownHandler, IGlobalApplicationMenuPressHandler, IGlobalApplicationMenuPressUpHandler { }

  public interface IGlobalApplicationMenuPressDownHandler : IEventSystemHandler {
    void OnGlobalApplicationMenuPressDown(VREventData eventData);
  }

  public interface IGlobalApplicationMenuPressHandler : IEventSystemHandler {
    void OnGlobalApplicationMenuPress(VREventData eventData);
  }

  public interface IGlobalApplicationMenuPressUpHandler : IEventSystemHandler {
    void OnGlobalApplicationMenuPressUp(VREventData eventData);
  }

  //GLOBAL TOUCHPAD 
  public interface IGlobalTouchpadHandler : IGlobalTouchpadPressSetHandler, IGlobalTouchpadTouchSetHandler { }

  public interface IGlobalTouchpadPressSetHandler : IGlobalTouchpadPressDownHandler, IGlobalTouchpadPressHandler, IGlobalTouchpadPressUpHandler { }
  public interface IGlobalTouchpadTouchSetHandler : IGlobalTouchpadTouchDownHandler, IGlobalTouchpadTouchHandler, IGlobalTouchpadTouchUpHandler { }

  public interface IGlobalTouchpadPressDownHandler : IEventSystemHandler {
    void OnGlobalTouchpadPressDown(VREventData eventData);
  }

  public interface IGlobalTouchpadPressHandler : IEventSystemHandler {
    void OnGlobalTouchpadPress(VREventData eventData);
  }

  public interface IGlobalTouchpadPressUpHandler : IEventSystemHandler {
    void OnGlobalTouchpadPressUp(VREventData eventData);
  }

  public interface IGlobalTouchpadTouchDownHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouchDown(VREventData eventData);
  }

  public interface IGlobalTouchpadTouchHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouch(VREventData eventData);
  }

  public interface IGlobalTouchpadTouchUpHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouchUp(VREventData eventData);
  }

  public interface IGlobalPressDownHandler : IGlobalApplicationMenuPressDownHandler, IGlobalGripPressDownHandler, IGlobalTouchpadPressDownHandler, IGlobalTriggerPressDownHandler { }
}