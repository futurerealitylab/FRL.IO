using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public class HapticTest : MonoBehaviour {

    public enum Sample {
      None, Full, FullHalf, KnockKnock, SharpTick, BluntTick
    }

    public AnimationCurve curve;
    public float time;
    public XRControllerModule module;
    public Sample sample;

    Dictionary<Sample, byte[]> samples = new Dictionary<Sample, byte[]>();

    private void Start() {
      byte[] full = new byte[320];
      for (int i = 0; i < 320; i++) full[i] = (byte)255;
      samples.Add(Sample.Full, full);

      byte[] half = new byte[320];
      for (int i = 0; i < 320; i++) half[i] = i % 2 == 0 ? (byte)0 : (byte)255;
      samples.Add(Sample.FullHalf, half);

      byte[] knock = new byte[160];
      for (int i = 0; i < 160; i++) knock[i] = (i < 52 || i > 104) ? (byte)255 : (byte)0;
      samples.Add(Sample.KnockKnock, knock);

      byte[] sharp = new byte[7] { 0, 0, 255, 255, 255, 0, 0 };
      samples.Add(Sample.SharpTick, sharp);

      byte[] blunt = new byte[7] { 0, 255, 0, 255, 0, 255, 0 };
      samples.Add(Sample.BluntTick, blunt);
    }


    // Update is called once per frame
    void Update() {
      if (Input.GetKeyDown(KeyCode.Space)) {
        Debug.Log("Sending Haptic Pulse to: " + module.name);
        if (sample != Sample.None) module.HapticPulse(samples[sample]);
        else module.HapticPulse(curve, time);
      }
    }
  }
}

