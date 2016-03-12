using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AIStateMachine))]
[RequireComponent(typeof(AIAnimation))]
[RequireComponent(typeof(AIConversation))]
[RequireComponent(typeof(AIMovement))]

/// <summary>
/// Holds references to all AI scripts
/// </summary>
public class AIManager : Agent {

	[HideInInspector] public AIData data;

	[HideInInspector] public new AIAnimation animation;
	[HideInInspector] public AIConversation conversation;
	[HideInInspector] public AIMovement movement;
	[HideInInspector] public AIStateMachine stateMachine;

	protected override void Awake ()
	{
		base.Awake();

		animation = GetComponent<AIAnimation>();
		conversation = GetComponent<AIConversation>();
		movement = GetComponent<AIMovement>();
		stateMachine = GetComponent<AIStateMachine>();
	}
}
