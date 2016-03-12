using UnityEngine;
using System.Collections;

public class RaycastManager : MonoBehaviour {

	// We use one long ray and determine what we hit based on the layermask
	int rayDistance = 100;
	
	// Set up a layerMask so the raycast only hits what we define in the Start method
	int layerMask;

	// Keeps track of what we hit with our raycast
	RaycastHit hit;
	
	[HideInInspector] public bool lookingAtInteractableObject;
	[HideInInspector] public InteractableObject objectWeAreLookingAt;
	[HideInInspector] public InteractableObject objectWeAreInteractingWith;
	
	GameObject crosshair;
	Camera hudCamera;

	void Start () 
	{
		crosshair = UIManager.HUD.crosshair;
		hudCamera = UIManager.HUD.hudCamera.GetComponent<Camera>();

		// Define what layers we want our raycast to hit
		layerMask = (1 << LayerMask.NameToLayer("Villager"))
					| (1 << LayerMask.NameToLayer("Door"));
	}

	void Update () 
	{
		Raycast ();

		// If you are looking at something and you press R, an interaction happens
		if (lookingAtInteractableObject && Input.GetKeyDown(KeyCode.R))
		{
			objectWeAreLookingAt.Interact();
			objectWeAreInteractingWith = objectWeAreLookingAt;
		}
	}
	
	// Raycasts into the scene in the direction the camera is facing
	void Raycast()
	{		
		// Gets a ray through the crosshair
		Ray ray = Camera.main.ScreenPointToRay(hudCamera.WorldToScreenPoint(crosshair.transform.position));
		
		// Draws converationCheck with distance drawTalkDistance
		//Debug.DrawRay(ray.origin, ray.direction * drawTalkDistance, Color.red);

		// Raycast on the given layermask to see if anything can be interacted with
		if (Physics.Raycast (ray, out hit, rayDistance, layerMask))
		{
			// Hey we hit something! What is it?
			GameObject objectWeHit = hit.collider.gameObject;

			// This object should have an interactable object script seeing as how its on a layer that can be hit by this raycast, so let's get the reference to it
			InteractableObject interactableObject = GetInteractableObjectScript(objectWeHit);
	
			// Is this object a different one than we hit the last time we checked?
			if (objectWeAreLookingAt == null || lookingAtInteractableObject && objectWeHit != objectWeAreLookingAt.gameObject)
			{
				// Yep, so let's disable the ability to interact with the last object
				DisableAbilityToInteract();
			}

			// At what distance did we hit this object with our ray?
			float distanceToInteractableObject = Vector3.Distance(hit.point, ray.origin);

			// Is that distance less than or equal to the distance we can interact with this object at?
			if (distanceToInteractableObject <= interactableObject.InteractDistance)
			{
				// Yes it is, so let's enable the ability to interact with this object
				interactableObject.EnableAbilityToInteract();

				// Save a reference to the object we are looking at
				objectWeAreLookingAt = interactableObject;
				
				lookingAtInteractableObject = true;
			}
			else
			{
				// No it isn't, so let's disable the ability to interact
				DisableAbilityToInteract();
			}
		}
		else
		{
			// The raycast didn't hit anything --- We're not looking at an interactable object

			// Were we already looking at an interactable object?
			if (lookingAtInteractableObject)
			{				
				// Yep, so let's disable the ability to interact
				DisableAbilityToInteract();
			}
		}
	}

	InteractableObject GetInteractableObjectScript(GameObject obj)
	{
		InteractableObject interactableObjectScript = obj.GetComponent<InteractableObject>();
		
		// Does the object have the InteractableObject script on it?
		if (interactableObjectScript == null)
		{
			// Nope, so let's throw an error
			throw new MissingComponentException (obj.name + " does not have the InteractableObject script, but is on a layer that can be interacted with");
		}

		return interactableObjectScript;
	}

	void DisableAbilityToInteract()
	{
		if (objectWeAreLookingAt == null)
		{
			UIManager.HUD.ShowInteractionUI(false);

			return;
		}

		objectWeAreLookingAt.DisableAbilityToInteract();
		
		// Clear the reference to the thing we're looking at
		objectWeAreLookingAt = null;
		
		lookingAtInteractableObject = false;
	}
}
