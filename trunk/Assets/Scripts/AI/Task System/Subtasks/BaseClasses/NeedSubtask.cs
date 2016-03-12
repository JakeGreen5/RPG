using UnityEngine;
using System;
using System.Collections;

public abstract class NeedSubtask : Subtask {	
	
	public abstract float FullfillmentRate
	{
		get;
	}

	public abstract Type need
	{
		get;
	}
	
	public NeedSubtask() : base()
	{
	}
	
	public NeedSubtask(string obj, string scene) : base(obj, scene)
	{
	}
}
