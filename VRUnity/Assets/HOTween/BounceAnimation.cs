using UnityEngine;
using System.Collections;
using Holoville.HOTween;

/** BounceAnimation
 *
 * Hidden utility script for bouncing animation
 */
public class BounceAnimation : MonoBehaviour
{
	public GameObject objectToBounce; 						//!< The GameObject to Bounce, Assigned in the inspector
	public float bouceDuration = 0.3f; 						//!< The duration for the up part of the bouce
	public Vector3 bounceAmount = new Vector3(-5f, 0f, 0f); //!< The magnitude of the bouce in x, y, z in local space relative to the initial position
	public bool shouldRepeat = false; 						//!< Indicates if the animation should repeat
	
	//! Method Calls StartBounce
	/*!
	 \sa StartBounce
	 */
	void Awake()
	{
		StartBounce();
	}
	
	//! Method to start bounceing hotween on objectToBounce
	/*!

	 */
	Tweener StartBounce()
	{
		if (objectToBounce!=null)
		{
			if(shouldRepeat)
				return HOTween.To(objectToBounce.transform, bouceDuration, new TweenParms().Loops(-1, LoopType.Yoyo).Prop("localPosition", objectToBounce.transform.localPosition+bounceAmount));
			else
				return HOTween.To(objectToBounce.transform, bouceDuration, new TweenParms().Loops(1, LoopType.Yoyo).Prop("localPosition", objectToBounce.transform.localPosition+bounceAmount));
		}
		else 
			return null;
	}
	
}
