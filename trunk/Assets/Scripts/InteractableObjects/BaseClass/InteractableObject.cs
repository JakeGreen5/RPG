using UnityEngine;
using System.Collections;

public abstract class InteractableObject : MonoBehaviour {

	public abstract int InteractDistance
	{
		get;
	}

	public virtual void EnableAbilityToInteract()
	{
		UIManager.HUD.ShowInteractionUI(true);
	}

	public virtual void DisableAbilityToInteract()
	{
		UIManager.HUD.ShowInteractionUI(false);
	}
	
	public abstract void Interact();
}
