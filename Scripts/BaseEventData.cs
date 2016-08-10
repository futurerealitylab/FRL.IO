
namespace FRL.IO {
  public class BaseEventData {

    public BaseInputModule module {
      get; private set;
    }

    public BaseEventData(BaseInputModule module) {
      this.module = module;
    }
  }
}


