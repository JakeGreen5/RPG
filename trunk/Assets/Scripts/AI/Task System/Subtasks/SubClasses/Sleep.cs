using UnityEngine;
using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Sleep : NeedSubtask {

	public override float FullfillmentRate 
	{ 
		get { return .2f; }
	}

	public override Type need
	{
		get 
		{
			return typeof(Rest);
		}
	}
	
	public Sleep() : base()
	{
	}

	public Sleep(string gameObjectName, string targetScene) : base(gameObjectName, targetScene)
	{
	}
	
	public override AIAnimation.Type GetAnimation()
	{
		return AIAnimation.Type.Sleep;
	}
}
