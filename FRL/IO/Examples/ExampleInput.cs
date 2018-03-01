using UnityEngine;
using FRL.IO;

[RequireComponent(typeof(Receiver))]
public class ExampleInput : MonoBehaviour, IPointerTriggerPressDownHandler, IGlobalTouchpadTouchHandler {

  private Renderer objectRenderer;

  void Awake() {
    objectRenderer = GetComponent<Renderer>();
  }

  private void SetColor(Color color) {
    Debug.Log("Setting Color to: " + color);
    objectRenderer.material.color = color;
  }

  public void OnPointerTriggerPressDown(XREventData eventData) {
    Color color = (eventData.hand == XRHand.Left ? Color.blue : Color.red);
    SetColor(color);
  }

  public void OnGlobalTouchpadTouch(XREventData eventData) {
    Debug.Log("My " + eventData.hand + " hand is touching the Touchpad!");
  }
}
