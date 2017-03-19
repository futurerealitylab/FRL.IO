using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public class PointerInputModule : BaseInputModule {

    [Tooltip("Optional tag for limiting interaction.")]
    public string interactTag;
    [Range(0, float.MaxValue)]
    [Tooltip("Interaction range of the module.")]
    public float interactDistance = 10f;

    private List<RaycastHit> hits = new List<RaycastHit>();
    private Ray ray;

    protected new PointerEventData eventData;

    protected virtual void Awake() {
      eventData = new PointerEventData(this);
    }

    protected override void Process() {
      this.Raycast();
      this.UpdateCurrentObject();
      hasBeenProcessed = true;
    }

    protected virtual void OnDisable() {
      eventData.currentRaycast = null;
      this.UpdateCurrentObject();
      eventData.Reset();
    }

    protected void Raycast() {
      hits.Clear();

      //CAST RAY
      Vector3 v = transform.position;
      Quaternion q = transform.rotation;
      ray = new Ray(v, q * Vector3.forward);
      hits.AddRange(Physics.RaycastAll(ray, interactDistance));
      eventData.previousRaycast = eventData.currentRaycast;

      if (hits.Count == 0) {
        eventData.SetCurrentRaycast(null, Vector3.zero, Vector3.zero);
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
        eventData.SetCurrentRaycast(null, Vector3.zero, Vector3.zero);
      } else {
        eventData.SetCurrentRaycast(
          minHit.transform.gameObject, minHit.normal, minHit.point);
      }
    }

    protected void UpdateCurrentObject() {
      this.HandlePointerExitAndEnter(eventData);
    }

    protected void HandlePointerExitAndEnter(PointerEventData eventData) {
      if (eventData.previousRaycast != eventData.currentRaycast) {
        ExecuteEvents.Execute<IPointerEnterHandler>(
          eventData.currentRaycast, eventData, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute<IPointerExitHandler>(
          eventData.previousRaycast, eventData, ExecuteEvents.pointerExitHandler);
      } else if (eventData.currentRaycast != null) {
        ExecuteEvents.Execute<IPointerStayHandler>(
          eventData.currentRaycast, eventData, (x, y) => {
            x.OnPointerStay(eventData);
          });
      }
    }
  }
}

