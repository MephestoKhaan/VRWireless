using UnityEngine;
using System.Collections;

public class SensorListener : MonoBehaviour
{

	
	public void ParseOrientation(string data)
	{
		string[] orientationSplit = data.Split(';');
		float roll = float.Parse(orientationSplit[0]);
		float heading = float.Parse(orientationSplit[1]);
		SendRollEvent(roll);
		SendHeadingEvent(heading);
	}
	
	
	public void ParseGunOrientation(string data)
	{
		string[] orientationSplit = data.Split(';');
		float roll = float.Parse(orientationSplit[0]);
		float heading = float.Parse(orientationSplit[1]);
		float yaw = float.Parse(orientationSplit[2]);
		SendGunHeadingEvent(new Vector3(roll, heading, yaw));
	}
	
	public void ParseGunMode(string data)
	{
		SendGunModeEvent(data);	
	}
	
	public void ParseStep(string data)
	{
		float stepAmount = float.Parse(data);
		SendStepEvent(stepAmount);
	}
	
	public void ParseSync()
	{
		SendSyncEvent();
	}
	
	public void ParseViewMode(string data)
	{
		SendViewModeEvent(data);	
	}
	
	public delegate void VoidDelegate();
	public delegate void Vector3Delegate(Vector3 vec3);
	public delegate void FloatDelegate(float val);
	public delegate void StringDelegate(string val);
	public delegate void QuaternionDelegate(Quaternion quat);
	
	public FloatDelegate onStepEvent;
	public void SendStepEvent(float val){if (onStepEvent!=null){onStepEvent(val);}}	
		
	public FloatDelegate onRollEvent;
	public void SendRollEvent(float val){if (onRollEvent!=null){onRollEvent(val);}}	
	
	public FloatDelegate onHeadingEvent;
	public void SendHeadingEvent(float val){if (onHeadingEvent!=null){onHeadingEvent(val);}}
	
	public Vector3Delegate onGunHeadingEvent;
	public void SendGunHeadingEvent(Vector3 angle){if (onGunHeadingEvent!=null){onGunHeadingEvent(angle);}}
	
	public StringDelegate onGunModeEvent;
	public void SendGunModeEvent(string mode){if (onGunModeEvent!=null){onGunModeEvent(mode);}}
	
	public VoidDelegate onSyncEvent;
	public void SendSyncEvent(){if (onSyncEvent!=null){onSyncEvent();}}
	
	public StringDelegate onViewModeEvent;
	public void SendViewModeEvent(string mode){if (onViewModeEvent!=null){onViewModeEvent(mode);}}
	
}
