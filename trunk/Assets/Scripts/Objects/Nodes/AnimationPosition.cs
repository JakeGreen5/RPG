using UnityEngine;
using System.Collections;

public class AnimationPosition : MonoBehaviour {

	AIData occupant;

	public void SetOccupant(AIData occupant)
	{
		this.occupant = occupant;
	}

	public bool IsOccupied()
	{
		if (occupant != null)
		{
			return true;
		}

		return false;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(transform.position, new Vector3(.3f, .1f, .3f));
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Vector3 forwardPosition = transform.forward * .2f + transform.position;
		Gizmos.DrawCube(forwardPosition, new Vector3(.15f, .1f, .15f));
	}
}
