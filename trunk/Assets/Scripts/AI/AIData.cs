using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

#region XML Includes
[XmlInclude(typeof(DrinkAtTavern))]
[XmlInclude(typeof(Converse))]
[XmlInclude(typeof(EatAtHome))]
[XmlInclude(typeof(SleepAtHome))]
[XmlInclude(typeof(Walk))]
[XmlInclude(typeof(Talk))]
[XmlInclude(typeof(Eat))]
[XmlInclude(typeof(Sleep))]
[XmlInclude(typeof(Drink))]
#endregion

[XmlRoot ("AIData")]
public class AIData {
	
	[XmlAttribute("name")]
	public string name;
	public string currentScene;
	public AIProperty property;
	public List<Need> needs;
	public List<Task> tasks;
	public Task CurrentTask
	{
		get 
		{
			if (tasks.Count > 0)
				return tasks[0];
			else
				return null;
		}
	}
	
	public int TaskCount
	{
		get
		{
			return tasks.Count;
		}
	}
	
	const float hungerPerMinute = .1f;
	const float fatiguePerMinute = .03f;

	public AIData()
	{
		TimeManager.Instance.NextMinute += OneMinutePassed;
	}

	public AIData(string startScene, string name, AIProperty property)
	{
		this.currentScene = startScene;
		this.name = name;
		this.property = property;

		AddNeeds();
		
		tasks = new List<Task>();
		AddDefaultTask();

		UpdatePriorities();

		TimeManager.Instance.NextMinute += OneMinutePassed;
	}

	void AddNeeds()
	{
		needs = new List<Need>();
		needs.Add(new Food(hungerPerMinute));
		needs.Add(new Rest(fatiguePerMinute));
		if (name == "Henry") Debug.Log ("Added needs");
		
		foreach (Need need in needs)
		{
			need.Score = UnityEngine.Random.Range(0, 100);
		}
	}

	/// <summary>
	/// Removes all event handlers. This is used when this class is destroyed to avoid memory leaks.
	/// </summary>
	public void RemoveEventHandlers()
	{		
		TimeManager.Instance.NextMinute -= OneMinutePassed;

		foreach (Task task in tasks)
		{
			task.OnTaskFinished -= HandleOnTaskFinished;
		}
	}

	#region Tasks
	public void AddTask(Task task)
	{
		if (CurrentTask is DrinkAtTavern)
		{
			RemoveTask(CurrentTask);
		}

		task.OnTaskFinished += HandleOnTaskFinished;
		tasks.Add(task);
		if (name == "Henry") Debug.Log (name + " is adding " + task.name + " to his list");
	}

	public void RemoveTask(Task task)
	{
		task.OnTaskFinished -= HandleOnTaskFinished;
		tasks.Remove(task);
		if (name == "Henry") Debug.Log (name + " is removing " + task.name + " from his list");

		if (!(task is DrinkAtTavern) && tasks.Count == 0)
		{
			AddDefaultTask();
		}
	}

	void HandleOnTaskFinished (Task task)
	{
		if (name == "Henry") Debug.Log (name + " finished " + task.name);
		RemoveTask(task);
	}
	
	void AddDefaultTask()
	{			
		AddTask(new DrinkAtTavern("Inn", "Bar"));
		CurrentTask.Begin(currentScene);
	}
	#endregion

	#region Needs
	/// <summary>
	/// Check the needs every minute.
	/// </summary>
	void OneMinutePassed ()
	{
		UpdatePriorities();
	}

	/// <summary>
	/// Updates the priority list
	/// </summary>
	void UpdatePriorities()
	{
		foreach (Need need in needs)
		{
			//if (name == "Henry") Debug.Log ("Update Priority " + need + " / " + need.Score);
			need.Score -= need.rateOfDecay;
			
			if (need.Score < 50)
			{
				Task task = null;
				if (need is Food)
				{
					task = new EatAtHome(property.houseScene, property.diningChairName);
				}
				else if (need is Rest)
				{
					task = new SleepAtHome(property.houseScene, property.bedName);
				}
				else
				{
					Debug.Log ("Warning! Your need type isn't being parsed correctly");
				}
				
				bool isTaskInList = false;
				foreach (Task taskInList in tasks)
				{
					if (taskInList.GetType() == task.GetType())
					{
						isTaskInList = true;
						break;
					}
				}
				
				if (!isTaskInList)
				{
					AddTask(task);
				}
			}
		}
	}
	#endregion
}
