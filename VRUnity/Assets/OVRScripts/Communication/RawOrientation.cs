using UnityEngine;
using System.Collections;

public class RawOrientation : MonoBehaviour {

	void Start()
	{
		(FindObjectOfType(typeof(SensorListener)) as SensorListener).onHeadingEvent+=SetRotation;
	}
	
	void SetRotation(float yAngle)
	{
		transform.eulerAngles = new Vector3(0f, yAngle, 0f);
	}
}
