using UnityEngine;
using System.Collections;

public class GunInput : MonoBehaviour {
	
	public Transform gunTransform;
	public Transform gunOrigin;
	
	public Vector3 gunAngle;
	
	Vector3 baseHeadingAngle = Vector3.zero;

	
	void Start()
	{
		GetComponent<SensorListener>().onGunHeadingEvent+=SetGunAngle;
	}
	
	void OnDestroy()
	{
		GetComponent<SensorListener>().onGunHeadingEvent-=SetGunAngle;
	}
	
	void SetGunAngle(Vector3 angle)
	{
		gunAngle = angle;
	}
	
	public void SetBaseHeading(Vector3 angle)
	{
		baseHeadingAngle = angle;
	}
	
	void Update()
	{
		Vector3 angle =  (gunAngle - baseHeadingAngle);
		gunTransform.eulerAngles = new Vector3(angle.x, gunTransform.eulerAngles.y, angle.z);
		gunOrigin.eulerAngles = new Vector3(gunOrigin.eulerAngles.x, angle.y, gunOrigin.eulerAngles.z);
		
	}
	
}
