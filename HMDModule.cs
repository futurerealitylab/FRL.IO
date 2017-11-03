using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public class HMDModule : PointerInputModule {
    private VREventData _eventData;

    protected override VREventData eventData {
      get {
        return _eventData;
      }
    }

  }
}

