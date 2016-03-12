using UnityEngine;
using System.Collections;

public class Drink : InfiniteSubtask {
	
	public Drink() : base()
	{
	}
	
	public Drink(string gameObjectName, string targetScene) : base(gameObjectName, targetScene)
	{
	}

	public override AIAnimation.Type GetAnimation()
	{
		return AIAnimation.Type.Drink;
	}
}
