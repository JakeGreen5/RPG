using UnityEngine;
using System.Collections;

public class EatAtHome : Task {

	public EatAtHome() : base()
	{
	}
	
	public EatAtHome(string targetScene, string gameObjectName) : base(targetScene, gameObjectName)
	{
		subTasks.Add(new Walk(gameObjectName, targetScene));
		subTasks.Add(new Eat(gameObjectName, targetScene));
	}
}
