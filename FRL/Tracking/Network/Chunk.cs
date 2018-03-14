using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace FRL.Network {
  public class Chunk {

    /// <summary>
    /// Length of time before Chunk is considered untracked.
    /// </summary>
    const float TRACKED_TIMEOUT = .2f; // 5 fps

    float timestamp, lastTimestamp;

    /// <summary>
    /// Static read-only list of all the specified Chunks.
    /// </summary>
    /// <typeparam name="T">The Chunk subtype to filter.</typeparam>
    /// <returns>A ReadOnlyCollection containing Ts.</returns>
    public static ReadOnlyCollection<T> All<T>() where T : Chunk {
      List<T> filtered = new List<T>();
      // This could be slow. A better (but less flexible) solution is to override OnEnable/Disable
      foreach (Chunk chunk in instances)
        if (chunk is T) filtered.Add(chunk as T);
      return filtered.AsReadOnly();
    }

    /// <summary>
    /// Instance list made internal (unsafe) for speed within XRNetworkClient.cs.
    /// </summary>
    internal static HashSet<Chunk> instances = new HashSet<Chunk>();

    internal static Dictionary<string, Chunk> lookup = new Dictionary<string, Chunk>();

    /// <summary>
    /// The last time the Chunk received fresh data.
    /// </summary>
    public float Timestamp {
      get { return timestamp; }
      internal set {
        lastTimestamp = timestamp;
        timestamp = value;
      }
    }

    public Chunk(string label) {
      this.label = label;
      instances.Add(this);
      lookup.Add(label, this);
    }

    /// <summary>
    /// The difference between the current timestamp and the last.
    /// In the case of an update: similar to Time.deltaTime,
    /// but between updates instead of the render loop.
    /// </summary>
    /// <returns>The delta, in seconds.</returns>
    public float DeltaTime() {
      return Timestamp - lastTimestamp;
    }

    /// <summary>
    /// Read-only bool indicating if the Chunk received fresh data this frame.
    /// </summary>
    public bool UpdatedThisFrame {
      get; internal set;
    }

    /// <summary>
    /// Read-only bool indicating if the Controller has received fresh data recently.
    /// Use Chunk.UpdatedThisFrame for a more exact value.
    /// </summary>
    public bool Tracked {
      get {
        return instances.Contains(this) && (Time.unscaledTime - Timestamp < TRACKED_TIMEOUT);
      }
    }

    /// <summary>
    /// Override this to control the data label (string identifier).
    /// </summary>
    private string label;
    public string Label { get { return label; } }


    /// <summary>
    /// Data container. Protected to allow for direct modification in subclasses.
    /// When subclassing, make sure to create public-facing proxies for readability and
    /// controlling read/write access. See examples.
    /// </summary>
    internal protected Flake data = new Flake();

    /// <summary>
    /// Reset the flake data. A necessary override to allocate the optional members beforehand.
    /// </summary>
    public virtual void ResetData() {
      data = new Flake();
    }
  }
}
