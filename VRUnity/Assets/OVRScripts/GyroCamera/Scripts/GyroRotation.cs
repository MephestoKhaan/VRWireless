using UnityEngine;
using System.Collections;

public class GyroRotation : MonoBehaviour {

    // Gyroscope-controlled camera for iPhone & Android revised 2.26.12
    // Perry Hoberman <hoberman@bway.net>
	
    // Usage:
    // Attach this script to main camera.
	
    // Note: Unity Remote does not currently support gyroscope.
    //
    // This script uses three techniques to get the correct orientation out of the gyroscope attitude:
    // 1. creates a parent transform (camParent) and rotates it with eulerAngles
    // 2. for Android (Samsung Galaxy Nexus) only: remaps gyro.Attitude quaternion values from xyzw to wxyz (quatMap)
    // 3. multiplies attitude quaternion by quaternion quatMult
    // Also creates a grandparent (camGrandparent) which can be rotated with localEulerAngles.y
    // This node allows an arbitrary heading to be added to the gyroscope reading
    // so that the virtual camera can be facing any direction in the scene, no matter what the phone's heading

    static bool gyroBool;
    private Gyroscope gyro;
    private Quaternion quatMult;
    private Quaternion quatMap;
	
	public static Quaternion gyroQuat
	{
		get{
			EnsureInstance();
			return m_GyroQuat;
		}
	}
	private static Quaternion m_GyroQuat;
	
	public static Vector3 gyroEuler
	{
		get{
			EnsureInstance();
			return m_GyroEuler;
		}
	}
	private static Vector3 m_GyroEuler;
	
	public static GyroRotation instance;
	
	RemoteGyroscope rGyro;
	
	static void EnsureInstance()
	{
		if (instance) return;
		
		GameObject instanceGO = new GameObject("GryoRotation");
		instance = instanceGO.AddComponent<GyroRotation>();
	}
	
	public static void RotateHeading(float angle)
	{
		instance.transform.parent.parent.Rotate(Vector3.up, angle);
	}
	
    void Awake() {
		
		instance = this;
		
        // find the current parent of the camera's transform
        Transform currentParent = transform.parent;

        // instantiate a new transform
        GameObject camParent = new GameObject ("camParent"); 

        // match the transform to the camera position
        camParent.transform.position = transform.position; 

        // make the new transform the parent of the camera transform
        transform.parent = camParent.transform; 

        // make the original parent the grandparent of the camera transform
        //camParent.transform.parent = currentParent; 

        // instantiate a new transform
        GameObject camGrandparent = new GameObject ("camGrandParent"); 

        // match the transform to the camera position
        camGrandparent.transform.position = transform.position; 

        // make the new transform the parent of the camera transform
        camParent.transform.parent = camGrandparent.transform; 

        // make the original parent the grandparent of the camera transform
		camGrandparent.transform.parent = currentParent;

        // check whether device supports gyroscope
       //Removed Unity version Checks to support unity 4 - dustin.hagen
        gyroBool = SystemInfo.supportsGyroscope;
		
		camParent.transform.eulerAngles = new Vector3(90,90,0); 
		quatMult = new Quaternion(0,0,1,0);
		
        if (gyroBool) {
            gyro = Input.gyro;
            gyro.enabled = true;

            #if UNITY_IPHONE
                    camParent.transform.eulerAngles = new Vector3(90,90,0); 

                        if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
                           quatMult = new Quaternion(0,0,1,0); //**
                        } else if (Screen.orientation == ScreenOrientation.LandscapeRight) {
                           quatMult = new Quaternion(0,0,1,0); //**
                        } else if (Screen.orientation == ScreenOrientation.Portrait) {
                           quatMult = new Quaternion(0,0,1,0); //**
                        } else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
                           quatMult = new Quaternion(0,0,1,0); // Unable to build package on upsidedown
                        }
            #endif
            #if UNITY_ANDROID
                    camParent.transform.eulerAngles = new Vector3(90,90,0);
                        if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
                           quatMult = new Quaternion(0,0,1,0); //**
                        } else if (Screen.orientation == ScreenOrientation.LandscapeRight) {
                           quatMult = new Quaternion(0,0,1,0); //**
                        } else if (Screen.orientation == ScreenOrientation.Portrait) {
                           quatMult = new Quaternion(0,0,1,0); //**
                        } else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
                           quatMult = new Quaternion(0,0,1,0); // Unable to build package on upsidedown
                        }
            #endif
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        } else {
            #if UNITY_EDITOR
                //print("NO GYRO");
            #endif
        }
    }
 
    void Update () {
        if (gyroBool || Application.isEditor) { 
            #if UNITY_IPHONE
                 quatMap = gyro.attitude;
             #endif
 
            #if UNITY_ANDROID
			if(gyro != null)
			{
                 quatMap = new Quaternion(gyro.attitude.x,gyro.attitude.y,gyro.attitude.z,gyro.attitude.w);	
			}
            #endif 
			
#if UNITY_EDITOR
			if (rGyro == null){
				rGyro = GyroMote.gyro();
			}
			
			if (rGyro!=null && !started){
				OnStartedGyro();
				started = true;
				Debug.Log("Started");
			}
			
			if (rGyro != null){
				quatMap = rGyro.attitude;
			}
			else
			{

			}
#endif

            transform.localRotation = quatMap * quatMult;
			m_GyroQuat = transform.rotation;
			m_GyroEuler = transform.eulerAngles;
        }
    }
	
	bool started;
	
	public delegate void StartedGyro();
	public static StartedGyro startedGyro;
	static void OnStartedGyro(){if (startedGyro!=null){startedGyro();}}
}
