using UnityEngine;
using System.Collections;
using FRL.IO;
using System;

[RequireComponent(typeof(Receiver))]
public class TeleportController : MonoBehaviour, IGlobalTriggerClickHandler {

  private static Color validCastColor = Color.green;
  private static Color invalidCastColor = Color.red;

  public GameObject camRig;
  public ScreenFader fader;
  public GameObject cursor;
  public LineRenderer line;

  private Receiver receiver;

  private static Coroutine teleportRoutine;

  void Awake() {
    receiver = this.GetComponent<Receiver>();
  }

  void Update() {
    if (receiver.module == null) {
      Debug.LogError("TeleportController " + name + " does not have a module. It requires a module.");
    }

    ViveControllerModule.EventData eventData = (receiver.module as ViveControllerModule).GetEventData();
    if (eventData.currentRaycast != null) {
      if (eventData.currentRaycast.GetComponent<TeleportLocation>() != null) {
        EnableCursorAndLine(eventData, validCastColor);
      } else {
        EnableCursorAndLine(eventData, invalidCastColor);
      }
    } else {
      DisableCursorAndLine();
    }
  }

  void OnDisable() {
    DisableCursorAndLine();
  }

  void EnableCursorAndLine(PointerEventData eventData, Color color) {
    if (cursor) {
      cursor.SetActive(true);
      cursor.transform.position = eventData.worldPosition;
      cursor.transform.up = eventData.worldNormal;
      cursor.GetComponentInChildren<Renderer>().material.color = color;
    }

    if (line) {
      line.gameObject.SetActive(true);
      line.SetVertexCount(2);
      line.SetPosition(0, eventData.module.transform.position);
      line.SetPosition(1, eventData.worldPosition);
      line.SetColors(color, color);
    }
  }

  void DisableCursorAndLine() {
    if (cursor) {
      cursor.SetActive(false);
    }
    if (line) {
      line.gameObject.SetActive(false);
    }
  }

  void IGlobalTriggerClickHandler.OnGlobalTriggerClick(BaseEventData eventData) {
    PointerEventData e = (PointerEventData)eventData;
    if (e.currentRaycast != null && e.currentRaycast.GetComponent<TeleportLocation>() != null) {
      Teleport(e.worldPosition);
    }
  }

  public void Teleport(Vector3 position) {
    if (!this.isActiveAndEnabled) {
      return;
    }

    if (teleportRoutine == null) {
      teleportRoutine = StartCoroutine(TeleportWithFade(position));
    }
  }

  private IEnumerator TeleportWithFade(Vector3 position) {
    //fade out to black
    fader.fadeIn = false;
    yield return new WaitForSeconds(fader.fadeTime);

    //move
    Vector3 camPosition = Camera.main.transform.position;
    camPosition = new Vector3(camPosition.x, 0f, camPosition.z);
    Vector3 offset = new Vector3(camRig.transform.position.x, 0f, camRig.transform.position.z) - camPosition;

    camRig.transform.position = position + offset;

    //fade in
    fader.fadeIn = true;
    yield return new WaitForSeconds(fader.fadeTime);

    teleportRoutine = null;
  }
}
