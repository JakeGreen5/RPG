using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InanimateObject : BaseObject {
	
	// A list of tiles under this object
	[HideInInspector] public List<Tile> tilesOccupied = new List<Tile>();
	[HideInInspector] public AnimationPosition animationPosition;

	protected override void Awake()
	{
		base.Awake();		
		
		// Get the tiles under this object and simulataneously set them to unwalkable
		tilesOccupied = Pathfinding.Instance.SetWalkabilityOfTilesUnderObject(gameObject, false);

		animationPosition = transform.GetComponentInChildren<AnimationPosition>();
	}

	/// <summary>
	/// Called when an AI interacts with this object.
	/// </summary>
	public void Interact()
	{

	}
}
