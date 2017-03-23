using UnityEngine;
using System.Collections;

namespace FRL.IO {
  public abstract class BaseInputModule : MonoBehaviour {

    protected BaseEventData eventData;

    protected bool hasBeenProcessed = false;

    protected abstract void Process();
  }
}

