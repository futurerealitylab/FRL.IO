-----------------------------------------------------
---------Vive Support Library for Unity--------------
-----------------------------------------------------

1) **Version**

2) **Overview**

3) **Guide**

3.5) **EventData**

4) **Interface Bindings and descriptions**

5) **Credit**

6) **Contact & Copyright**

-------------------------------------------------------------------------------


1) **Version**

1.00 : 10-6-2016
- First initial release. The library now operates independently from 
  Unity's event system.
	- SteamVR Plugin 1.1.1


-------------------------------------------------------------------------------


2) **Overview**

This support library takes the complex problem of processing I/O from the
SteamVR SDK and simplifies it. All I/O from the two Vive controllers that
come with the HTC Vive are turned into interface based event calls; when
a certain button action occurs (press down / press / press up), a respective
function is called on the gameObject. 

For example:

	- A controller is pointed at GameObject X.
	- X has Components that implement the interface IPointerTriggerHandler.
	- The controller's trigger is pressed down in the next frame.
	- The function IPointerTriggerHandler.OnTriggerPressDown() is called
          on all Components on that object that implement IPointerTriggerHandler.
	- A binding context is formed between the controller and GameObject X.
	- During all frames following, before the trigger is released,
          IPointerTriggerHandler.OnTriggerPress() is called.
	- The controller's trigger is released.
	- The function IPointerTriggerHandler.OnTriggerPressUp() is called.

A big benefit of dealing with I/O in this manner is no longer needing to
do index management. The runtime indexes of SteamVR controllers change
as controllers connect and disconnect, and this support library does away
with needing to remember in code those index changes.

Also, game programming can now occur on the object, instead of on the controller.


-------------------------------------------------------------------------------


3) **Guide**

This support library requires the SteamVR SDK for Unity.

All scripts that want to use this library should include:

using FRL.IO;

How to get set up:
 - Drag the [Camera Rig] Prefab library into the scene.
 - Apply the ViveControllerModule.cs Component to:
	- [Camera Rig] -> Controller (left)
	- [Camera Rig] -> Controller (right)
 - That's it! You're ready to start scripting gameObjects!

 
How to use the Vive Pointer interfaces (when the controller is pointing
at a gameObject):

 - The gameObject must have a Collider component for raycasting.
 - Use any set of sub interfaces of the IPointerViveHandler interface.
 - The functions will be called on the object _iff_, during the initial
   "down" of the button, the object is the closest legal raycast from
   the controller. After that, the gameObject will receive all following
   events for that context, even if it is no longer the closest legal raycast.

How to use the Vive Global interfaces (receiving input regardless):

 - The gameObject must have a GlobalReceiver component.
 - If the Module field is populated with a specific instance of ViveControllerModule,
   the gameObject will only receive input from that module. Otherwise, it will receive
   global input from all controller modules.
 - Use any set of sub interfaces of the IGlobalViveHandler interface.

How to test without an HTC Vive:

 - Attach a ViveControllerModule.cs to any gameObject (preferably the Camera).
 - Select the checkbox "TestInput".
 - The arrow keys will rotate the object around the x/y axes.
 - The following letter keys will emulate corresponding button calls:

	- A: AppMenu press
	- G: Grip press
	- P: Touchpad press
	- T: Trigger press
	- O: Touchpad touch
	- R: Trigger touch

-------------------------------------------------------------------------------


3.5) **EventData**

Every interface function receives a ViveControllerModule.EventData object with it.
The ViveControllerModule.EventData object has the following fields:

module : ViveControllerModule
- The module that called the function.

steamVRTrackedObject : SteamVR_TrackedObject
- The connected SteamVR_TrackedObject, if it exists.

currentRaycast : GameObject
- The gameObject currently hit by the raycast out of the controller.

previousRaycast : GameObject
- The gameObject previously hit by the raycast out of the controller.

touchpadAxis : Vector2
- The current Vector2 location of where the touchpad is being touched.

triggerAxis : Vector2
- The current Vector2 value of the trigger being touched or pressed

appMenuPress : GameObject
- The object bound by press context to the app menu.

gripPress : GameObject
- The object bound by press context to the grip.

touchpadPress : GameObject
- The object bound by press context to the touchpad.

triggerPress : GameObject
- The object bound by press context to the trigger.

touchpadTouch : GameObject
- The object bound by touch context to the touchpad.

triggerTouch : GameObject
- The object bound by touch context to the trigger.

worldNormal : Vector3
- The world normal of the current raycast, if it exists.

worldPosition: Vector3
The world position of the current raycast, if it exists.


-------------------------------------------------------------------------------


4) **Interface Bindings**

IViveHandler:
- IPointerViveHandler:
--- IPointerAppMenuHandler:
----- IPointerAppMenuPressDownHandler.OnPointerAppMenuPressDown()
----- IPointerAppMenuPressHandler.OnPointerAppMenuPress()
----- IPointerAppMenuPressUpHandler.OnPointerAppMenuPressUp()
--- IPointerGripHandler:
----- IPointerGripPressDownHandler.OnPointerGripPressDown()
----- IPointerGripPressHandler.OnPointerGripPress()
----- IPointerGripPressUpHandler.OnPointerGripPressUp()
--- IPointerTouchpadHandler:
----- IPointerTouchpadPressSetHandler:
------- IPointerTouchpadPressDownHandler.OnPointerTouchpadPressDown()
------- IPointerTouchpadPressHandler.OnPointerTouchpadPress()
------- IPointerTouchpadPressUpHandler.OnPointerTouchpadPressUp()
----- IPointerTouchpadTouchSetHandler:
------- IPointerTouchpadTouchDownHandler.OnPointerTouchpadTouchDown()
------- IPointerTouchpadTouchHandler.OnPointerTouchpadTouch()
------- IPointerTouchpadTouchUpHandler.OnPointerTouchpadTouchUp()
--- IPointerTriggerHandler:
----- IPointerTriggerPressSetHandler:
------- IPointerTriggerPressDownHandler.OnPointerTriggerPressDown()
------- IPointerTriggerPressHandler.OnPointerTriggerPress()
------- IPointerTriggerPressUpHandler.OnPointerTriggerPressUp()
----- IPointerTriggerTouchSetHandler:
------- IPointerTriggerTouchDownHandler.OnPointerTriggerTouchDown()
------- IPointerTriggerTouchHandler.OnPointerTriggerTouch()
------- IPointerTriggerTouchUpHandler.OnPointerTriggerTouchUp()
- IGlobalViveHandler:
--- IGlobalAppMenuHandler:
----- IGlobalAppMenuPressDownHandler.OnGlobalAppMenuPressDown()
----- IGlobalAppMenuPressHandler.OnGlobalAppMenuPress()
----- IGlobalAppMenuPressUpHandler.OnGlobalAppMenuPressUp()
--- IGlobalGripHandler:
----- IGlobalGripPressDownHandler.OnGlobalGripPressDown()
----- IGlobalGripPressHandler.OnGlobalGripPress()
----- IGlobalGripPressUpHandler.OnGlobalGripPressUp()
--- IGlobalTouchpadHandler:
----- IGlobalTouchpadPressSetHandler:
------- IGlobalTouchpadPressDownHandler.OnGlobalTouchpadPressDown()
------- IGlobalTouchpadPressHandler.OnGlobalTouchpadPress()
------- IGlobalTouchpadPressUpHandler.OnGlobalTouchpadPressUp()
----- IGlobalTouchpadTouchSetHandler:
------- IGlobalTouchpadTouchDownHandler.OnGlobalTouchpadTouchDown()
------- IGlobalTouchpadTouchHandler.OnGlobalTouchpadTouch()
------- IGlobalTouchpadTouchUpHandler.OnGlobalTouchpadTouchUp()
--- IGlobalTriggerHandler:
----- IGlobalTriggerPressSetHandler:
------- IGlobalTriggerPressDownHandler.OnGlobalTriggerPressDown()
------- IGlobalTriggerPressHandler.OnGlobalTriggerPress()
------- IGlobalTriggerPressUpHandler.OnGlobalTriggerPressUp()
----- IGlobalTriggerTouchSetHandler:
------- IGlobalTriggerTouchDownHandler.OnGlobalTriggerTouchDown()
------- IGlobalTriggerTouchHandler.OnGlobalTriggerTouch()
------- IGlobalTriggerTouchUpHandler.OnGlobalTriggerTouchUp()


-------------------------------------------------------------------------------


5) **Credit**

Thanks go out to New York University, Ken Perlin's Future Reality Lab, Ken
Perlin, David Lobser, Wenbo Lan, Fengyuan Zhu, Zhu Wang, Zhenyi He, Connor
Defanti, Zach Cimafonte, Evan Moore, Aaron Gaudette, Gene Miller, Dan Zhang,
and Scott Garner.

-------------------------------------------------------------------------------


6) **Contact & Copyright**

For any questions, please contact me at: herscher@nyu.edu

Copyright (c) 2016 Sebastian Herscher, Future Reality Lab @ NYU

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.



-------------------------------------------------------------------------------								