

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
    void OnPointerAppMenuPressDown(PointerEventData eventData);
  }

  public interface IPointerAppMenuPressHandler : IEventSystemHandler {
    void OnPointerAppMenuPress(PointerEventData eventData);
  }

  public interface IPointerAppMenuPressUpHandler : IEventSystemHandler {
    void OnPointerAppMenuPressUp(PointerEventData eventData);
  }

  //GRIP HANDLER
  public interface IPointerGripHandler : IPointerGripPressDownHandler, IPointerGripPressHandler, IPointerGripPressUpHandler { }

  public interface IPointerGripPressDownHandler : IEventSystemHandler {
    void OnPointerGripPressDown(PointerEventData eventData);
  }

  public interface IPointerGripPressHandler : IEventSystemHandler {
    void OnPointerGripPress(PointerEventData eventData);
  }
  public interface IPointerGripPressUpHandler : IEventSystemHandler {
    void OnPointerGripPressUp(PointerEventData eventData);
  }

  //TOUCHPAD HANDLER
  public interface IPointerTouchpadHandler : IPointerTouchpadPressSetHandler, IPointerTouchpadTouchSetHandler { }
  public interface IPointerTouchpadPressSetHandler : IPointerTouchpadPressDownHandler, IPointerTouchpadPressHandler, IPointerTouchpadPressUpHandler { }
  public interface IPointerTouchpadTouchSetHandler : IPointerTouchpadTouchDownHandler, IPointerTouchpadTouchHandler, IPointerTouchpadTouchUpHandler { }

  public interface IPointerTouchpadPressDownHandler : IEventSystemHandler {
    void OnPointerTouchpadPressDown(PointerEventData eventData);
  }

  public interface IPointerTouchpadPressHandler : IEventSystemHandler {
    void OnPointerTouchpadPress(PointerEventData eventData);
  }

  public interface IPointerTouchpadPressUpHandler : IEventSystemHandler {
    void OnPointerTouchpadPressUp(PointerEventData eventData);
  }

  public interface IPointerTouchpadTouchDownHandler : IEventSystemHandler {
    void OnPointerTouchpadTouchDown(PointerEventData eventData);
  }

  public interface IPointerTouchpadTouchHandler : IEventSystemHandler {
    void OnPointerTouchpadTouch(PointerEventData eventData);
  }

  public interface IPointerTouchpadTouchUpHandler : IEventSystemHandler {
    void OnPointerTouchpadTouchUp(PointerEventData eventData);
  }

  //TRIGGER HANDLER
  public interface IPointerTriggerHandler : IPointerTriggerPressSetHandler, IPointerTriggerTouchSetHandler { }
  public interface IPointerTriggerPressSetHandler : IPointerTriggerPressDownHandler, IPointerTriggerPressHandler, IPointerTriggerPressUpHandler { }
  public interface IPointerTriggerTouchSetHandler : IPointerTriggerTouchDownHandler, IPointerTriggerTouchHandler, IPointerTriggerTouchUpHandler { }

  public interface IPointerTriggerPressDownHandler : IEventSystemHandler {
    void OnPointerTriggerPressDown(PointerEventData eventData);
  }

  public interface IPointerTriggerPressHandler : IEventSystemHandler {
    void OnPointerTriggerPress(PointerEventData eventData);
  }

  public interface IPointerTriggerPressUpHandler : IEventSystemHandler {
    void OnPointerTriggerPressUp(PointerEventData eventData);
  }

  public interface IPointerTriggerTouchDownHandler : IEventSystemHandler {
    void OnPointerTriggerTouchDown(PointerEventData eventData);
  }

  public interface IPointerTriggerTouchHandler : IEventSystemHandler {
    void OnPointerTriggerTouch(PointerEventData eventData);
  }

  public interface IPointerTriggerTouchUpHandler : IEventSystemHandler {
    void OnPointerTriggerTouchUp(PointerEventData eventData);
  }

  public interface IPointerTriggerClickHandler : IEventSystemHandler {
    void OnPointerTriggerClick(PointerEventData eventData);
  }


  /// <summary>
  /// GLOBAL HANDLERS
  /// </summary>

  /// GLOBAL GRIP HANDLER
  public interface IGlobalGripHandler : IGlobalGripPressDownHandler, IGlobalGripPressHandler, IGlobalGripPressUpHandler { }

  public interface IGlobalGripPressDownHandler : IEventSystemHandler {
    void OnGlobalGripPressDown(BaseEventData eventData);
  }

  public interface IGlobalGripPressHandler : IEventSystemHandler {
    void OnGlobalGripPress(BaseEventData eventData);
  }

  public interface IGlobalGripPressUpHandler : IEventSystemHandler {
    void OnGlobalGripPressUp(BaseEventData eventData);
  }

  //GLOBAL TRIGGER HANDLER
  public interface IGlobalTriggerHandler : IGlobalTriggerPressSetHandler, IGlobalTriggerTouchSetHandler { }
  public interface IGlobalTriggerPressSetHandler : IGlobalTriggerPressDownHandler, IGlobalTriggerPressHandler, IGlobalTriggerPressUpHandler { }
  public interface IGlobalTriggerTouchSetHandler : IGlobalTriggerTouchDownHandler, IGlobalTriggerTouchHandler, IGlobalTriggerTouchUpHandler { }

  public interface IGlobalTriggerPressDownHandler : IEventSystemHandler {
    void OnGlobalTriggerPressDown(BaseEventData eventData);
  }

  public interface IGlobalTriggerPressHandler : IEventSystemHandler {
    void OnGlobalTriggerPress(BaseEventData eventData);
  }

  public interface IGlobalTriggerPressUpHandler : IEventSystemHandler {
    void OnGlobalTriggerPressUp(BaseEventData eventData);
  }

  public interface IGlobalTriggerTouchDownHandler : IEventSystemHandler {
    void OnGlobalTriggerTouchDown(BaseEventData eventData);
  }

  public interface IGlobalTriggerTouchHandler : IEventSystemHandler {
    void OnGlobalTriggerTouch(BaseEventData eventData);
  }

  public interface IGlobalTriggerTouchUpHandler : IEventSystemHandler {
    void OnGlobalTriggerTouchUp(BaseEventData eventData);
  }

  public interface IGlobalTriggerClickHandler : IEventSystemHandler {
    void OnGlobalTriggerClick(BaseEventData eventData);
  }

  //GLOBAL APPLICATION MENU
  public interface IGlobalApplicationMenuHandler : IGlobalApplicationMenuPressDownHandler, IGlobalApplicationMenuPressHandler, IGlobalApplicationMenuPressUpHandler { }

  public interface IGlobalApplicationMenuPressDownHandler : IEventSystemHandler {
    void OnGlobalApplicationMenuPressDown(BaseEventData eventData);
  }

  public interface IGlobalApplicationMenuPressHandler : IEventSystemHandler {
    void OnGlobalApplicationMenuPress(BaseEventData eventData);
  }

  public interface IGlobalApplicationMenuPressUpHandler : IEventSystemHandler {
    void OnGlobalApplicationMenuPressUp(BaseEventData eventData);
  }

  //GLOBAL TOUCHPAD 
  public interface IGlobalTouchpadHandler : IGlobalTouchpadPressSetHandler, IGlobalTouchpadTouchSetHandler { }

  public interface IGlobalTouchpadPressSetHandler : IGlobalTouchpadPressDownHandler, IGlobalTouchpadPressHandler, IGlobalTouchpadPressUpHandler { }
  public interface IGlobalTouchpadTouchSetHandler : IGlobalTouchpadTouchDownHandler, IGlobalTouchpadTouchHandler, IGlobalTouchpadTouchUpHandler { }

  public interface IGlobalTouchpadPressDownHandler : IEventSystemHandler {
    void OnGlobalTouchpadPressDown(BaseEventData eventData);
  }

  public interface IGlobalTouchpadPressHandler : IEventSystemHandler {
    void OnGlobalTouchpadPress(BaseEventData eventData);
  }

  public interface IGlobalTouchpadPressUpHandler : IEventSystemHandler {
    void OnGlobalTouchpadPressUp(BaseEventData eventData);
  }

  public interface IGlobalTouchpadTouchDownHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouchDown(BaseEventData eventData);
  }

  public interface IGlobalTouchpadTouchHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouch(BaseEventData eventData);
  }

  public interface IGlobalTouchpadTouchUpHandler : IEventSystemHandler {
    void OnGlobalTouchpadTouchUp(BaseEventData eventData);
  }

  public interface IGlobalPressDownHandler : IGlobalApplicationMenuPressDownHandler, IGlobalGripPressDownHandler, IGlobalTouchpadPressDownHandler, IGlobalTriggerPressDownHandler { }
}