using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FRL.IO {
  public class TeleportLocation : MonoBehaviour {//, IPointerTriggerPressDownHandler, IPointerStayHandler, IPointerEnterHandler, IPointerExitHandler {

    //public Teleporter teleporter;
    //public  GameObject cursorPrefab;
    //public BaseInputModule module;
  

    //private Dictionary<BaseInputModule, GameObject> cursors = new Dictionary<BaseInputModule, GameObject>();


    //void IPointerTriggerPressDownHandler.OnPointerTriggerPressDown(ViveControllerModule.EventData eventData) {
    //  if (module == null || module != null && module == eventData.module) {
    //    teleporter.Teleport(eventData.worldPosition);
    //  }
    //}

    //void IPointerStayHandler.OnPointerStay(PointerEventData eventData) {
    //  if (module == null || module != null && module == eventData.module) {
    //    GameObject cursor = cursors[eventData.module];
    //    cursor.transform.position = eventData.worldPosition;
    //    cursor.transform.up = eventData.worldNormal;
    //  }
    //}

    //void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
    //  if (module == null || module != null && module == eventData.module) {
    //    cursors[eventData.module] = GameObject.Instantiate(cursorPrefab);
    //  }
    //}

    //void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
    //  if (module == null || module != null && module == eventData.module) {
    //    GameObject.Destroy(cursors[eventData.module]);
    //    cursors[eventData.module] = null;
    //  }
    //}
  }
}

