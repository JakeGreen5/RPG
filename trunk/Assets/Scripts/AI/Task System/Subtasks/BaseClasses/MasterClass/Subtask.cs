using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public abstract class Subtask {

	public bool hasStarted;
	
	// The minute of game time the subtask was started
	public float timeStarted;
	// The day of game time the subtask was started
	public float dayStarted;

	public string gameObjectName;
	public string targetScene;
	
	public Subtask()
	{
	}

	public Subtask(string gameObjectName, string targetScene)
	{
		this.gameObjectName = gameObjectName;
		this.targetScene = targetScene;
	}

	public virtual void Begin()
	{
		hasStarted = true;
		timeStarted = TimeManager.Instance.timeOfDayInMinutes;
		dayStarted = (float)TimeManager.Instance.dayOfTheWeek;
	}

	public abstract AIAnimation.Type GetAnimation();
}
