using UnityEngine;
using System.Collections;

public class PedometerInput : MonoBehaviour {
	
	const float STEP_SIZE = 0.05f;
	
	void Start()
	{
		GetComponent<SensorListener>().onStepEvent+=TakeAStep;
	}
	
	void OnDestroy()
	{
		GetComponent<SensorListener>().onStepEvent-=TakeAStep;
	}
	
	void TakeAStep(float accuracy)
	{
		OVRPlayerController.Step(STEP_SIZE, 0.25f);
	}
}
