using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AIStateMachine : MonoBehaviour {

	AIManager manager;

	public enum State { Idle, Walking, PerformingTask }
	public State state = State.Idle;

	Task currentTask
	{
		get { return manager.data.CurrentTask; }
	}
	
	// Whether or not to display the priority list on the GUI
	public bool displayPriorityList;

	private Task storedTask;

	private Vector3 positionBeforeAnimation;
		
	void Start () 
	{
		manager = GetComponent<AIManager>();

		TimeManager.Instance.NextMinute += OneMinutePassed;
	}
	
	void OnDestroy()
	{
		if (!DataManager.ApplicationHasQuit)
		{
			TimeManager.Instance.NextMinute -= OneMinutePassed;
		}
	}
	
	// Displays the priority list in the top right corner of the screen
	void OnGUI()
	{
		if (manager.data.name == "Henry" || displayPriorityList)
		{			
			for (int i = 0; i < manager.data.TaskCount; i++)
			{
				Rect thisPriority = new Rect(Screen.width - 300, i * 30, 100, 30);
				GUI.Box(new Rect(Screen.width - 200, thisPriority.y, 100, 30), manager.data.tasks[i].name);
				GUI.Box(new Rect(Screen.width - 100, thisPriority.y, 100, 30), manager.data.tasks[i].CurrentDestination.name);
			}

			for (int j = 0; j < manager.data.needs.Count; j++)
			{
				Rect needRect = new Rect(Screen.width - 450, j * 30, 100, 30);
				GUI.Box(needRect, manager.data.needs[j].Score.ToString());
				
				Rect needNameRect = new Rect(Screen.width - 550, j * 30, 100, 30);
				GUI.Box(needNameRect, manager.data.needs[j].GetType().ToString());
			}
		}
	}

	void Update ()
	{
		StateMachine();
	}
	
	/// <summary>
	/// State machine.
	/// </summary>
	private void StateMachine()
	{
		if (currentTask != storedTask)
		{
			//Debug.Log ("Priority for " + manager.data.name + " changed to " + currentTask.name);
			storedTask = currentTask;
			StateIdle();
		}

		switch (state)
		{
		case State.Idle:

			if (currentTask == null) return;
			
			if (!currentTask.hasStarted)
				currentTask.Begin(manager.data.currentScene);
			
			// Go to your next priority
			if (manager.data.CurrentTask.CurrentSubtask is Walk)
			{
				StateWalk();
			}
			else
			{
				// This is an edge case. A villager is idle yet they aren't walking to the object for their task.
				// Insert a walk subtask at the top of their subtask queue so they walk to their task.
				// This can happen when a villager has been simulating a subtask outside of your scene and you walk into their scene.
				Subtask currentSubtask = currentTask.CurrentSubtask;
				currentTask.subTasks.Insert(0, new Walk(currentSubtask.gameObjectName, currentSubtask.targetScene));
			}
			break;
			
		case State.Walking:

			manager.movement.FollowPath();
			manager.movement.MoveToSeekPosition();
			
			// If we have reached the end of the path, go to work
			if ((manager.movement.destinationCount >= manager.movement.yellowBrickRoad.Count))
			{
				manager.movement.destination = null;
				manager.movement.destinationCount = 0;

				StatePerformTask();
			}
			
			break;
			
		case State.PerformingTask:

			SetPositionToAnimationPosition();

			// Check if we have finished this task
			CheckForTaskCompletion();

			break;
		}
	}

	/// <summary>
	/// If a task is complete, go to idle.
	/// </summary>
	void CheckForTaskCompletion()
	{
		// If the top priority is a door
		if (currentTask.CurrentDestination.GetComponent<DoorInteract>() != null)
		{
			Debug.Log (manager.data.name + " exited the scene. He wants to " + currentTask.name + ".");

			// Go through door
			manager.data.currentScene = currentTask.CurrentDestination.GetComponent<DoorInteract>().sceneToLoad.ToString();

			currentTask.scenePath.RemoveAt(0);
			((Walk)currentTask.CurrentSubtask).EnteredNewScene();

			Game.Instance.RemoveVillager(gameObject);
		}
		// If the top priority has a completion time
		else if (currentTask.CurrentSubtask is TimeSubtask)
		{
			TimeSubtask timeSubtask = (TimeSubtask)currentTask.CurrentSubtask;

			// If the subtask time has been going on longer than the completion time
			if (TimeManager.Instance.TimePassedSince(timeSubtask.dayStarted, timeSubtask.timeStarted) >= timeSubtask.CompletionTime)
			{
				manager.data.CurrentTask.ProceedToNextSubtask();
			}
		}
	}

	/// <summary>
	/// Every minute, update the need if we're performing a task that fulfills our need.
	/// </summary>
	void OneMinutePassed()
	{
		if (state == State.PerformingTask)
		{
			// Update the need that is being fulfilled if there is one
			if (currentTask.CurrentSubtask is NeedSubtask)
			{
				NeedSubtask needSubtask = ((NeedSubtask)currentTask.CurrentSubtask);
				Type typeOfNeed = needSubtask.need;
				
				Need need = null;
				foreach (Need n in manager.data.needs)
				{
					if (n.GetType() == typeOfNeed)
					{
						need = n;
						break;
					}
				}

				need.Score += needSubtask.FullfillmentRate;
				
				// Fulfull need
				if (need.Score >= 100)
				{
					if (manager.data.name == "Henry") Debug.Log (manager.data.name + " finished " + manager.data.CurrentTask.name + ".");
					
					manager.data.CurrentTask.ProceedToNextSubtask();
				}
			}
		}
	}

	// Go to the idle state
	public void StateIdle()
	{
		manager.animation.PlayAnimation(AIAnimation.Type.Idle);		
			
		// Reset the position to the saved position (if there is one, Vector3.zero means there isn't a saved position).
		// The saved position is set right before performing a task with an animation.
		if (positionBeforeAnimation != Vector3.zero)
		{
			transform.position = positionBeforeAnimation;
			positionBeforeAnimation = Vector3.zero;
		}

		state = State.Idle;
	}
	
	// Walk to your highest priority
	public void StateWalk()
	{
		manager.movement.SetPath(currentTask.CurrentDestination);
		
		state = State.Walking;
	}
	
	// Save the current time and go to work
	public void StatePerformTask()
	{
		// Store the priority and time
		storedTask = currentTask;
		currentTask.timeStarted = TimeManager.Instance.timeOfDayInMinutes;
		currentTask.dayStarted = (float)TimeManager.Instance.dayOfTheWeek;

		// If not a door
		if (currentTask.CurrentDestination.GetComponent<DoorInteract>() == null)
		{			
			manager.data.CurrentTask.ProceedToNextSubtask();
			
			// If the new subtask is to walk, simply go to that state
			if (currentTask.CurrentSubtask is Walk)
			{
				StateWalk();
				return;
			}
			else if (currentTask.CurrentDestination is InanimateObject)
			{
				AnimationPosition animationPosition = ((InanimateObject)currentTask.CurrentDestination).animationPosition;
				if (animationPosition != null)
				{
					// Is this place is occupied? (Someone is sitting in this chair, for example)
					if (animationPosition.IsOccupied())
					{
						// Find a new destination using the same gameObjectName, hopefully it'll find an object that isn't occupied
						currentTask.subTasks.Add(new Walk(currentTask.CurrentSubtask.gameObjectName, currentTask.CurrentSubtask.targetScene));
						currentTask.FindCurrentDestination();
					}
					else
					{
						animationPosition.SetOccupant(manager.data);
					}
				}
			}
		}
		
		// Store the position we started this task at so we can reset to this position when we're finished
		positionBeforeAnimation = transform.position;
		
		// Face the work object
		Vector3 directionToLook = (currentTask.CurrentDestination.transform.position - transform.position).normalized;
		directionToLook.y = 0;
		transform.forward = directionToLook;
		
		// Play the correct animation dependent upon the object we are working on
		manager.animation.PlayAnimation(currentTask.CurrentSubtask.GetAnimation());
		
		state = State.PerformingTask;
	}
	
	// Move to the object's animation position if it has one
	private void SetPositionToAnimationPosition()
	{
		if (currentTask.CurrentDestination is InanimateObject)
		{
			AnimationPosition animationPosition = ((InanimateObject)currentTask.CurrentDestination).animationPosition;
			if (animationPosition != null)
			{
				transform.forward = animationPosition.transform.forward;
				transform.position = animationPosition.transform.position;
			}
		}
	}
}
