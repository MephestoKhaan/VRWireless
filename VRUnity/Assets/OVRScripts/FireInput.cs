using UnityEngine;
using System.Collections;

public class FireInput : MonoBehaviour {

	public GameObject gun;
	
	
	void Start()
	{
		GetComponent<SensorListener>().onGunModeEvent+=SetGunMode;
	}
	
	void OnDestroy()
	{
		GetComponent<SensorListener>().onGunModeEvent-=SetGunMode;
	}
	
	void SetGunMode(string mode)
	{
		mode = mode.ToLower();
		if(mode == "fire")
		{
			gun.SendMessage("OnStartFire");	
		}
		else if(mode == "stopfire")
		{
			gun.SendMessage("OnStopFire");	
		}
		else if(mode == "pump")
		{
			gun.SendMessage("OnFireGrenade");	
		}
		else if(mode == "clipoff")
		{
			gun.SendMessage("OnRemoveClip");	
		}
		else if(mode == "clipon")
		{
			gun.SendMessage("OnReload");	
		}
	}
	
}
