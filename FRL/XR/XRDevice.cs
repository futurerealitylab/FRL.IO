using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL {
  public abstract class XRDevice : MonoBehaviour {

    [SerializeField]
    [HideInInspector]
    private XRSystem _system;
    public XRSystem System {
      get { return _system; }
      set {
        _system = value;
        OnSystemSwitch(_system);
      }
    }

    [Range(0f, 10f)]
    public float untrackDelay = 3f;

    public Action OnUntracked;
    public Action OnTracked;
    public Action<XRSystem> OnSystemSwitched;

    private Coroutine untrackRoutine;
    protected bool isTrackedPrevious = true;
    protected bool isTracked = true;

    public bool IsTracked {
      get { return isTracked; }
    }

    protected virtual void Start() {

    }

    protected virtual void Update() {
      if (!isTrackedPrevious && isTracked) {
        Track();
      } else if (isTrackedPrevious && !isTracked) {
        Untrack();
      }
      isTrackedPrevious = isTracked;
    }

    private IEnumerator UntrackAsync() {
      yield return new WaitForSeconds(untrackDelay);
      if (OnUntracked != null)
        OnUntracked();
      untrackRoutine = null;
    }

    protected virtual void Untrack() {
      if (untrackRoutine != null)
        return;
      untrackRoutine = StartCoroutine(UntrackAsync());
    }

    protected virtual void Track() {
      if (untrackRoutine != null) {
        StopCoroutine(untrackRoutine);
        untrackRoutine = null;
      }
      if (OnTracked != null)
        OnTracked();
    }

    protected virtual void SwitchToSystem(XRSystem system) {
      OnSystemSwitch(system);
      if (OnSystemSwitched != null) OnSystemSwitched(system);
    }

    protected abstract void OnSystemSwitch(XRSystem system);
  }
}
