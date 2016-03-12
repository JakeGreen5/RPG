using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class handles the simuation of all AI that are not in the player's current scene
/// </summary>
public class AISimulation : Singleton<AISimulation> {

	List<AIData> dataList = new List<AIData>();

	public override void Awake()
	{
		base.Awake();

		TimeManager.Instance.NextMinute += OneMinutePassed;
	}

	/// <summary>
	/// Clears the all the AIData.
	/// </summary>
	public void ClearList()
	{
		dataList.Clear();
	}

	/// <summary>
	/// Adds a villager to the simulation array.
	/// </summary>
	/// <param name="villagerData">Villager data.</param>
	public void AddVillager(AIData villagerData)
	{
		if (!dataList.Contains(villagerData))
		{
			dataList.Add(villagerData);
		}
	}

	/// <summary>
	/// Removes a villager from the simulation array.
	/// </summary>
	/// <param name="villagerData">Villager data.</param>
	public void RemoveVillager(AIData villagerData)
	{
		if (dataList.Contains(villagerData))
		{
			dataList.Remove(villagerData);

			Game.Instance.AddVillager(villagerData, "SpawnPoint_" + villagerData.currentScene);
		}
	}

	/// <summary>
	/// Simulates every AI in the list
	/// </summary>
	void OneMinutePassed ()
	{
		// Iterate backwards so we can remove villagers if we want. -- Does this make sense? I'm not sure. It's been awhile since I wrote this.
		for (int i = dataList.Count - 1; i >= 0; i--)
		{
			AIData data = dataList[i];

			SimulateTask(data);
		}
	}

	void SimulateTask(AIData data)
	{
		Task currentTask = data.CurrentTask;

		if (currentTask == null) return;

		if (!currentTask.hasStarted)
			currentTask.Begin(data.currentScene);
		
		if (!currentTask.CurrentSubtask.hasStarted)
			currentTask.CurrentSubtask.Begin();

		// Is the task object not in this scene?
		if (currentTask.CurrentSubtask is Walk && currentTask.scenePath != null && currentTask.scenePath.Count > 0)
		{
			Walk walkTask = ((Walk)currentTask.CurrentSubtask);

			// Did we finish walking to the next scene?
			if (TimeManager.Instance.TimePassedSince(walkTask.dayEnteredScene, walkTask.timeEnteredScene) >= walkTask.CompletionTime)
			{
				Debug.Log (data.name + " reached the " + currentTask.scenePath[0].name + " scene. His task is " + data.CurrentTask.name);

				// Did we reach the player's scene?
				if (currentTask.scenePath[0].name == Application.loadedLevelName)
				{
					// Hand the logic over to the game
					RemoveVillager(data);
				}

				data.currentScene = currentTask.scenePath[0].name;
				currentTask.scenePath.RemoveAt(0);
				walkTask.EnteredNewScene();
			}
		}
		else
		{
			// Have we reached the game object to start the task?
			if (currentTask.CurrentSubtask is Walk)
			{
				Walk walkTask = ((Walk)currentTask.CurrentSubtask);

				if (TimeManager.Instance.TimePassedSince(walkTask.dayEnteredScene, walkTask.timeEnteredScene) >= walkTask.CompletionTime)
				{
					if (data.name == "Henry") Debug.Log (data.name + " reached the " + currentTask.CurrentSubtask.gameObjectName +  ".");
					currentTask.ProceedToNextSubtask();
				}
			}
			else
			{
				// Is this a task where we are fulfilling a need?
				if (currentTask.CurrentSubtask is NeedSubtask)
				{
					NeedSubtask needSubtask = ((NeedSubtask)currentTask.CurrentSubtask);
					Type typeOfNeed = needSubtask.need;

					Need need = null;
					foreach (Need n in data.needs)
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
						if (data.name == "Henry") Debug.Log (data.name + " finished " + currentTask.name + ".");

						if (data.name == "Henry") currentTask.ProceedToNextSubtask(true);
						else currentTask.ProceedToNextSubtask();
					}
				}
				// Is this a subtask that has a completion time?
				else if (currentTask.CurrentSubtask is TimeSubtask)
				{
					TimeSubtask timeSubtask = ((TimeSubtask)currentTask.CurrentSubtask);

					if (TimeManager.Instance.TimePassedSince(currentTask.CurrentSubtask.dayStarted, currentTask.CurrentSubtask.timeStarted) >= timeSubtask.CompletionTime)
					{
						Debug.Log (data.name + " finished " + currentTask.name + ".");

						currentTask.ProceedToNextSubtask();
					}
				}
			}
		}
	}
}
