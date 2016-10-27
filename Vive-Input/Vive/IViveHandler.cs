using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace FRL.IO {
	public interface IViveHandler : IPointerViveHandler, IGlobalViveHandler { }

	public interface IPointerViveHandler : IPointerAppMenuHandler, IPointerGripHandler, IPointerTouchpadHandler, IPointerTriggerHandler { }

	//APPLICATION MENU HANDLER
	public interface IPointerAppMenuHandler : IPointerAppMenuPressDownHandler, IPointerAppMenuPressHandler, IPointerAppMenuPressUpHandler { }

	public interface IPointerAppMenuPressDownHandler : IEventSystemHandler {
		void OnPointerAppMenuPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IPointerAppMenuPressHandler : IEventSystemHandler {
		void OnPointerAppMenuPress(ViveControllerModule.EventData eventData);
	}

	public interface IPointerAppMenuPressUpHandler : IEventSystemHandler {
		void OnPointerAppMenuPressUp(ViveControllerModule.EventData eventData);
	}

	//GRIP HANDLER
	public interface IPointerGripHandler : IPointerGripPressDownHandler, IPointerGripPressHandler, IPointerGripPressUpHandler { }

	public interface IPointerGripPressDownHandler : IEventSystemHandler {
		void OnPointerGripPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IPointerGripPressHandler : IEventSystemHandler {
		void OnPointerGripPress(ViveControllerModule.EventData eventData);
	}
	public interface IPointerGripPressUpHandler : IEventSystemHandler {
		void OnPointerGripPressUp(ViveControllerModule.EventData eventData);
	}

	//TOUCHPAD HANDLER
	public interface IPointerTouchpadHandler : IPointerTouchpadPressSetHandler, IPointerTouchpadTouchSetHandler { }
	public interface IPointerTouchpadPressSetHandler : IPointerTouchpadPressDownHandler, IPointerTouchpadPressHandler, IPointerTouchpadPressUpHandler { }
	public interface IPointerTouchpadTouchSetHandler : IPointerTouchpadTouchDownHandler, IPointerTouchpadTouchHandler, IPointerTouchpadTouchUpHandler { }

	public interface IPointerTouchpadPressDownHandler : IEventSystemHandler {
		void OnPointerTouchpadPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTouchpadPressHandler : IEventSystemHandler {
		void OnPointerTouchpadPress(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTouchpadPressUpHandler : IEventSystemHandler {
		void OnPointerTouchpadPressUp(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTouchpadTouchDownHandler : IEventSystemHandler {
		void OnPointerTouchpadTouchDown(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTouchpadTouchHandler : IEventSystemHandler {
		void OnPointerTouchpadTouch(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTouchpadTouchUpHandler : IEventSystemHandler {
		void OnPointerTouchpadTouchUp(ViveControllerModule.EventData eventData);
	}

	//TRIGGER HANDLER
	public interface IPointerTriggerHandler : IPointerTriggerPressSetHandler, IPointerTriggerTouchSetHandler { }
	public interface IPointerTriggerPressSetHandler : IPointerTriggerPressDownHandler, IPointerTriggerPressHandler, IPointerTriggerPressUpHandler { }
	public interface IPointerTriggerTouchSetHandler : IPointerTriggerTouchDownHandler, IPointerTriggerTouchHandler, IPointerTriggerTouchUpHandler { }

	public interface IPointerTriggerPressDownHandler : IEventSystemHandler {
		void OnPointerTriggerPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTriggerPressHandler : IEventSystemHandler {
		void OnPointerTriggerPress(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTriggerPressUpHandler : IEventSystemHandler {
		void OnPointerTriggerPressUp(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTriggerTouchDownHandler : IEventSystemHandler {
		void OnPointerTriggerTouchDown(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTriggerTouchHandler : IEventSystemHandler {
		void OnPointerTriggerTouch(ViveControllerModule.EventData eventData);
	}

	public interface IPointerTriggerTouchUpHandler : IEventSystemHandler {
		void OnPointerTriggerTouchUp(ViveControllerModule.EventData eventData);
	}

  public interface IPointerTriggerClickHandler : IEventSystemHandler {
    void OnPointerTriggerClick(ViveControllerModule.EventData eventData);
  }

	//GLOBAL VIVE HANDLER: ALL Global BUTTON SETS
	public interface IGlobalViveHandler : IGlobalGripHandler, IGlobalTriggerHandler, IGlobalApplicationMenuHandler, IGlobalTouchpadHandler { }

	/// GLOBAL GRIP HANDLER
	public interface IGlobalGripHandler : IGlobalGripPressDownHandler, IGlobalGripPressHandler, IGlobalGripPressUpHandler { }

	public interface IGlobalGripPressDownHandler : IEventSystemHandler {
		void OnGlobalGripPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalGripPressHandler : IEventSystemHandler {
		void OnGlobalGripPress(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalGripPressUpHandler : IEventSystemHandler {
		void OnGlobalGripPressUp(ViveControllerModule.EventData eventData);
	}


	//GLOBAL TRIGGER HANDLER
	public interface IGlobalTriggerHandler : IGlobalTriggerPressSetHandler, IGlobalTriggerTouchSetHandler { }
	public interface IGlobalTriggerPressSetHandler : IGlobalTriggerPressDownHandler, IGlobalTriggerPressHandler, IGlobalTriggerPressUpHandler { }
	public interface IGlobalTriggerTouchSetHandler : IGlobalTriggerTouchDownHandler, IGlobalTriggerTouchHandler, IGlobalTriggerTouchUpHandler { }

	public interface IGlobalTriggerPressDownHandler : IEventSystemHandler {
		void OnGlobalTriggerPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTriggerPressHandler : IEventSystemHandler {
		void OnGlobalTriggerPress(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTriggerPressUpHandler : IEventSystemHandler {
		void OnGlobalTriggerPressUp(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTriggerTouchDownHandler : IEventSystemHandler {
		void OnGlobalTriggerTouchDown(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTriggerTouchHandler : IEventSystemHandler {
		void OnGlobalTriggerTouch(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTriggerTouchUpHandler : IEventSystemHandler {
		void OnGlobalTriggerTouchUp(ViveControllerModule.EventData eventData);
	}

  public interface IGlobalTriggerClickHandler : IEventSystemHandler {
    void OnGlobalTriggerClick(ViveControllerModule.EventData eventData);
  }

	//GLOBAL APPLICATION MENU
	public interface IGlobalApplicationMenuHandler : IGlobalApplicationMenuPressDownHandler, IGlobalApplicationMenuPressHandler, IGlobalApplicationMenuPressUpHandler { }

	public interface IGlobalApplicationMenuPressDownHandler : IEventSystemHandler {
		void OnGlobalApplicationMenuPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalApplicationMenuPressHandler : IEventSystemHandler {
		void OnGlobalApplicationMenuPress(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalApplicationMenuPressUpHandler : IEventSystemHandler {
		void OnGlobalApplicationMenuPressUp(ViveControllerModule.EventData eventData);
	}

	//GLOBAL TOUCHPAD 
	public interface IGlobalTouchpadHandler : IGlobalTouchpadPressSetHandler, IGlobalTouchpadTouchSetHandler { }

	public interface IGlobalTouchpadPressSetHandler : IGlobalTouchpadPressDownHandler, IGlobalTouchpadPressHandler, IGlobalTouchpadPressUpHandler { }
	public interface IGlobalTouchpadTouchSetHandler : IGlobalTouchpadTouchDownHandler, IGlobalTouchpadTouchHandler, IGlobalTouchpadTouchUpHandler { }

	public interface IGlobalTouchpadPressDownHandler : IEventSystemHandler {
		void OnGlobalTouchpadPressDown(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTouchpadPressHandler : IEventSystemHandler {
		void OnGlobalTouchpadPress(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTouchpadPressUpHandler : IEventSystemHandler {
		void OnGlobalTouchpadPressUp(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTouchpadTouchDownHandler : IEventSystemHandler {
		void OnGlobalTouchpadTouchDown(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTouchpadTouchHandler : IEventSystemHandler {
		void OnGlobalTouchpadTouch(ViveControllerModule.EventData eventData);
	}

	public interface IGlobalTouchpadTouchUpHandler : IEventSystemHandler {
		void OnGlobalTouchpadTouchUp(ViveControllerModule.EventData eventData);
	}
}