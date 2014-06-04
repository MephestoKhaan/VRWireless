using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class CrouchInput : MonoBehaviour {
	
	const float TWEEN_DURATION = 0.1f;
	
	public Transform holder;
	
	float maxHeight;
	public float minHeight;
	
	float standingRoll = 1f;
	
	public float gyroSensitivity = 2f;
	
	public float crouchLevel{
		get{
			return mCrouchLevel;
		}
		set{
			SetCrouch(value);
		}
	}
	
	float mCrouchLevel;
	
	float roll;
	
	static CrouchInput Instance;
	
	void Awake()
	{
		Instance = this;
		
		maxHeight  = holder.localPosition.y;
		SetCrouch(1f);
		
		GetComponent<SensorListener>().onRollEvent+=SetRoll;
	}
	
	void OnDestroy()
	{
		GetComponent<SensorListener>().onRollEvent-=SetRoll;
	}
	
	void SetRoll(float val)
	{
		roll = val;
	}
	
	void Update()
	{	
		
		float normalizedRoll = Mathf.PingPong(Mathf.Abs(roll), 90f)/90f;
		
		float inverse = 1f-normalizedRoll;		
		inverse*=gyroSensitivity;	
		
		normalizedRoll = 1f-inverse;
		normalizedRoll = Mathf.Clamp01(normalizedRoll);
		
		SetCrouch(normalizedRoll);
		
	}
	
	void SetCrouch(float crouchAmount)
	{
		if (!holder) return;
		
		float normalizedCrouchAmount = crouchAmount/standingRoll;
		
		float height = Mathf.Lerp(minHeight, maxHeight, normalizedCrouchAmount);
		
		Vector3 position = holder.localPosition;
		position.y = height;
		holder.localPosition = position;
		
		
		mCrouchLevel = crouchAmount;
	}
}
