// Exchange.cs
// Created by Aaron Gaudette on 01.04.17

//#define PROFILE

using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace FRL.Network {

  /// <summary>
  /// Abstract base class for the emitter and sink. Contains socket and threading structure.
  /// Less of a design paradigm, more of a practical implementation.
  /// </summary>
  internal abstract class Exchange {

    protected const int SOCKET_TIMEOUT = 1000; //ms
    protected const int PACKET_SIZE = 65507; //bytes

    public bool Running { get { return running; } }
    bool running;

    protected byte[] buffer;
    protected string address;
    protected int port;

    protected Socket socket;

    protected abstract ThreadStart Run { get; } // Overriden for thread loops
    Thread thread;

    protected UnityEngine.Object ThreadLock { get { return threadLock; } }
    UnityEngine.Object threadLock;

    /// <summary>
    /// Initialize with an address and port.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    protected Exchange(string address, int port) {
      threadLock = new UnityEngine.Object();

      this.address = address;
      this.port = port;

      running = false;

      // Initialize the socket
      socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

      socket.SendTimeout = SOCKET_TIMEOUT;
      // Critical, otherwise the receive thread will never terminate
      socket.ReceiveTimeout = SOCKET_TIMEOUT;
    }

    /// <summary>
    /// Call this to ensure running is set to false after thread termination.
    /// </summary>
    protected void Cleanup() {
      if (XRNetworkClient.VERBOSE_LOGS) {
        Debug.Log(
          "Network.Exchange: " + this.GetType().Name + " thread on address "
          + address + ":" + port + " stopped, cleaning up..."
        );
      }

      running = false;
      if (socket != null) socket.Close();
    }

    /// <summary>
    /// Start the thread, as long as it isn't already running.
    /// </summary>
    /// <returns>True if the thread was started, false if it is already running.</returns>
    public bool Start() {
      if (running) return false;
      running = true;

      if (XRNetworkClient.VERBOSE_LOGS) {
        Debug.Log(
          "Network.Exchange: " + this.GetType().Name + " thread on address "
          + address + ":" + port + " starting..."
        );
      }

      thread = new Thread(Run);
      thread.Start();
      return true;
    }

    /// <summary>
    /// Stop the thread.
    /// </summary>
    public void Stop() {
      running = false;

      if (XRNetworkClient.VERBOSE_LOGS) {
        Debug.Log(
          "Network.Exchange: " + this.GetType().Name + " thread on address "
          + address + ":" + port + " stopping..."
        );
      }
    }

    // Debug
    protected int packetCount = 0;
    public int ResetPacketCount() {
      int count = packetCount;
      packetCount = 0;
      return count;
    }
    public abstract string ResetDebugData();
  }

  /// <summary>
  /// Receives/accumulates and processes packets via multicast (downstream) on a dedicated thread.
  /// </summary>
  internal sealed class Sink : Exchange {

    Stack<Update> updates;

    /// <summary>
    /// Initializes downstream, multicast socket given relay address and downstream port.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    internal Sink(string address, int port) : base(address, port) {
      updates = new Stack<Update>();

      socket.Bind(new IPEndPoint(IPAddress.Any, port));
      socket.SetSocketOption(
        SocketOptionLevel.IP, SocketOptionName.AddMembership,
        new MulticastOption(IPAddress.Parse(address))
      );
    }

    /// <summary>
    /// Given a list of untracked Controllers, pop updates off the stack (processing them
    /// chronologically). Update the untracked Controller collection.
    /// </summary>
    /// <param name="untracked"></param>
    public void Update() {
      foreach (Chunk chunk in Chunk.instances)
        chunk.UpdatedThisFrame = false;

      lock (ThreadLock) {
        Update update;
        while (updates.Count > 0) {
          update = updates.Pop();
          update.Load();
        }
      }
    }

    /// <summary>
    /// Core receive loop, accumulating packets as quickly as possible. Raw buffers are translated
    /// and pushed to the packet stack(s) for processing on the main thread.
    /// </summary>
    protected override ThreadStart Run {
      get {
        return () => {
          try {
            // Receive loop
            while (Running) {
              // Get packet (blocking)
              try {
                buffer = new byte[PACKET_SIZE];
                if (socket.Receive(buffer) == 0) continue;
              } catch (SocketException e) {
                if (e.ErrorCode != 10035 && e.ErrorCode != 10060) { // Timeouts
                  Debug.LogError("Network.Sink: Socket error: " + e);
                } else {
                  if (XRNetworkClient.VERBOSE_LOGS) {
                    Debug.Log("Network.Sink: Timeout: " + e);
                  }
                }
                continue;
              }

              Nugget nugget = Nugget.Create(ref buffer);
              // Push to stack(s)
              if (nugget is Update)
                lock (ThreadLock) { updates.Push(nugget as Update); }
              packetCount++;
            }

            Cleanup();
          } catch (SocketException e) {
            Debug.Log(
              "Network.Exchange: Socket exception ("
              + e.ErrorCode + "): " + e
            );
            Cleanup();
          } catch (Exception e) {
            Debug.Log("Network.Sink: " + e);
            Cleanup();
          }
        };
      }
    }

    // Debug
    Dictionary<string, string> debugData = new Dictionary<string, string>();
    public override string ResetDebugData() {
      string data = "Port " + port + ":"
         + (debugData.Count == 0 ? "\n   (Empty)" : "");
      foreach (var entry in debugData)
        data += "\n   " + entry.Key + " (" + entry.Value + ")";
      debugData.Clear();
      return data;
    }
  }
}
