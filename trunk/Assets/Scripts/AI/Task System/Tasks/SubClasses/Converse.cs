using UnityEngine;
using System.Collections;

public class Converse : Task {

	public Converse() : base()
	{
	}

	public Converse(string targetScene, string gameObjectName) : base(targetScene, gameObjectName)
	{
		subTasks.Add(new Walk(gameObjectName, targetScene));
		subTasks.Add(new Talk(gameObjectName, targetScene));
	}
}
