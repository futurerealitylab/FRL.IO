using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.IO;

namespace FRL {
	[RequireComponent(typeof(LineRenderer))]
	[RequireComponent(typeof(Receiver))]
	public class XRControllerRod : MonoBehaviour, IGlobalMenuPressDownHandler {

		private LineRenderer line;
		private bool visible = false;

		private Vector3 sPoint = Vector3.zero;
		private Vector3 ePoint = Vector3.zero;

		private Receiver receiver;

		// Use this for initialization
		void Start () {
			line = GetComponent<LineRenderer>();
			receiver = GetComponent<Receiver>();
		}
		
		// Update is called once per frame
		void Update () {
			if (visible) {
				Transform mTransform = receiver.module.transform;
				sPoint = mTransform.position;
				XRControllerModule module = receiver.module as XRControllerModule;
				if (module.xrEventData.currentRaycast != null) {
					ePoint = module.xrEventData.worldPosition;
				} else {
					ePoint = mTransform.position + mTransform.rotation * new Vector3(0f, 0f,module.interactDistance);
				}
			} else {
				ePoint = sPoint = Vector3.zero;
			}
      DrawRod();
		}

    void DrawRod() {
      line.positionCount = 2;
      line.SetPosition(0, sPoint);
      line.SetPosition(1, ePoint);
    }

		public void OnGlobalMenuPressDown(XREventData eventData) {
			visible = !visible;
		}
	}

}
