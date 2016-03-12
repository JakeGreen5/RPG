using UnityEngine;
using System.Collections;

public class Player : Agent {

	[HideInInspector] public RaycastManager raycastManager;

	protected override void Awake()
	{
		base.Awake();

		raycastManager = GetComponent<RaycastManager>();
	}
}
