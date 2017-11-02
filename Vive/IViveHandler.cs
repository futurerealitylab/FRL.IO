namespace FRL.IO {
	public interface IViveHandler : IPointerViveHandler, IGlobalViveHandler { }

	public interface IPointerViveHandler : IPointerAppMenuHandler, IPointerGripHandler, IPointerTouchpadHandler, IPointerTriggerHandler { }
	public interface IGlobalViveHandler : IGlobalGripHandler, IGlobalTriggerHandler, IGlobalAppMenuHandler, IGlobalTouchpadHandler { }
}