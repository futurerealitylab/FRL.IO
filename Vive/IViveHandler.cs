using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace FRL.IO {
	public interface IViveHandler : IPointerViveHandler, IGlobalViveHandler { }

	public interface IPointerViveHandler : IPointerAppMenuHandler, IPointerGripHandler, IPointerTouchpadHandler, IPointerTriggerHandler { }
	public interface IGlobalViveHandler : IGlobalGripHandler, IGlobalTriggerHandler, IGlobalApplicationMenuHandler, IGlobalTouchpadHandler { }
}