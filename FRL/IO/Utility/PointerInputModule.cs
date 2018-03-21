using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public abstract class PointerInputModule : BaseInputModule {

    [Tooltip("Optional tag for limiting interaction.")]
    public string interactTag;
    [Range(0, float.MaxValue)]
    [Tooltip("Interaction range of the module.")]
    public float interactDistance = 10f;

    private List<RaycastHit> hits = new List<RaycastHit>();
    private Ray ray;

    protected override BaseEventData baseEventData {
      get {
        return pointerEventData;
      }
    }

    protected abstract PointerEventData pointerEventData {
      get;
    }

    protected virtual void Awake() {

    }

    protected override void Process() {
      this.Raycast();
      this.UpdateCurrentObject();
      hasBeenProcessed = true;
    }

    protected virtual void OnDisable() {
      pointerEventData.currentRaycast = null;
      this.UpdateCurrentObject();
      pointerEventData.Reset();
    }

    protected void Raycast() {
      hits.Clear();

      //CAST RAY
      Vector3 v = transform.position;
      Quaternion q = transform.rotation;
      ray = new Ray(v, q * Vector3.forward);
      hits.AddRange(Physics.RaycastAll(ray, interactDistance));
      pointerEventData.previousRaycast = pointerEventData.currentRaycast;

      if (hits.Count == 0) {
        pointerEventData.SetCurrentRaycast(null, Vector3.zero, Vector3.zero);
        return;
      }

      //find the closest object.
      RaycastHit minHit = hits[0];
      for (int i = 0; i < hits.Count; i++) {
        if (hits[i].distance < minHit.distance) {
          minHit = hits[i];
        }
      }

      //make sure the closest object is able to be interacted with.
      if (interactTag != null && interactTag.Length > 1
        && !minHit.transform.tag.Equals(interactTag)) {
        pointerEventData.SetCurrentRaycast(null, Vector3.zero, Vector3.zero);
      } else {
        pointerEventData.SetCurrentRaycast(
          minHit.collider.gameObject, minHit.normal, minHit.point);
      }
    }

    protected void UpdateCurrentObject() {
      this.HandlePointerExitAndEnter(pointerEventData);
    }

    protected void HandlePointerExitAndEnter(PointerEventData pointerEventData) {
      if (pointerEventData.previousRaycast != pointerEventData.currentRaycast) {
        ExecuteEvents.Execute<IPointerEnterHandler>(
          pointerEventData.currentRaycast, pointerEventData, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute<IPointerExitHandler>(
          pointerEventData.previousRaycast, pointerEventData, ExecuteEvents.pointerExitHandler);
      } else if (pointerEventData.currentRaycast != null) {
        ExecuteEvents.Execute<IPointerStayHandler>(
          pointerEventData.currentRaycast, pointerEventData, (x, y) => {
            x.OnPointerStay(pointerEventData);
          });
      }
    }
  }
}

