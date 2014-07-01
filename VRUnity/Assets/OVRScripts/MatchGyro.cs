using UnityEngine;
using System.Collections;

public class MatchGyro : MonoBehaviour {
	
	public Transform masterTransform;
	public HeadingInput heading;
	public GunInput gun;

	public bool match;
	
	void Start()
	{
		GetComponent<SensorListener>().onSyncEvent+=matchGyros;
	}
	
	void OnDestroy()
	{
		GetComponent<SensorListener>().onSyncEvent-=matchGyros;
	}
	
	void Update ()
	{
		if(match)
		{
			match = false;
			matchGyros();	
		}
	}
	
	private void matchGyros()
	{
		if(heading)
		{
			heading.SetBaseHeading(-masterTransform.eulerAngles.y + heading.headingAngle);
		}
		
		if(gun)
		{
			Vector3 correctedAngle = -gun.gunAngle;
			correctedAngle.y = -masterTransform.eulerAngles.y + gun.gunAngle.y;
			gun.SetBaseHeading(correctedAngle);	
		}
	}
}
