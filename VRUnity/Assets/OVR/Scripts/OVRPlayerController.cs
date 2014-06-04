/************************************************************************************

Filename    :   OVRPlayerController.cs
Content     :   Player controller interface. 
				This script drives OVR camera as well as controls the locomotion
				of the player, and handles physical contact in the world.	
Created     :   January 8, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using Holoville.HOTween;

[RequireComponent(typeof(CharacterController))]

//-------------------------------------------------------------------------------------
// ***** OVRPlayerController
//
// OVRPlayerController implements a basic first person controller for the Rift. It is 
// attached to the OVRPlayerController prefab, which has an OVRCameraController attached
// to it. 
// 
// The controller will interact properly with a Unity scene, provided that the scene has
// collision assigned to it. 
//
// The OVRPlayerController prefab has an empty GameObject attached to it called 
// ForwardDirection. This game object contains the matrix which motor control bases it
// direction on. This game object should also house the body geometry which will be seen
// by the player.
//
public class OVRPlayerController : OVRComponent
{
	protected CharacterController 	Controller 		 = null;
	protected OVRCameraController 	CameraController = null;

	public float Acceleration 	   = 0.1f;
	public float Damping 		   = 0.65f;
	public float BackAndSideDampen = 0.5f;
	float JumpForce 		   = 2.3f;
	public float RotationAmount    = 1.5f;
	public float GravityModifier   = 0.379f;
		
	private float   MoveScale 	   = 1.0f;
	private Vector3 MoveThrottle   = Vector3.zero;
	private float   FallSpeed 	   = 0.0f;
	
	// Initial direction of controller (passed down into CameraController)
	private Quaternion OrientationOffset = Quaternion.identity;			
	// Rotation amount from inputs (passed down into CameraController)
	private float 	YRotation 	 = 0.0f;
	
	// Transfom used to point player in a given direction; 
	// We should attach objects to this if we want them to rotate 
	// separately from the head (i.e. the body)
	protected Transform DirXform = null;
	
	// We can adjust these to influence speed and rotation of player controller
	private float MoveScaleMultiplier     = 1.0f; 
	private float RotationScaleMultiplier = 1.0f; 
	
	//
	// STATIC VARIABLES
	//
	public static bool  AllowMouseRotation      = true;
 	
	public static OVRPlayerController Instance;
	
	public static void Step(float size, float duration)
	{
		if (!Instance) return;

		//Instance.stepMomentum = 0f;
		HOTween.Kill(Instance);
		Instance.stepDirection = Instance.DirXform.forward;
		HOTween.To(Instance, duration * 0.33f, new TweenParms().Prop("stepMomentum", size));
		HOTween.To(Instance, duration * 0.66f, new TweenParms().Prop("stepMomentum", 0f).Delay(duration));
	}
	
	public float stepMomentum = 0f;
	public Vector3 stepDirection = Vector3.forward;
	
	// * * * * * * * * * * * * *
	
	// Awake
	new public virtual void Awake()
	{
		base.Awake();
		
		Instance = this;
		
		// We use Controller to move player around
		Controller = gameObject.GetComponent<CharacterController>();
		
		if(Controller == null)
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");
					
		// We use OVRCameraController to set rotations to cameras, 
		// and to be influenced by rotation
		OVRCameraController[] CameraControllers;
		CameraControllers = gameObject.GetComponentsInChildren<OVRCameraController>();
		
		if(CameraControllers.Length == 0)
			Debug.LogWarning("OVRPlayerController: No OVRCameraController attached.");
		else if (CameraControllers.Length > 1)
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraController attached.");
		else
			CameraController = CameraControllers[0];	
	
		// Instantiate a Transform from the main game object (will be used to 
		// direct the motion of the PlayerController, as well as used to rotate
		// a visible body attached to the controller)
		DirXform = null;
		Transform[] Xforms = gameObject.GetComponentsInChildren<Transform>();
		
		for(int i = 0; i < Xforms.Length; i++)
		{
			if(Xforms[i].name == "ForwardDirection")
			{
				DirXform = Xforms[i];
				break;
			}
		}
		
		if(DirXform == null)
			Debug.LogWarning("OVRPlayerController: ForwardDirection game object not found. Do not use.");
	}

	// Start
	new public virtual void Start()
	{
		base.Start();
		
		InitializeInputs();	
		SetCameras();
	}
		
	// Update 
	new public virtual void Update()
	{
		base.Update();
		
		UpdateMovement();
		
		Vector3 moveDirection = Vector3.zero;
		
		float motorDamp = (1.0f + (Damping * DeltaTime));
		MoveThrottle.x /= motorDamp;
		MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
		MoveThrottle.z /= motorDamp;

		moveDirection += MoveThrottle * DeltaTime;
		
		// Gravity
		if (Controller.isGrounded && FallSpeed <= 0)
			FallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));	
		else
			FallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * DeltaTime);	

		moveDirection.y += FallSpeed * DeltaTime;

		// Offset correction for uneven ground
		float bumpUpOffset = 0.0f;
		
		if (Controller.isGrounded && MoveThrottle.y <= 0.001f)
		{
			bumpUpOffset = Mathf.Max(Controller.stepOffset, 
									 new Vector3(moveDirection.x, 0, moveDirection.z).magnitude); 
			moveDirection -= bumpUpOffset * Vector3.up;
		}			
	 
		Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), 
											 new Vector3(1, 0, 1));	
		
		//moveDirection.y=0;
		
		// Move contoller
		Controller.Move(moveDirection);
		
		Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));
		
		if (predictedXZ != actualXZ)
			MoveThrottle += (actualXZ - predictedXZ) / DeltaTime; 
		
		// Update rotation using CameraController transform, possibly proving some rules for 
		// sliding the rotation for a more natural movement and body visual
		UpdatePlayerForwardDirTransform();
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{

	}
		
	// UpdateMovement
	//
	// COnsolidate all movement code here
	//
	static float sDeltaRotationOld = 0.0f;
	public virtual void UpdateMovement()
	{
		// Do not apply input if we are showing a level selection display
		if(OVRMainMenu.sShowLevels == false)
		{
			MoveScale = 0.0f;
			MoveScale *= DeltaTime;
			
			// Compute this for key movement
			float moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier * 4f;
			
			MoveThrottle += DirXform.TransformDirection((Vector3.forward * moveInfluence)) + (stepMomentum * stepDirection);
			
			
			/////////
			float deltaRotation = 0.0f;
			if(AllowMouseRotation == false){}
				deltaRotation = Input.GetAxis("Mouse X") * 1.0f * 3.25f;
			
			float filteredDeltaRotation = (sDeltaRotationOld * 0.0f) + (deltaRotation * 1.0f);
			YRotation += filteredDeltaRotation;
			sDeltaRotationOld = filteredDeltaRotation;
			 
		}
		
		// Update cameras direction and rotation
		SetCameras();

	}
	

	// UpdatePlayerControllerRotation
	// This function will be used to 'slide' PlayerController rotation around based on 
	// CameraController. For now, we are simply copying the CameraController rotation into 
	// PlayerController, so that the PlayerController always faces the direction of the 
	// CameraController. When we add a body, this will change a bit..
	public virtual void UpdatePlayerForwardDirTransform()
	{
		if ((DirXform != null) && (CameraController != null)){
			CameraController.transform.localPosition = Controller.height * Vector3.up;
			//DirXform.rotation = CameraController.transform.rotation;
		}
	}
	
	///////////////////////////////////////////////////////////
	// PUBLIC FUNCTIONS
	///////////////////////////////////////////////////////////
	
	// Jump
	public bool Jump()
	{
		if (!Controller.isGrounded)
			return false;

		MoveThrottle += new Vector3(0, JumpForce, 0);

		return true;
	}

	// Stop
	public void Stop()
	{
		Controller.Move(Vector3.zero);
		MoveThrottle = Vector3.zero;
		FallSpeed = 0.0f;
	}	
	
	// InitializeInputs
	public void InitializeInputs()
	{
		// Get our start direction
		OrientationOffset = transform.rotation;
		// Make sure to set y rotation to 0 degrees
		YRotation = 0.0f;
	}
	
	// SetCameras
	public void SetCameras()
	{
		if(CameraController != null)
		{
			// Make sure to set the initial direction of the camera 
			// to match the game player direction
			CameraController.SetOrientationOffset(OrientationOffset);
			CameraController.SetYRotation(YRotation);
		}
	}
	
	// Get/SetMoveScaleMultiplier
	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = MoveScaleMultiplier;
	}
	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		MoveScaleMultiplier = moveScaleMultiplier;
	}
	
	// Get/SetRotationScaleMultiplier
	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = RotationScaleMultiplier;
	}
	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		RotationScaleMultiplier = rotationScaleMultiplier;
	}
}

