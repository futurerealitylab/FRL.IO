// Translation.cs
// Created by Aaron Gaudette on 12.02.17
// Collection of classes dealing with raw protocol translation

using System;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;

namespace FRL.Network {

  /// <summary>
  /// Wrapper around a nugget struct in order to prevent unwanted copying--provides
  /// low-level functionality.
  /// </summary>
  internal class NuggetWrapper {
    public readonly Protocol.Nugget data;

    /// <summary>
    /// Generate a nugget from a raw buffer.
    /// </summary>
    /// <param name="buffer"></param>
    public NuggetWrapper(ref byte[] buffer) {
      data = Protocol.Nugget.GetRootAsNugget(new FlatBuffers.ByteBuffer(buffer));
    }
    /// <summary>
    /// Copy a raw flake from this wrapper's nugget to a specified target Controller.
    /// </summary>
    /// <param name="i">Index of the Protocol.Flake in this nugget to copy.</param>
    /// <param name="controller">Target Controller.</param>
    public void CopyToChunk(int i, Chunk chunk) {
      CopyToFlake(i, chunk.data);
    }

    /// <summary>
    /// Copy a raw flake from this wrapper's nugget to a specified Flake in Unity.
    /// </summary>
    /// <param name="i">Index of the Protocol.Flake in this nugget to copy.</param>
    /// <param name="flake">Target Flake.</param>
    public void CopyToFlake(int i, Flake flake) {
      flake.vector3s = new Vector3[data.Flakes(i).Value.Vector3sLength];
      for (int j = 0; j < data.Flakes(i).Value.Vector3sLength; ++j)
        flake.vector3s[j] = new Vector3(
           data.Flakes(i).Value.Vector3s(j).Value.X,
           data.Flakes(i).Value.Vector3s(j).Value.Y,
           data.Flakes(i).Value.Vector3s(j).Value.Z
        );
      flake.vector4s = new Quaternion[data.Flakes(i).Value.Vector4sLength];
      for (int j = 0; j < data.Flakes(i).Value.Vector4sLength; ++j)
        flake.vector4s[j] = new Quaternion(
           data.Flakes(i).Value.Vector4s(j).Value.X,
           data.Flakes(i).Value.Vector4s(j).Value.Y,
           data.Flakes(i).Value.Vector4s(j).Value.Z,
           data.Flakes(i).Value.Vector4s(j).Value.W
        );

      flake.floats = new float[data.Flakes(i).Value.FloatsLength];
      for (int j = 0; j < data.Flakes(i).Value.FloatsLength; ++j)
        flake.floats[j] = data.Flakes(i).Value.Floats(j);

      flake.ints = new int[data.Flakes(i).Value.IntsLength];
      for (int j = 0; j < data.Flakes(i).Value.IntsLength; ++j)
        flake.ints[j] = data.Flakes(i).Value.Ints(j);

      flake.bytes = new byte[data.Flakes(i).Value.BytesLength];
      for (int j = 0; j < data.Flakes(i).Value.BytesLength; ++j)
        flake.bytes[j] = (byte)data.Flakes(i).Value.Bytes(j);

      flake.text = data.Flakes(i).Value.Text;
    }
  }

  /// <summary>
  /// Abstract base class for translation functionality around updates and
  /// events/notifications (incoming). Contains a NuggetWrapper.
  /// </summary>
  abstract internal class Nugget {
    protected NuggetWrapper nugget;

    /// <summary>
    /// Protected constructor for factory method.
    /// </summary>
    /// <param name="nugget"></param>
    protected Nugget(NuggetWrapper nugget) {
      this.nugget = nugget;
    }

    /// <summary>
    /// Factory method for creating Nuggets of a specific derived type given a raw buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>The derived Nugget.</returns>
    public static Nugget Create(ref byte[] buffer) {
      NuggetWrapper nugget = new NuggetWrapper(ref buffer);
      switch (nugget.data.Type) {
        case Protocol.NuggetType.UPDATE:
          return new Update(nugget);
        default: return null;
      }
    }

    /// <summary>
    /// Debug function for custom Client inspector.
    /// </summary>
    /// <param name="debugData"></param>
    public void UpdateDebug(Dictionary<string, string> debugData) {
      string scope = nugget.data.Scope;
      for (int i = 0; i < nugget.data.FlakesLength; ++i)
        debugData[scope + "."
           + nugget.data.Flakes(i).Value.Label] = nugget.data.Origin;
    }
  }

  /// <summary>
  /// Nugget extension for updates (a collection of flakes).
  /// </summary>
  internal sealed class Update : Nugget {
    readonly Dictionary<string, int> lookup;

    /// <summary>
    /// Update constructor: initializes and populates lookup table.
    /// </summary>
    /// <param name="nugget"></param>
    internal Update(NuggetWrapper nugget) : base(nugget) {
      // Initialize lookup table
      lookup = new Dictionary<string, int>();
      for (int i = 0; i < nugget.data.FlakesLength; ++i)
        lookup[nugget.data.Flakes(i).Value.Label] = i;
    }

    /// <summary>
    /// Load a raw flake from this Update into a specified Controller in Unity using the lookup table.
    /// </summary>
    /// <param name="controller"></param>
    /// <returns>
    /// True if successful, false if the Controller is not present, there is a scope mismatch, or
    /// the flake is null.
    /// </returns>
    public void Load() {
      foreach (string label in lookup.Keys) {
        Chunk chunk;
        if (!Chunk.lookup.ContainsKey(label)) {
          chunk = new Chunk(label);
        } else {
          chunk = Chunk.lookup[label];
        }
        if (chunk.UpdatedThisFrame) continue; //Skip already loaded chunks.
        chunk.Timestamp = Time.unscaledTime; // Set timestamp
        nugget.CopyToChunk(lookup[chunk.Label], chunk); // Copy over data
        chunk.UpdatedThisFrame = true; // Set updated flag
      }
    }
  }
}
