using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrinkAtTavern : Task {

	public string targetScene;
	public string gameObjectName;

	public DrinkAtTavern() : base()
	{
	}

	public DrinkAtTavern(string targetScene, string gameObjectName) : base(targetScene, gameObjectName)
	{
		this.targetScene = targetScene;
		this.gameObjectName = gameObjectName;

		subTasks.Add(new Walk(gameObjectName, targetScene));
	}

	public override void ProceedToNextSubtask(bool showLogs = false)
	{
		GoToChair();

		base.ProceedToNextSubtask();
	}

	private void GoToChair()
	{
		if (subTasks.Count == 1)
		{
			if (Application.loadedLevelName == "Inn")
			{
				GameObject.Find("Bartender").GetComponent<Bartender>().Bartend();
			}
				
			string chair = "chair1";
			
			subTasks.Add(new Walk(chair, targetScene));
			subTasks.Add(new Drink(chair, targetScene));
		}
	}
}
