using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Talk : Subtask {
	
	public Talk() : base()
	{
	}

	public Talk(string gameObjectName, string targetScene) : base(gameObjectName, targetScene)
	{

	}
	public override AIAnimation.Type GetAnimation()
	{
		// TODO Replace with Talk animation
		return AIAnimation.Type.Idle;
	}
}
