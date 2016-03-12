using UnityEngine;
using System.Collections;

public class SleepAtHome : Task {

	public SleepAtHome() : base()
	{
	}

	public SleepAtHome(string targetScene, string gameObjectName) : base(targetScene, gameObjectName)
	{
		subTasks.Add(new Walk(gameObjectName, targetScene));
		subTasks.Add(new Sleep(gameObjectName, targetScene));
	}
}
