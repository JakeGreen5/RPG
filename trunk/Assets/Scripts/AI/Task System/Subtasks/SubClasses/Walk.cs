using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Walk : TimeSubtask {
	
	public float completionTime;
	public override float CompletionTime
	{
		get { return completionTime; }
	}	
	
	// The minute of game time we entered the scene we're in
	public float timeEnteredScene;
	// The day of game time we entered the scene we're in
	public float dayEnteredScene;

	public Walk() : base()
	{
	}

	public Walk(string gameObjectName, string targetScene) : base(gameObjectName, targetScene)
	{
	}

	public override AIAnimation.Type GetAnimation()
	{
		return AIAnimation.Type.Walk;
	}

	// Walking can take between min and max time.
	// This is for when the villager is not in the scene, since they could just actually walk to the object if they're in the scene.
	private void GenerateRandomWalkTime()
	{
		float min = 8.0f;
		float max = 15.0f;

		completionTime = Random.Range (min, max);
	}
	
	public override void Begin()
	{
		base.Begin();
		
		GenerateRandomWalkTime();
		EnteredNewScene();
	}

	public void EnteredNewScene()
	{
		timeEnteredScene = TimeManager.Instance.timeOfDayInMinutes;
		dayEnteredScene = (float)TimeManager.Instance.dayOfTheWeek;
	}
}
