using UnityEngine;
using System.Collections;

namespace FRL.IO {
  public class Teleporter : MonoBehaviour {

    public ScreenFader fader;

    private Coroutine teleportRoutine;
    

    public bool isTeleporting {
      get {
        return teleportRoutine != null;
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
      Vector3 offset = new Vector3(transform.position.x, 0f, transform.position.z) - camPosition;

      this.transform.position = position + offset;

      //fade in
      fader.fadeIn = true;
      yield return new WaitForSeconds(fader.fadeTime);


      teleportRoutine = null;
    }
  }
}

