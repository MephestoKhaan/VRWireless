using UnityEngine;
using System.Collections;

public class HeadingInput : MonoBehaviour {
	
	public Transform baseTransform;
	public Transform headingTransform;
	public Transform eyesTransform;
	
	public float headingAngle;
	float baseHeadingAngle;

	public bool bindToHead;
	
	void Start()
	{
		GetComponent<SensorListener>().onHeadingEvent+=SetHeadingAngle;
		GetComponent<SensorListener>().onViewModeEvent+=SetViewMode;
	}
	
	void OnDestroy()
	{
		GetComponent<SensorListener>().onHeadingEvent-=SetHeadingAngle;
		GetComponent<SensorListener>().onViewModeEvent-=SetViewMode;
	}
	
	void SetViewMode(string viewMode)
	{
		bindToHead = (viewMode == "eyes");	
	}
	
	void SetHeadingAngle(float angle)
	{
		headingAngle = angle;
	}
	
	public void SetBaseHeading(float angle)
	{
		baseHeadingAngle = angle;
	}
	
	void Update()
	{
		
		if(bindToHead)
		{
			SetBaseHeading(-eyesTransform.eulerAngles.y + headingAngle);
		}
		
		Vector3 angle = Vector3.up * (headingAngle - baseHeadingAngle);
		headingTransform.eulerAngles = angle;
		
	}
}
