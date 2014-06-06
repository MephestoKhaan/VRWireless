VRWireless
==========
## This project is a full Wireless VR experience!

I created it in the middle of this VR Boom we are living to try to solve most of the problems that I find in some famous systems like the Oculus Rift: constraints on the movements.
The core project was written during HackManchester 2013 in 25 hours and polished later for Mancheste's Android Meetup, ence the photos and presentations that can be found in the SHOWCASE folder.


Components:
- Wireless helmet using an android device running a modified version of Unity3D's Angry Bots powered by a tweaked Oculus Rift SDK (at this moment, original OR SDK only works on PC)
- A toy gun modified using an Arduino board + Android phone to track its position, trigger and ammo clip state.
- A phone using a custom pedometer and other sensors to track user's movement.


In DataStreamers you can find the Android code that will run on the movement phone and the Gun phone as well.
In VRGun there is the simple code, based on original's Makey Makey code to "digitalise" the plastic Gun and send the events to the Gun-phone
In VRUnity is the modified game that will work as a server for the data streamers and track the head movements while displaying stereoscopic graphics in a 1st person perspective for the game Angry Bots

The .apks for this projects can be found in the respective folders so there is no need to re-compile the project (only the Arduino board).
