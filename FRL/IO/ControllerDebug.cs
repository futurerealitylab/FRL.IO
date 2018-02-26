using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.IO;
using UnityEngine.UI;

public class ControllerDebug : MonoBehaviour {

  public List<XRControllerModule> modules;
  public Text text;
	
	// Update is called once per frame
	void Update () {
    string status = "";
    foreach (XRControllerModule module in modules) {
      status += module.hand.ToString() + ":\t\t";
      status += module.IsTracked ? "Tracked\t" : "Untracked\t";
      status += module.transform.localPosition.ToString() + "\t";
      status += module.transform.localRotation.ToString();
      status += "\n";
    }
    text.text = status;
	}
}
