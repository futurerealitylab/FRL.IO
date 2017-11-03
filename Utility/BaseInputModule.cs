using UnityEngine;
using System.Collections;

namespace FRL.IO {
  public abstract class BaseInputModule : MonoBehaviour {

    protected abstract BaseEventData baseEventData {
      get;
    }

    protected bool hasBeenProcessed = false;

    protected abstract void Process();
  }
}

