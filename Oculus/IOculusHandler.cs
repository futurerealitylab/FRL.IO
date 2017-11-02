namespace FRL.IO {
  public interface IOculusHandler : IPointerOculusHandler, IGlobalOculusHandler { }

  public interface IPointerOculusHandler : IPointerTriggerHandler, IPointerAppMenuHandler, IPointerGripHandler, IPointerAHandler, IPointerBHandler, IPointerXHandler, IPointerYHandler, IPointerThumbstickHandler { }
  public interface IGlobalOculusHandler : IGlobalTriggerHandler, IGlobalAppMenuHandler, IGlobalGripHandler, IGlobalAHandler, IGlobalBHandler, IGlobalXHandler, IGlobalYHandler, IGlobalThumbstickHandler { }
}

