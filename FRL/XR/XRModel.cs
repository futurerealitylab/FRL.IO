using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL {
  public class XRModel : MonoBehaviour {

    public XRDevice device;
    [Range(0f, 5f)]
    public float fadeTime = 1f;

    public bool ShowOverride { get; set; }
    public bool HideOverride { get; set; }

    private List<Renderer> renderers = new List<Renderer>();
    private Coroutine fadeRoutine;

    void Start() {
      renderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    void OnEnable() {
      if (device) {
        device.OnTracked += DoTracked;
        device.OnUntracked += DoUntracked;
      } else {
        Debug.LogError("No XRSystemDevice for XRSystemModel: " + this.name);
      }
    }

    void OnDisable() {
      if (device) {
        device.OnTracked -= DoTracked;
        device.OnUntracked -= DoUntracked;
      }
    }

    private void Update() {
      if (ShowOverride || HideOverride) {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (ShowOverride) this.SetAlpha(1f);
        else if (HideOverride) this.SetAlpha(0f);
      }
    }

    void DoUntracked() {
      if (fadeRoutine != null) {
        StopCoroutine(fadeRoutine);
      }
      fadeRoutine = StartCoroutine(FadeAsync(false));
    }

    void SetAlpha(float alpha) {
      Mathf.Clamp01(alpha);
      foreach (Renderer r in renderers) {
        Color c = r.material.color;
        r.material.color = new Color(c.r, c.g, c.b, alpha);
        r.enabled = alpha > 0;
      }
    }

    void DoTracked() {
      if (fadeRoutine != null) {
        StopCoroutine(fadeRoutine);
      }
      fadeRoutine = StartCoroutine(FadeAsync(true));
    }

    IEnumerator FadeAsync(bool isFadingIn) {

      float startTime = 0f;
      while (startTime < fadeTime) {
        startTime += Time.deltaTime;
        float alpha = Mathf.Clamp01(startTime / fadeTime);
        alpha = isFadingIn ? alpha : 1 - alpha;
        SetAlpha(alpha);
        yield return null;
      }
      SetAlpha(isFadingIn ? 1 : 0);
    }
  }
}

