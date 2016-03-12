using UnityEngine;
using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Eat : NeedSubtask {
	
	public override float FullfillmentRate 
	{ 
		get { return 2f; }
	}
	
	public override Type need
	{
		get 
		{
			return typeof(Food);
		}
	}
	
	public Eat() : base()
	{
	}

	public Eat(string gameObjectName, string targetScene) : base(gameObjectName, targetScene)
	{
	}

	public override AIAnimation.Type GetAnimation()
	{
		return AIAnimation.Type.Eat;
	}
}
