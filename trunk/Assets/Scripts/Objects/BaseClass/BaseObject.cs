using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class controls the base functionality for all gameObjects tagged with "Object"
/// </summary>
public class BaseObject : MonoBehaviour {

	// A list of nodes that can be used for pathfinding seek points when navigating to this object
	[HideInInspector] public List<ObjectNode> nodes;

	protected virtual void Awake()
	{
		nodes = new List<ObjectNode>(gameObject.GetComponentsInChildren<ObjectNode>());
	}
}
