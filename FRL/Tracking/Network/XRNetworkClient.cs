// Client.cs
// Created by Aaron Gaudette on 11.11.16

using System.Collections.Generic;
using UnityEngine;

namespace FRL.Network {

  /// <summary>
  /// @internal
  /// Native C# network client endpoint with optimized send/receive functionality.
  /// Incoming (downstream) packets are processed on a separate thread. Outgoing (upstream)
  /// packets are processed on a separate thread at a fixed rate and sent asynchronously.
  /// </summary>
  public class XRNetworkClient : Global<XRNetworkClient> {
    public string multicastAddress = "239.0.2.4";
    public int port = 9591;

    /// <summary>
    /// Address of the central relay node.
    /// </summary>
    public string MulticastAddress { get { return multicastAddress; } }
    public int Port { get { return port; } }

    public enum Rate { MOBILE, DESKTOP, HIGH, UNLOCKED };

    /// <summary>
    /// The emission rate of the Client. MOBILE = 30 updates per second, DESKTOP = 60 updates per
    /// second. Capped emission rates induce some additional latency but enforce consistency and
    /// reduce bandwidth and CPU requirements. In general, you can cap to a rate 'x' if your
    /// render FPS is at least 2 * x + 1.
    /// If you're manually capping your framerate and/or using VSync, select UNLOCKED.
    /// When unlocked, the Client will attempt to send (update) packets as fast as Unity renders.
    /// Note: Events are always sent as soon as they are pushed.
    /// </summary>
    [HideInInspector]
    public Rate rate = Rate.UNLOCKED;

    /// <summary>
    /// Minimum time (ms) between updates.
    /// </summary>
    internal static long REFRESH_PERIOD {
      get {
        switch (global.rate) {
          case Rate.MOBILE:
            return 33;
          case Rate.DESKTOP:
            return 17;
          case Rate.HIGH:
            return 11;
        }
        return 0;
      }
    }

    /// <summary>
    /// If true, logs debug data to the console.
    /// </summary>
    [SerializeField] public bool verboseLogs = false;

    public static bool VERBOSE_LOGS { get; private set; }

    List<Chunk> staged, untracked;
    Sink sink;

    // Editor debug data
    public int receivedPPS;
    public int receivedFPS;
    [HideInInspector]
    public List<string> threadData = new List<string>();
    float frameTime = 0;

    public static bool IsTracked(string label) {
      return Chunk.lookup.ContainsKey(label) && Chunk.lookup[label].Tracked;
    }

    public static Vector3 GetPosition(string label) {
      if (!IsTracked(label) || Chunk.lookup[label].data.vector3s.Length == 0) return Vector3.zero;
      return Chunk.lookup[label].data.vector3s[0];
    }

    public static Quaternion GetRotation(string label) {
      if (!IsTracked(label) || Chunk.lookup[label].data.vector4s.Length == 0) return Quaternion.identity;
      return Chunk.lookup[label].data.vector4s[0];
    }

    public static Vector3 GetVector3(string label, int index) {
      if (!IsTracked(label) || Chunk.lookup[label].data.vector3s.Length > (index + 1)) return Vector3.zero;
      return Chunk.lookup[label].data.vector3s[index];
    }

    public static Quaternion GetQuaternion(string label, int index) {
      if (!IsTracked(label) || Chunk.lookup[label].data.vector4s.Length > (index + 1)) return Quaternion.identity;
      return Chunk.lookup[label].data.vector4s[index];
    }

    public static int GetInt(string label, int index) {
      if (!IsTracked(label) || Chunk.lookup[label].data.ints.Length > (index + 1)) return 0;
      return Chunk.lookup[label].data.ints[index];
    }

    public static float GetFloat(string label, int index) {
      if (!IsTracked(label) || Chunk.lookup[label].data.floats.Length > (index + 1)) return 0;
      return Chunk.lookup[label].data.floats[index];
    }

    /// <summary>
    /// Restarts the client, using the settings configued in the inspector.
    /// </summary>
    public static void Restart() {
      global.RestartThreads();
    }

    /// <summary>
    /// Restarts the threads for sending and receiving data, using the client settings configured
    /// in the inspector.
    /// </summary>
    internal void RestartThreads() {
      Stop(); // Make sure nothing is running
      sink = new Sink(multicastAddress, port);
      // Start threads
      sink.Start();
    }

    /// <summary>
    /// Stops the threads for sending and receiving data.
    /// </summary>
    internal void Stop() {
      if (sink != null) sink.Stop();
    }

    void Awake() {
      staged = new List<Chunk>();
      untracked = new List<Chunk>();
      RestartThreads();
    }

    /// <summary>
    /// Publishes events, updates and stages Controllers.
    /// Because of the script execution order, this is guaranteed to be called
    /// before any of the Controllers, priming their data.
    /// </summary>
    void Update() {
      VERBOSE_LOGS = verboseLogs;
      if (!sink.Running) {
        sink.Start();
      }

      // Set editor debug data every second
      if (Application.isEditor) {
        frameTime += Time.deltaTime;
        if (frameTime > 1) {
          Display();
          frameTime = 0;
        }
      } else {
        frameTime += Time.deltaTime;
        if (frameTime > 1) {
          receivedPPS = sink.ResetPacketCount();
          frameTime = 0;
        }
      }

      Refresh(); // Update sink as soon as possible and stage Controllers for sending
    }

    /// <summary>
    /// Stages Controllers for sending/receiving and updates the sink (incoming data).
    /// </summary>
    void Refresh() {
      staged.Clear(); 
      untracked.Clear();
      sink.Update();
    }

    /// <summary>
    /// Set editor debug data
    /// </summary>
    void Display() {
      threadData.Clear();
      threadData.Add(sink.ResetDebugData());
      receivedPPS = sink.ResetPacketCount();
    }

    void OnDestroy() {
      Stop();
    }

    void OnApplicationFocus(bool focus) {
      if (focus) {
        Debug.Log("Application Resuming. Restarting XRNetworkClient threads.");
        RestartThreads();
      }
    }
  }
}
