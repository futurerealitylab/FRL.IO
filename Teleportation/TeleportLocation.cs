using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FRL.IO {
  public class TeleportLocation : MonoBehaviour, IPointerTriggerPressDownHandler, IPointerStayHandler, IPointerEnterHandler, IPointerExitHandler {

    public Teleporter teleporter;

    public  GameObject cursorPrefab;

    private Dictionary<BaseInputModule, GameObject> cursors = new Dictionary<BaseInputModule, GameObject>();


    void IPointerTriggerPressDownHandler.OnPointerTriggerPressDown(ViveControllerModule.EventData eventData) {
      teleporter.Teleport(eventData.worldPosition);
    }

    void IPointerStayHandler.OnPointerStay(PointerEventData eventData) {
      GameObject cursor = cursors[eventData.module];
      cursor.transform.position = eventData.worldPosition;
      cursor.transform.up = eventData.worldNormal;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
      cursors[eventData.module] = GameObject.Instantiate(cursorPrefab);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
      GameObject.Destroy(cursors[eventData.module]);
      cursors[eventData.module] = null;
    }
  }
}

