# FRL.XR
The unified FRL library for XR systems in Unity.

## Install
Either download and unpack the .zip, or clone this repo directly into the `Assets` folder of your Unity project.
```bash
cd ./Assets/
git clone https://github.com/futurerealitylab/FRL.XR.git
```

## XRManager and IO: How to use
Drag the `XRManager` prefab into your scene. Then, look at the `XRManager` Component:

![XRManager](https://github.com/futurerealitylab/FRL.XR/blob/master/Documentation/XRManager.PNG)

There are four supported SDKs: `Wave`, `OVR`, `SteamVR`, and `Daydream`. `Windows Mixed Reality` is technically supported without an SDK, but requires Windows 10 Fall Creators edition. Select the SDKs you plan on using. There will be compilation errors if you select an SDK and don't have it installed in your Unity project. Once you've selected the SDKs, change the `System` field to match the system you want to use.

If you have custom models for your controllers, you can put them under the `XRController` GameObjects found under `XRManager`.

### Receiving Input On GameObjects: Pointer vs. Global

In order for a GameObject to receive input when it is being pointed at, it needs a component which implements one of the `Pointer` interfaces, and a `Collider` component. 

In order for a GameObject to receive input globally (whenever any button is pressed), it needs a `Receiver` component and a component which implements one of the `Global` interfaces. If the `module` field of the `Receiver` component is populated with one of the `XRControllerModule` or `HMDModule` components found on the `XRHMD` and `XRController` GameObjects, that GameObject will only receive global input from that specific module. Otherwise, the GameObject will receive input from __all__ input modules.

### Interfaces and Button -> Function Call

Here is an example implementation of a GameObject receiving global and pointer inputs.

```csharp
using UnityEngine;
using FRL.IO;

[RequireComponent(typeof(Receiver))]
public class ExampleInput : MonoBehaviour, IPointerTriggerPressDownHandler, IGlobalTouchpadTouchHandler {

  private Renderer objectRenderer;

  void Awake() {
    objectRenderer = GetComponent<Renderer>();
  }

  private void SetColor(Color color) {
    Debug.Log("Setting Color to: " + color);
    objectRenderer.material.color = color;
  }

  public void OnPointerTriggerPressDown(XREventData eventData) {
    //This will only be called when the object is being pointed at by a controller.
    Color color = (eventData.hand == XRHand.Left ? Color.blue : Color.red);
    SetColor(color);
  }

  public void OnGlobalTouchpadTouch(XREventData eventData) {
    //This will be called whenever any touchpad is touched.
    Debug.Log("My " + eventData.hand + " hand is touching the Touchpad!");
  }
}
```

The full set of supported interfaces can be found in the `FRL/IO/Interfaces/Interfaces.cs` class.

## XRNetworkClient: How to use

The `XRNetworkClient` is designed to receive data from a `holojam-node` relay. Place the `XRNetworkClient.cs` component onto a GameObject, and make sure the multicast address and the port match your `holojam-node` address and port.

### Getting a Tracked Object

Place an `XRTrackedObject.cs` component onto the GameObject you want to track. 



## For Developers

This is an actively developed, open source library. Please feel free to log issues and feedback! It helps us make the library better for you.

## License

Copyright 2018 NYU Future Reality lab

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
