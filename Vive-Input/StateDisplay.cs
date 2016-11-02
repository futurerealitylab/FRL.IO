using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FRL.IO;
using System;
using UnityEngine.UI;

public class StateDisplay : MonoBehaviour {

  public GameObject goal;

  private Text text;

  private static Dictionary<Type, string> stateNames = new Dictionary<Type, string>() {
    {typeof(Grabber), "Grab" },
    {typeof(TeleportController), "Teleport" }
  };

  void Awake() {
    text = GetComponent<Text>();
    if (text == null) {
      text = GetComponentInChildren<Text>();
    }
  }

  void Update() {
    bool found = false;
    foreach (Type type in stateNames.Keys) {
      MonoBehaviour bh = goal.GetComponent(type) as MonoBehaviour;
      if (bh != null && bh.isActiveAndEnabled) {
        found = true;
        text.text = stateNames[type];
      }
    }
    if (!found) {
      text.text = "NaN";
    }
  }
}
