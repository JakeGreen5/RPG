using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public abstract class Task {

	public bool hasStarted;

	// The minute of game time the task was started
	public float timeStarted;
	// The day of game time the task was started
	public float dayStarted;

	public delegate void TaskFinished(Task task);
	public event TaskFinished OnTaskFinished;

	public string name
	{
		get { return this.GetType().Name; }
	}

	public List<Subtask> subTasks = new List<Subtask>();
	public int currentSubTaskIndex = 0;

	public Subtask CurrentSubtask
	{
		get { return subTasks[currentSubTaskIndex]; }
	}
	
	public List<Scene> scenePath;
	
	BaseObject currentDestination;
	public BaseObject CurrentDestination
	{
		get
		{
			if (currentDestination == null)
			{
				if (Application.loadedLevelName == CurrentSubtask.targetScene)
				{
					FindCurrentDestination();
				}
				else
				{
					GameObject door = SceneNavigation.Instance.GetDoorToAdjacentScene(scenePath[0]);
					currentDestination = door.GetComponent<BaseObject>();
					
					if (currentDestination == null)
						Debug.LogWarning("Could not find BaseObject component on " + door.gameObject.name + ". It's likely that this door is missing the Object tag.");
				}
			}

			return currentDestination;
		}
	}

	public Task()
	{
	}
	
	public Task(string targetScene, string gameObjectName)
	{
	}

	public void Begin(string currentScene)
	{
		timeStarted = TimeManager.Instance.timeOfDayInMinutes;
		dayStarted = (float)TimeManager.Instance.dayOfTheWeek;
		hasStarted = true;

		CalculateScenePath(currentScene, CurrentSubtask.targetScene);
	}

	public virtual void ProceedToNextSubtask(bool showLogs = false)
	{
		ClearOccupyStatus();

		if (currentSubTaskIndex + 1 >= subTasks.Count)
		{
			// We finished this task
			if (OnTaskFinished != null) OnTaskFinished(this);
			return;
		}

		currentSubTaskIndex++;

		// Set the current destination to null if our next subtask is walk, since this maens we're headed to a new destination
		if (CurrentSubtask is Walk)
		{
			currentDestination = null;
		}

		CurrentSubtask.Begin();

		string currentScene = CurrentSubtask.targetScene;
		CalculateScenePath(currentScene, CurrentSubtask.targetScene);
	}
	
	public void CalculateScenePath(string currentScene, string targetScene)
	{
		scenePath = SceneNavigation.Instance.GetScenePathToScene(currentScene, targetScene);
	}

	/// <summary>
	/// Finds the current destination using the current subtask's gameObjectName variable
	/// </summary>
	public void FindCurrentDestination()
	{		
		// Find all objects with the name we are looking for
		List<BaseObject> potentialDestinations = new List<BaseObject>();
		foreach (BaseObject obj in Game.Instance.objectList)
		{
			if (obj.name == CurrentSubtask.gameObjectName)
			{
				potentialDestinations.Add(obj);
			}
		}
		
		// If we couldn't find an object with this name in the object list, try to find it using GameObject.Find as a last resort
		if (potentialDestinations.Count == 0)
		{
			Debug.Log ("Defaulting to Find by name");
			currentDestination = GameObject.Find(CurrentSubtask.gameObjectName).GetComponent<BaseObject>();
		}
		else
		{
			// Pick a random object with that name
			currentDestination = potentialDestinations[Random.Range(0, potentialDestinations.Count)];
		}
		
		// Print a warning that our destination is null					
		if (currentDestination == null)
			Debug.LogWarning("Could not find BaseObject component on " + CurrentSubtask.gameObjectName + ". It's likely that this object is missing the Object tag.");
	}

	// Sets the current destination to unoccupied if it had an animation position. This is used when leaving an object.
	private void ClearOccupyStatus()
	{
		if (currentDestination != null && currentDestination is InanimateObject)
		{
			AnimationPosition animationPosition = ((InanimateObject)CurrentDestination).animationPosition;
			if (animationPosition != null)
			{
				animationPosition.SetOccupant(null);
			}
		}
	}
}
