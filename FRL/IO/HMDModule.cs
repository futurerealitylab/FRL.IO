using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public class HMDModule : PointerInputModule {

    private EventData eventData;
    protected override PointerEventData pointerEventData {
      get {
        return eventData;
      }
    }

    protected override void Awake() {
      base.Awake();
      eventData = new EventData(this);
    }

    public class EventData : PointerEventData {
      public HMDModule hmdModule;
      internal EventData(HMDModule module) : base(module) {
        this.hmdModule = module;
      }
    }
  }
}

