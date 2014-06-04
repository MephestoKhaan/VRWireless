using UnityEngine;
using System.Collections;

public class MayaTypeConstraint : MonoBehaviour {
	
	Transform thisTransform;
	public Transform targetTransform;
	public MayaTypeConstraintType constraintType;

	// Use this for initialization
	void Awake()
	{
		thisTransform = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (constraintType == MayaTypeConstraintType.Aim && targetTransform == null)
		{
			if(Camera.main != null)
				targetTransform = Camera.main.transform;
			else return;
		}
		
		if (targetTransform==null) return;
		
		switch (constraintType)
		{
		case MayaTypeConstraintType.PointOrient:
			thisTransform.position = targetTransform.position;
			thisTransform.rotation = targetTransform.rotation;
			break;
		case MayaTypeConstraintType.Orient:
			thisTransform.rotation = targetTransform.rotation;
			break;
		case MayaTypeConstraintType.Point:
			thisTransform.position = targetTransform.position;
			break;
		case MayaTypeConstraintType.Aim:
			Vector3 position = targetTransform.position;
			position.y = thisTransform.position.y;
			thisTransform.LookAt(position, Vector3.up);
			break;
		default:
			break;
			
		}
	}
}

public enum MayaTypeConstraintType
{
	PointOrient,
	Point,
	Orient,
	Aim
}