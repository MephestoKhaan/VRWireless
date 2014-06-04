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
public class OVRDeviceRouter : MonoBehaviour 
{
	// Imported functions from 
	// OVRPlugin.dll 	(PC)
	// OVRPlugin.bundle (OSX)
	// OVRPlugin.so 	(Linux, Android)
	
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
	
	static bool useImposter = false;
	OVRDevice device;
	OVRDeviceImposter imposter;
	
	// PUBLIC
	public float InitialPredictionTime{
		get{
			return useImposter ? imposter.InitialPredictionTime : device.InitialPredictionTime;
		}
	}
	public float InitialAccelGain{
		get{
			return useImposter ? imposter.InitialAccelGain : device.InitialAccelGain;
		}
	}
	
	// STATIC
	private static MessageList MsgList 							= new MessageList(0, 0, 0);
	private static bool  OVRInit 								= false;
	
	public static int    SensorCount{
		get{
			return useImposter ?  OVRDeviceImposter.SensorCount : OVRDevice.SensorCount;
		}
	}
	
	public static String DisplayDeviceName{
		get{
			return useImposter ? OVRDeviceImposter.DisplayDeviceName : OVRDevice.DisplayDeviceName;
		}
	}
	
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

	

	// * * * * * * * * * * * *
	// PUBLIC FUNCTIONS
	// * * * * * * * * * * * *
	
	// Inited - Check to see if system has been initialized
	public static bool IsInitialized()
	{
		//return OVRInit;
		return true;
	}
	
	// HMDPreset
	public static bool IsHMDPresent()
	{
		//return OVR_IsHMDPresent();
		return true;
	}

	// SensorPreset
	public static bool IsSensorPresent(int sensor)
	{
		//return OVR_IsSensorPresent(sensor);
		return true;
	}
	
	// GetOrientation
	public static bool GetOrientation(int sensor, ref Quaternion q)
	{
//		float w = 0, x = 0, y = 0, z = 0;
//
//        if (OVR_GetSensorOrientation(sensor, ref w, ref x, ref y, ref z) == true)
//		{
//			q.w =  w;		
//		
//			// Change the co-ordinate system from right-handed to Unity left-handed
//			/*
//			q.x =  x; 
//			q.y =  y;
//			q.z =  -z; 
//			q = Quaternion.Inverse(q);
//			*/
//		
//			// The following does the exact same conversion as above
//			q.x = -x; 
//			q.y = -y;
//			q.z =  z;	
//		
//			return true;
//		}
//		
//		return false;
		return true;
	}
	
	// GetPredictedOrientation
	public static bool GetPredictedOrientation(int sensor, ref Quaternion q)
	{
//		float w = 0, x = 0, y = 0, z = 0;
//
//        if (OVR_GetSensorPredictedOrientation(sensor, ref w, ref x, ref y, ref z) == true)
//		{
//
//			q.w =  w;		
//			q.x = -x; 
//			q.y = -y;
//			q.z =  z;	
//		
//			return true;
//		}
//		
//		return false;
		return true;

	}		
	
	// ResetOrientation
	public static bool ResetOrientation(int sensor)
	{
		return true;
        //return OVR_ResetSensorOrientation(sensor);
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
//		if ( (predictionTime > 0.0f) &&
//             (OVR_SetSensorPredictionTime(sensor, predictionTime) == true))
//		{
//			PredictionTime = predictionTime;
//			return true;
//		}
//		
//		return false;
		return true;
        //
	}
	
	// GetAccelGain
	public static float GetAccelGain(int sensor)
	{		
		return 1f;
        //return AccelGain;
	}

	// SetAccelGain
	public static bool SetAccelGain(int sensor, float accelGain)
	{
//		if ( (accelGain > 0.0f) &&
//             (OVR_SetSensorAccelGain(sensor, accelGain) == true))
//		{
//			AccelGain = accelGain;
//			return true;
//		}
//		
//		return false;
		return true;
        //
	}
	
	// GetDistortionCorrectionCoefficients
	public static bool GetDistortionCorrectionCoefficients(ref float k0, 
														   ref float k1, 
														   ref float k2, 
														   ref float k3)
	{
//		if(!OVRInit)
//			return false;
//		
//		k0 = DistK0;
//		k1 = DistK1;
//		k2 = DistK2;
//		k3 = DistK3;
//		
//		return true;
		return true;
        //
	}
	
	// SetDistortionCorrectionCoefficients
	public static bool SetDistortionCorrectionCoefficients(float k0, 
														   float k1, 
														   float k2, 
														   float k3)
	{
//		if(!OVRInit)
//			return false;
//		
//		DistK0 = k0;
//		DistK1 = k1;
//		DistK2 = k2;
//		DistK3 = k3;
//		
//		return true;
		return true;
        //
	}
	
	// GetPhysicalLensOffsets
	public static bool GetPhysicalLensOffsets(ref float lensOffsetLeft, 
											  ref float lensOffsetRight)
	{
//		if(!OVRInit)
//			return false;
//		
//		lensOffsetLeft  = LensOffsetLeft;
//		lensOffsetRight = LensOffsetRight;	
//		
//		return true;
		return true;
        //
	}
	
	// GetIPD
	public static bool GetIPD(ref float IPD)
	{
//		if(!OVRInit)
//			return false;
//
//		OVR_GetInterpupillaryDistance(ref IPD);
//		
//		return true;
		
		return true;
        //
	}
		
	// CalculateAspectRatio
	public static float CalculateAspectRatio()
	{
//		if(Application.isEditor)
//			return (Screen.width * 0.5f) / Screen.height;
//		else
//			return (HResolution * 0.5f) / VResolution;
		
		return 1f;
        //
	}
	
	// VerticalFOV
	// Compute Vertical FOV based on distance, distortion, etc.
    // Distance from vertical center to render vertical edge perceived through the lens.
    // This will be larger then normal screen size due to magnification & distortion.
	public static float VerticalFOV()
	{
//		if(!OVRInit)
//		{
//			return 90.0f;
//		}
//			
//    	float percievedHalfScreenDistance = (VScreenSize / 2) * DistortionScale();
//    	float VFov = Mathf.Rad2Deg * 2.0f * 
//			         Mathf.Atan(percievedHalfScreenDistance / EyeToScreenDistance);	
//		
//		return VFov;
		
		return 1f;
        //
	}
	
	// DistortionScale - Used to adjust size of shader based on 
	// shader K values to maximize screen size
	public static float DistortionScale()
	{
//		if(OVRInit)
//		{
//			float ds = 0.0f;
//		
//			// Compute distortion scale from DistortionFitX & DistortionFitY.
//    		// Fit value of 0.0 means "no fit".
//    		if ((Mathf.Abs(DistortionFitX) < 0.0001f) &&  (Math.Abs(DistortionFitY) < 0.0001f))
//    		{
//        		ds = 1.0f;
//    		}
//    		else
//    		{
//        		// Convert fit value to distortion-centered coordinates before fit radius
//        		// calculation.
//        		float stereoAspect = 0.5f * Screen.width / Screen.height;
//        		float dx           = (DistortionFitX * DistortionFitScale) - LensOffsetLeft;
//        		float dy           = (DistortionFitY * DistortionFitScale) / stereoAspect;
//        		float fitRadius    = Mathf.Sqrt(dx * dx + dy * dy);
//        		ds  			   = CalcScale(fitRadius);
//    		}	
//			
//			if(ds != 0.0f)
//				return ds;
//			
//		}
//		
//		return 1.0f; // no scale
		
		return 1f;
        //
	}
	
	// LatencyProcessInputs
    public static void ProcessLatencyInputs()
	{
		return;
        //OVR_ProcessLatencyInputs();
	}
	
	// LatencyProcessInputs
    public static bool DisplayLatencyScreenColor(ref byte r, ref byte g, ref byte b)
	{
        
		return true;
        //return OVR_DisplayLatencyScreenColor(ref r, ref g, ref b);
	}
	
	// LatencyGetResultsString
    public static System.IntPtr GetLatencyResultsString()
	{
        
		return System.IntPtr.Zero;
        //return OVR_GetLatencyResultsString();
	}
	
	// Computes scale that should be applied to the input render texture
    // before distortion to fit the result in the same screen size.
    // The 'fitRadius' parameter specifies the distance away from distortion center at
    // which the input and output coordinates will match, assuming [-1,1] range.
    public static float CalcScale(float fitRadius)
    {
//        float s = fitRadius;
//        // This should match distortion equation used in shader.
//        float ssq   = s * s;
//        float scale = s * (DistK0 + DistK1 * ssq + DistK2 * ssq * ssq + DistK3 * ssq * ssq * ssq);
//        return scale / fitRadius;
		return 1f;
    }
	
	// CalculatePhysicalLensOffsets - Used to offset perspective and distortion shift
	public static bool CalculatePhysicalLensOffsets(ref float leftOffset, ref float rightOffset)
	{
//		leftOffset  = 0.0f;
//		rightOffset = 0.0f;
//		
//		if(!OVRInit)
//			return false;
//		
//		float halfHSS = HScreenSize * 0.5f;
//		float halfLSD = LensSeparationDistance * 0.5f;
//		
//		leftOffset =  (((halfHSS - halfLSD) / halfHSS) * 2.0f) - 1.0f;
//		rightOffset = (( halfLSD / halfHSS) * 2.0f) - 1.0f;
//		
//		return true;
		return true;
        //
	}
	
	// MAG YAW-DRIFT CORRECTION FUNCTIONS
	
	// AUTO MAG CALIBRATION FUNCTIONS
	
	// BeginMagAutoCalibraton
	public static bool BeginMagAutoCalibration(int sensor)
	{
		//return OVR_BeginMagAutoCalibraton(sensor);
		return true;
	}
	
	// StopMagAutoCalibraton
	public static bool StopMagAutoCalibration(int sensor)
	{
		//return OVR_StopMagAutoCalibraton(sensor);
		return true;
	}
	
	// UpdateMagAutoCalibration
	public static bool UpdateMagAutoCalibration(int sensor)
	{
		//return OVR_UpdateMagAutoCalibration(sensor);
		return true;
	}
	
	// MANUAL MAG CALIBRATION FUNCTIONS
	
	// BeginMagManualCalibration
	public static bool BeginMagManualCalibration(int sensor)
	{
		//return OVR_BeginMagManualCalibration(sensor);
		return true;
	}
	
	// StopMagManualCalibration
	public static bool StopMagManualCalibration(int sensor)
	{
		//return OVR_StopMagManualCalibration(sensor);
		return true;
	}
	
	// UpdateMagManualCalibration
	public static bool UpdateMagManualCalibration(int sensor)
	{
		//return OVR_UpdateMagManualCalibration(sensor);
		return true;
	}
	
	// SHARED MAG CALIBRATION FUNCTIONS
	
	// MagNumberOfSamples
	// For AUTO mode: number returned is the current number of valid samples while looking around
	// For MANUAL mode:
	// 0 = Look Forward, 1 = Look Up, 2 = Look Left, 3 = Look Right, 4 = Calibration Complete
	public static int MagNumberOfSamples(int sensor)
	{
		//return OVR_MagNumberOfSamples(sensor);
		return 1;
	}
	
	// IsMagCalibrated
	public static bool IsMagCalibrated(int sensor)
	{
		//return OVR_IsMagCalibrated(sensor);
		
		return true;
	}
	
	// SetMagReference
	public static bool SetMagReference(int sensor)
	{
		//return OVR_SetMagReference(sensor);
		return true;
	}
	
	// EnableMagYawCorrection
	public static bool EnableMagYawCorrection(int sensor, bool enable)
	{
		//return OVR_EnableMagYawCorrection(sensor, enable);
		return true;
	}
	
	// IsMagYawCorrectionInProgress
	public static bool IsMagYawCorrectionInProgress(int sensor)
	{
		//return OVR_IsMagYawCorrectionInProgress(sensor);
		return true;
	}
	
	// GetYawErrorAngle (in degrees)
	public static float GetYawErrorAngle(int sensor)
	{
		//return OVR_GetYawErrorAngle(sensor) * 57.295779513f;
		
		return 0f;
	}
}
