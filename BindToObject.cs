using UnityEngine;
using System.Collections;

public class BindToObject : MonoBehaviour {

  public bool bind = true;
  public Transform goal;
  public Vector3 offset;
  public Vector3 rotOffset;
	// Use this for initialization
	void Start () {
	
	}

  // Update is called once per frame
  void Update() {
    if (bind && goal) {
      this.transform.position = goal.transform.position + goal.transform.rotation * offset;
      this.transform.rotation = goal.transform.rotation * Quaternion.Euler(rotOffset);
    }
	}
}
