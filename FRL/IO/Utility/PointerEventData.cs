using UnityEngine;

namespace FRL.IO {
  public class PointerEventData : BaseEventData {

    /// <summary>
    /// The world normal of the current raycast, if it exists. Otherwise, this will equal Vector3.zero.
    /// </summary>
    public Vector3 worldNormal {
      get; internal set;
    }

    /// <summary>
    /// The world position of the current raycast, if it exists. Otherwise, this will equal Vector3.zero.
    /// </summary>
    public Vector3 worldPosition {
      get; internal set;
    }

    /// <summary>
    /// The GameObject currently hit by a raycast from the module.
    /// </summary>
    public GameObject currentRaycast {
      get; internal set;
    }

    /// <summary>
    /// The GameObject previously hit by a raycast from the module.
    /// </summary>
    public GameObject previousRaycast {
      get; internal set;
    }

    public PointerEventData(BaseInputModule module) : base(module) {

    }

    internal void SetCurrentRaycast(GameObject go, Vector3 normal, Vector3 position) {
      this.currentRaycast = go;
      this.worldNormal = normal;
      this.worldPosition = position;
    }

    internal override void Reset() {
      currentRaycast = null;
      previousRaycast = null;
      worldNormal = Vector3.zero;
      worldPosition = Vector3.zero;
    }
  }
}