using UnityEngine;
using System.Collections;

#pragma warning disable 0414

public class ThirdPersonMovement : MonoBehaviour 
{
	Animator anim;

	const string VERTICAL = "Vertical";
	const string HORIZONTAL = "Horizontal";
	const string MOVEMENT_JUMP = "Jump";
	const string MOVEMENT_RUN = "Run";
	const string MOVEMENT_WALK = "Walk";
	const string MOVEMENT_WALK_BACK = "WalkBack";

	enum DirectionState
	{
		Forward = 0,
		Right = 1,
		Left = 2,
		Backward = 3,
		ForwardAndRight = 4,
		ForwardAndLeft = 5,
		BackwardAndRight = 6,
		BackwardAndLeft = 7
	}
	DirectionState directionState;
	
	public float walkSpeed;
	public float runSpeed;
    public float jumpSpeed;
    public float gravity;
	
	Transform cameraTransform;
	
	Vector3 moveDirection;
	float speed;
	float v;
	float h;
	
	// Use this for initialization
	void Start () 
	{
		cameraTransform = Camera.main.transform;
		anim = GetComponent<Animator>();
		directionState = DirectionState.Forward;
	}
	
	// Update is called once per frame
	void Update () 
	{		
		// Get the player input
		v = Input.GetAxisRaw(VERTICAL);
		h = Input.GetAxisRaw(HORIZONTAL);
		
		CalculateMoveDirection();
	}

	void CalculateMoveDirection()
	{
		DetermineRotation();

		// Jumping
		if (Input.GetKeyDown(KeyCode.Space))
		{
			anim.SetBool(MOVEMENT_JUMP, true);
		}
		else
		{
			anim.SetBool(MOVEMENT_JUMP, false);
		}

		// Locomotion
		bool walkBack = v < 0;
		bool walkForward = v > 0 || h != 0;
		bool runForward = (Input.GetKey (KeyCode.LeftShift) && v > 0) || (Input.GetKey (KeyCode.LeftShift) && h != 0);		
		if (runForward)
		{
			anim.SetBool(MOVEMENT_RUN, true);
			anim.SetBool(MOVEMENT_WALK, false);
			anim.SetBool(MOVEMENT_WALK_BACK, false);
		}
		else if (walkBack)
		{			
			anim.SetBool(MOVEMENT_RUN, false);
			anim.SetBool(MOVEMENT_WALK, false);
			anim.SetBool(MOVEMENT_WALK_BACK, true);
		}
		else if (walkForward)
		{
			anim.SetBool(MOVEMENT_RUN, false);
			anim.SetBool(MOVEMENT_WALK, true);
			anim.SetBool(MOVEMENT_WALK_BACK, false);
		}
		else // idle
		{
			anim.SetBool(MOVEMENT_RUN, false);
			anim.SetBool(MOVEMENT_WALK, false);
			anim.SetBool(MOVEMENT_WALK_BACK, false);
		}
	}

	// Rotate the player to face the camera if they're moving
	void DetermineRotation()
	{	
		if (v > 0 && h > 0)
		{
			directionState = DirectionState.ForwardAndRight;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y + 45, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, v + h);
		}
		else if (v > 0 && h < 0)
		{
			directionState = DirectionState.ForwardAndLeft;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y - 45, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, v - h);
		}
		else if (v < 0 && h > 0)
		{			
			directionState = DirectionState.BackwardAndRight;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y - 45, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, v - h);
		}
		else if (v < 0 && h < 0)
		{
			directionState = DirectionState.BackwardAndLeft;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y + 45, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, v + h);
		}
		else if (h > 0)
		{
			directionState = DirectionState.Right;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y + 90, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, h);
		}
		else if (h < 0)
		{
			directionState = DirectionState.Left;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y - 90, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, -h);
		}
		else if (v > 0)
		{
			directionState = DirectionState.Forward;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, v);
		}
		else if (v < 0)
		{
			directionState = DirectionState.Backward;
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cameraTransform.localEulerAngles.y, transform.localEulerAngles.z);
			moveDirection = new Vector3(0, moveDirection.y, v);
		}
		else
		{
			moveDirection = new Vector3(0, moveDirection.y, 0);
		}
	}
}
