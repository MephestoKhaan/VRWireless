/************************************************************************************

Filename    :   OVRDevice.cs
Content     :   Interface for the Oculus Rift Device
Created     :   February 14, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;
using System;
using System.Runtime.InteropServices;

//-------------------------------------------------------------------------------------
// ***** OVRDevice
//
// OVRDevice is the main interface to the Oculus Rift hardware. It includes wrapper functions
// for  all exported C++ functions, as well as helper functions that use the stored Oculus
// variables to help set up camera behavior.
//
// This component is added to the OVRCameraController prefab. It can be part of any 
// game object that one sees fit to place it. However, it should only be declared once,
// since there are public members that allow for tweaking certain Rift values in the
// Unity inspector.
//
public class OVRDeviceImposter : MonoBehaviour 
{
	// Imported functions from 
	// OVRPlugin.dll 	(PC)
	// OVRPlugin.bundle (OSX)
	// OVRPlugin.so 	(Linux, Android)
	
	const int RESOLUTION_X = 1280;
	const int RESOLUTION_Y = 800;
	const float SIZE_X = 0.14976f;
	const float SIZE_Y = 0.0936f;
	const float EYE_SCREEN_DIST = 0.041f;
	const float LENS_SEPARATION = 0.0635f;
	const float LEFT_EYE_OFFSET = 0.032f;
	const float RIGHT_EYE_OFFSET = 0.032f;
	const float VERTICAL_CENTER = 0.0468f;
	const float DIST_0 = 1f;
	const float DIST_1 = 0.22f;
	const float DIST_2 = 0.24f;
	const float DIST_3 = 0f;
	
	// MessageList
	[StructLayout(LayoutKind.Sequential)]
	public struct MessageList
	{
		public byte isHMDSensorAttached;
		public byte isHMDAttached;
		public byte isLatencyTesterAttached;
		
		public MessageList(byte HMDSensor, byte HMD, byte LatencyTester)
		{
			isHMDSensorAttached = HMDSensor;
			isHMDAttached = HMD;
			isLatencyTesterAttached = LatencyTester;
		}
	}
	
	// GLOBAL FUNCTIONS
	[DllImport ("OculusPlugin", EntryPoint="OVR_Update")]
	private static extern bool OVR_Update(ref MessageList messageList);	
	
	//[DllImport ("OculusPlugin")]	
	private static bool OVR_Initialize(){
		return true;//SystemInfo.supportsGyroscope;
	}
	
	//[DllImport ("OculusPlugin")]
	private static bool OVR_Destroy(){return true;}
	
	// SENSOR FUNCTIONS
	//[DllImport ("OculusPlugin")]
    private static int OVR_GetSensorCount(){return 1;}
	
	//[DllImport ("OculusPlugin")]
	private static bool OVR_IsHMDPresent(){return true;}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_GetSensorOrientation(int sensorID, 
														ref float w, 
														ref float x, 
														ref float y, 
														ref float z)
	{
		Quaternion attitude = GyroRotation.gyroQuat;
		w = attitude.w;
		x = -attitude.x;
		y = -attitude.y;
		z = attitude.z;
		
		return true;
	}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_GetSensorPredictedOrientation(int sensorID, 
															     ref float w, 
																 ref float x, 
																 ref float y, 
																 ref float z)
	{
		Quaternion attitude = GyroRotation.gyroQuat;
		w = attitude.w;
		x = -attitude.x;
		y = -attitude.y;
		z = attitude.z;
		
		return true;
	}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_GetSensorPredictionTime(int sensorID, ref float predictionTime){
		predictionTime = 0f;
		return true;
	}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_SetSensorPredictionTime(int sensorID, float predictionTime){return true;}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_GetSensorAccelGain(int sensorID, ref float accelGain){return true;}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_SetSensorAccelGain(int sensorID, float accelGain){return true;}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_EnableYawCorrection(int sensorID, float enable){return true;}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_ResetSensorOrientation(int sensorID){return true;}
	
	// HMD FUNCTIONS
	//[DllImport ("OculusPlugin")]
	private static bool OVR_IsSensorPresent(int sensor){return true;}//SystemInfo.supportsGyroscope;}
	//[DllImport ("OculusPlugin")]
	private static System.IntPtr OVR_GetDisplayDeviceName(){return System.IntPtr.Zero;} 
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetScreenResolution(ref int hResolution, ref int vResolution){
		hResolution = RESOLUTION_X;
		vResolution = RESOLUTION_Y;
		return true;
	}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetScreenSize(ref float hSize, ref float vSize){
		hSize = SIZE_X;
		vSize = SIZE_Y;
		return true;
	}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetEyeToScreenDistance(ref float eyeToScreenDistance){
		eyeToScreenDistance = EYE_SCREEN_DIST;
		return true;
	}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetInterpupillaryDistance(ref float interpupillaryDistance){
		interpupillaryDistance = LENS_SEPARATION;
		return true;
	}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetLensSeparationDistance(ref float lensSeparationDistance){
		lensSeparationDistance = LENS_SEPARATION;
		return true;
	}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetEyeOffset(ref float leftEye, ref float rightEye){
		leftEye = LEFT_EYE_OFFSET;
		rightEye = RIGHT_EYE_OFFSET;
		return true;
	}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetScreenVCenter(ref float vCenter){
		vCenter = VERTICAL_CENTER;
		return true;
	}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_GetDistortionCoefficients(ref float k0, 
														     ref float k1, 
															 ref float k2, 
															 ref float k3)
	{
		k0 = DIST_0;
		k1 = DIST_1;
		k2 = DIST_2;
		k3 = DIST_3;
		return true;
	}
	
	// LATENCY TEST FUNCTIONS
	//[DllImport ("OculusPlugin")]
    private static void OVR_ProcessLatencyInputs(){return;}
	//[DllImport ("OculusPlugin")]
    private static bool OVR_DisplayLatencyScreenColor(ref byte r, 
															ref byte g, 
															ref byte b)
	{
		return false;
	}
	//[DllImport ("OculusPlugin")]
    private static System.IntPtr OVR_GetLatencyResultsString(){return System.IntPtr.Zero;}
	
	
	// MAGNETOMETER YAW-DRIFT CORRECTION FUNCTIONS
	// AUTOMATIC
	//[DllImport ("OculusPlugin")]
	private new static bool OVR_BeginMagAutoCalibraton(int sensor){return true;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_StopMagAutoCalibraton(int sensor){return true;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_UpdateMagAutoCalibration(int sensor){return true;}
	// MANUAL
	//[DllImport ("OculusPlugin")]
	private static bool OVR_BeginMagManualCalibration(int sensor){return true;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_StopMagManualCalibration(int sensor){return true;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_UpdateMagManualCalibration(int sensor){return true;}
	// SHARED
	//[DllImport ("OculusPlugin")]
	private static int OVR_MagNumberOfSamples(int sensor){return 0;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_IsMagCalibrated(int sensor){return true;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_SetMagReference(int sensor){return true;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_EnableMagYawCorrection(int sensor, bool enable){return true;}
	//[DllImport ("OculusPlugin")]
	private static bool OVR_IsMagYawCorrectionInProgress(int sensor){return true;}
	//[DllImport ("OculusPlugin")]
	private static float OVR_GetYawErrorAngle(int sensor){return 0f;}
	
	
	// PUBLIC
	public float InitialPredictionTime 							= 0.05f; // 50 ms
	public float InitialAccelGain  								= 0.05f; // default value
	
	// STATIC
	private static MessageList MsgList 							= new MessageList(0, 0, 0);
	private static bool  OVRInit 								= false;
	
	public static int    SensorCount 					 	    = 0;
	
	public static String DisplayDeviceName;
	
	public static int    HResolution, VResolution 				= 0;	 // pixels
	public static float  HScreenSize, VScreenSize 				= 0.0f;	 // meters
	public static float  EyeToScreenDistance  					= 0.0f;  // meters
	public static float  LensSeparationDistance 				= 0.0f;  // meters
	public static float  LeftEyeOffset, RightEyeOffset			= 0.0f;  // meters
	public static float  ScreenVCenter 							= 0.0f;	 // meters 
	public static float  DistK0, DistK1, DistK2, DistK3 		= 0.0f;
	
	// The physical offset of the lenses, used for shifting both IPD and lens distortion
	private static float LensOffsetLeft, LensOffsetRight   		= 0.0f;
	
	// Fit to top of the image (default is 5" display)
    private static float DistortionFitX 						= 0.0f;
    private static float DistortionFitY 						= 1.0f;
	
	// Copied from initialized public variables set in editor
	private static float PredictionTime 						= 0.0f;
	private static float AccelGain 								= 0.0f;
	
	// Used to reduce the size of render distortion and give better fidelity
	// Accessed with a public static function
	private static float  DistortionFitScale 					= 0.7f;  // Optimized for DK1 (7")
	
	
	// * * * * * * * * * * * * *

	// Awake
	void Awake () 
	{	
		OVRInit = OVR_Initialize();
		
		if(OVRInit == false) 
			return;

        SensorCount = OVR_GetSensorCount();
		
		// * * * * * * *
		// DISPLAY SETUP
		
		// We will get the HMD so that we can eventually target it within Unity
		DisplayDeviceName += Marshal.PtrToStringAnsi(OVR_GetDisplayDeviceName());
		
		OVR_GetScreenResolution (ref HResolution, ref VResolution);
		OVR_GetScreenSize (ref HScreenSize, ref VScreenSize);
		OVR_GetEyeToScreenDistance(ref EyeToScreenDistance);
		OVR_GetLensSeparationDistance(ref LensSeparationDistance);
		OVR_GetEyeOffset (ref LeftEyeOffset, ref RightEyeOffset);
		OVR_GetScreenVCenter (ref ScreenVCenter);
		OVR_GetDistortionCoefficients( ref DistK0, ref DistK1, ref DistK2, ref DistK3);
	
		// Distortion fit parameters based on if we are using a 5" (Prototype, DK2+) or 7" (DK1) 
		if (HScreenSize < 0.140f) 	// 5.5"
		{
			DistortionFitX = 0.0f;
			DistortionFitY = 1.0f;
			
			// Don't shrink as much (5.5" has denser pixels)
			DistortionFitScale = 1.0f;
		}
    	else 						// 7"
		{
			DistortionFitX = -1.0f;
			DistortionFitY =  0.0f;
		}
		
		// Calculate the lens offsets for each eye and store 
		CalculatePhysicalLensOffsets(ref LensOffsetLeft, ref LensOffsetRight);
		
		// * * * * * * *
		// SENSOR SETUP
		
		// PredictionTime set, to init sensor directly
		if(PredictionTime > 0.0f)
            OVR_SetSensorPredictionTime(0, PredictionTime);
		else
			SetPredictionTime(0, InitialPredictionTime);	
		
		// AcelGain set, used to correct gyro with accel. 
		// Default value is appropriate for typical use.
		if(AccelGain > 0.0f)
            OVR_SetSensorAccelGain(0, AccelGain);
		else
			SetAccelGain(0, InitialAccelGain);	
	}
   
	// Start (Note: make sure to always have a Start function for classes that have
	// editors attached to them)
	void Start()
	{
	}
	
	// Update
	// We can detect if our devices have been plugged or unplugged, as well as
	// run things that need to be updated in our game thread
	void Update()
	{	
		MessageList oldMsgList = MsgList;
		OVR_Update(ref MsgList);
		
		// HMD SENSOR
		if((MsgList.isHMDSensorAttached != 0) && 
		   (oldMsgList.isHMDSensorAttached == 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMDSensor, true); 
			//Debug.Log("HMD SENSOR ATTACHED");
		}
		else if((MsgList.isHMDSensorAttached == 0) && 
		   (oldMsgList.isHMDSensorAttached != 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMDSensor, false);
			//Debug.Log("HMD SENSOR DETACHED");
		}

		// HMD
		if((MsgList.isHMDAttached != 0) && 
		   (oldMsgList.isHMDAttached == 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMD, true); 
			//Debug.Log("HMD ATTACHED");
		}
		else if((MsgList.isHMDAttached == 0) && 
		   (oldMsgList.isHMDAttached != 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.HMD, false); 
			//Debug.Log("HMD DETACHED");
		}

		// LATENCY TESTER
		if((MsgList.isLatencyTesterAttached != 0) && 
		   (oldMsgList.isLatencyTesterAttached == 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.LatencyTester, true); 
			//Debug.Log("LATENCY TESTER ATTACHED");
		}
		else if((MsgList.isLatencyTesterAttached == 0) && 
		   (oldMsgList.isLatencyTesterAttached != 0))
		{
			OVRMessenger.Broadcast<OVRMainMenu.Device, bool>("Sensor_Attached", OVRMainMenu.Device.LatencyTester, false); 
			//Debug.Log("LATENCY TESTER DETACHED");
		}
	}
		
	// OnDestroy
	void OnDestroy()
	{
		OVR_Destroy();
		OVRInit = false;
	}
	
	
	// * * * * * * * * * * * *
	// PUBLIC FUNCTIONS
	// * * * * * * * * * * * *
	
	// Inited - Check to see if system has been initialized
	public static bool IsInitialized()
	{
		return OVRInit;
	}
	
	// HMDPreset
	public static bool IsHMDPresent()
	{
		return OVR_IsHMDPresent();
	}

	// SensorPreset
	public static bool IsSensorPresent(int sensor)
	{
		return OVR_IsSensorPresent(sensor);
	}
	
	// GetOrientation
	public static bool GetOrientation(int sensor, ref Quaternion q)
	{
		float w = 0, x = 0, y = 0, z = 0;

        if (OVR_GetSensorOrientation(sensor, ref w, ref x, ref y, ref z) == true)
		{
			q.w =  w;		
		
			// Change the co-ordinate system from right-handed to Unity left-handed
			/*
			q.x =  x; 
			q.y =  y;
			q.z =  -z; 
			q = Quaternion.Inverse(q);
			*/
		
			// The following does the exact same conversion as above
			q.x = -x; 
			q.y = -y;
			q.z =  z;	
		
			return true;
		}
		
		return false;
	}
	
	// GetPredictedOrientation
	public static bool GetPredictedOrientation(int sensor, ref Quaternion q)
	{
		float w = 0, x = 0, y = 0, z = 0;

        if (OVR_GetSensorPredictedOrientation(sensor, ref w, ref x, ref y, ref z) == true)
		{

			q.w =  w;		
			q.x = -x; 
			q.y = -y;
			q.z =  z;	
		
			return true;
		}
		
		return false;

	}		
	
	// ResetOrientation
	public static bool ResetOrientation(int sensor)
	{
        return OVR_ResetSensorOrientation(sensor);
	}
	
	// GetPredictionTime
	public static float GetPredictionTime(int sensor)
	{		
		// return OVRSensorsGetPredictionTime(sensor, ref predictonTime);
		return PredictionTime;
	}

	// SetPredictionTime
	public static bool SetPredictionTime(int sensor, float predictionTime)
	{
		if ( (predictionTime > 0.0f) &&
             (OVR_SetSensorPredictionTime(sensor, predictionTime) == true))
		{
			PredictionTime = predictionTime;
			return true;
		}
		
		return false;
	}
	
	// GetAccelGain
	public static float GetAccelGain(int sensor)
	{		
		return AccelGain;
	}

	// SetAccelGain
	public static bool SetAccelGain(int sensor, float accelGain)
	{
		if ( (accelGain > 0.0f) &&
             (OVR_SetSensorAccelGain(sensor, accelGain) == true))
		{
			AccelGain = accelGain;
			return true;
		}
		
		return false;
	}
	
	// GetDistortionCorrectionCoefficients
	public static bool GetDistortionCorrectionCoefficients(ref float k0, 
														   ref float k1, 
														   ref float k2, 
														   ref float k3)
	{
		if(!OVRInit)
			return false;
		
		k0 = DistK0;
		k1 = DistK1;
		k2 = DistK2;
		k3 = DistK3;
		
		return true;
	}
	
	// SetDistortionCorrectionCoefficients
	public static bool SetDistortionCorrectionCoefficients(float k0, 
														   float k1, 
														   float k2, 
														   float k3)
	{
		if(!OVRInit)
			return false;
		
		DistK0 = k0;
		DistK1 = k1;
		DistK2 = k2;
		DistK3 = k3;
		
		return true;
	}
	
	// GetPhysicalLensOffsets
	public static bool GetPhysicalLensOffsets(ref float lensOffsetLeft, 
											  ref float lensOffsetRight)
	{
		if(!OVRInit)
			return false;
		
		lensOffsetLeft  = LensOffsetLeft;
		lensOffsetRight = LensOffsetRight;	
		
		return true;
	}
	
	// GetIPD
	public static bool GetIPD(ref float IPD)
	{
		if(!OVRInit)
			return false;

		OVR_GetInterpupillaryDistance(ref IPD);
		
		return true;
	}
		
	// CalculateAspectRatio
	public static float CalculateAspectRatio()
	{
		if(Application.isEditor)
			return (Screen.width * 0.5f) / Screen.height;
		else
			return (HResolution * 0.5f) / VResolution;
	}
	
	// VerticalFOV
	// Compute Vertical FOV based on distance, distortion, etc.
    // Distance from vertical center to render vertical edge perceived through the lens.
    // This will be larger then normal screen size due to magnification & distortion.
	public static float VerticalFOV()
	{
		if(!OVRInit)
		{
			return 90.0f;
		}
			
    	float percievedHalfScreenDistance = (VScreenSize / 2) * DistortionScale();
    	float VFov = Mathf.Rad2Deg * 2.0f * 
			         Mathf.Atan(percievedHalfScreenDistance / EyeToScreenDistance);	
		
		return VFov;
	}
	
	// DistortionScale - Used to adjust size of shader based on 
	// shader K values to maximize screen size
	public static float DistortionScale()
	{
		if(OVRInit)
		{
			float ds = 0.0f;
		
			// Compute distortion scale from DistortionFitX & DistortionFitY.
    		// Fit value of 0.0 means "no fit".
    		if ((Mathf.Abs(DistortionFitX) < 0.0001f) &&  (Math.Abs(DistortionFitY) < 0.0001f))
    		{
        		ds = 1.0f;
    		}
    		else
    		{
        		// Convert fit value to distortion-centered coordinates before fit radius
        		// calculation.
        		float stereoAspect = 0.5f * Screen.width / Screen.height;
        		float dx           = (DistortionFitX * DistortionFitScale) - LensOffsetLeft;
        		float dy           = (DistortionFitY * DistortionFitScale) / stereoAspect;
        		float fitRadius    = Mathf.Sqrt(dx * dx + dy * dy);
        		ds  			   = CalcScale(fitRadius);
    		}	
			
			if(ds != 0.0f)
				return ds;
			
		}
		
		return 1.0f; // no scale
	}
	
	// LatencyProcessInputs
    public static void ProcessLatencyInputs()
	{
        OVR_ProcessLatencyInputs();
	}
	
	// LatencyProcessInputs
    public static bool DisplayLatencyScreenColor(ref byte r, ref byte g, ref byte b)
	{
        return OVR_DisplayLatencyScreenColor(ref r, ref g, ref b);
	}
	
	// LatencyGetResultsString
    public static System.IntPtr GetLatencyResultsString()
	{
        return OVR_GetLatencyResultsString();
	}
	
	// Computes scale that should be applied to the input render texture
    // before distortion to fit the result in the same screen size.
    // The 'fitRadius' parameter specifies the distance away from distortion center at
    // which the input and output coordinates will match, assuming [-1,1] range.
    public static float CalcScale(float fitRadius)
    {
        float s = fitRadius;
        // This should match distortion equation used in shader.
        float ssq   = s * s;
        float scale = s * (DistK0 + DistK1 * ssq + DistK2 * ssq * ssq + DistK3 * ssq * ssq * ssq);
        return scale / fitRadius;
    }
	
	// CalculatePhysicalLensOffsets - Used to offset perspective and distortion shift
	public static bool CalculatePhysicalLensOffsets(ref float leftOffset, ref float rightOffset)
	{
		leftOffset  = 0.0f;
		rightOffset = 0.0f;
		
		if(!OVRInit)
			return false;
		
		float halfHSS = HScreenSize * 0.5f;
		float halfLSD = LensSeparationDistance * 0.5f;
		
		leftOffset =  (((halfHSS - halfLSD) / halfHSS) * 2.0f) - 1.0f;
		rightOffset = (( halfLSD / halfHSS) * 2.0f) - 1.0f;
		
		return true;
	}
	
	// MAG YAW-DRIFT CORRECTION FUNCTIONS
	
	// AUTO MAG CALIBRATION FUNCTIONS
	
	// BeginMagAutoCalibraton
	public static bool BeginMagAutoCalibration(int sensor)
	{
		return OVR_BeginMagAutoCalibraton(sensor);
	}
	
	// StopMagAutoCalibraton
	public static bool StopMagAutoCalibration(int sensor)
	{
		return OVR_StopMagAutoCalibraton(sensor);
	}
	
	// UpdateMagAutoCalibration
	public static bool UpdateMagAutoCalibration(int sensor)
	{
		return OVR_UpdateMagAutoCalibration(sensor);
	}
	
	// MANUAL MAG CALIBRATION FUNCTIONS
	
	// BeginMagManualCalibration
	public static bool BeginMagManualCalibration(int sensor)
	{
		return OVR_BeginMagManualCalibration(sensor);
	}
	
	// StopMagManualCalibration
	public static bool StopMagManualCalibration(int sensor)
	{
		return OVR_StopMagManualCalibration(sensor);
	}
	
	// UpdateMagManualCalibration
	public static bool UpdateMagManualCalibration(int sensor)
	{
		return OVR_UpdateMagManualCalibration(sensor);
	}
	
	// SHARED MAG CALIBRATION FUNCTIONS
	
	// MagNumberOfSamples
	// For AUTO mode: number returned is the current number of valid samples while looking around
	// For MANUAL mode:
	// 0 = Look Forward, 1 = Look Up, 2 = Look Left, 3 = Look Right, 4 = Calibration Complete
	public static int MagNumberOfSamples(int sensor)
	{
		return OVR_MagNumberOfSamples(sensor);
	}
	
	// IsMagCalibrated
	public static bool IsMagCalibrated(int sensor)
	{
		return OVR_IsMagCalibrated(sensor);
	}
	
	// SetMagReference
	public static bool SetMagReference(int sensor)
	{
		return OVR_SetMagReference(sensor);
	}
	
	// EnableMagYawCorrection
	public static bool EnableMagYawCorrection(int sensor, bool enable)
	{
		return OVR_EnableMagYawCorrection(sensor, enable);
	}
	
	// IsMagYawCorrectionInProgress
	public static bool IsMagYawCorrectionInProgress(int sensor)
	{
		return OVR_IsMagYawCorrectionInProgress(sensor);
	}
	
	// GetYawErrorAngle (in degrees)
	public static float GetYawErrorAngle(int sensor)
	{
		return OVR_GetYawErrorAngle(sensor) * 57.295779513f;
	}
}
