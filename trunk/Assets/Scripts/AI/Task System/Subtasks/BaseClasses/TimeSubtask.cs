using UnityEngine;
using System.Collections;

public abstract class TimeSubtask : Subtask {

	public abstract float CompletionTime
	{
		get;
	}
	
	public TimeSubtask() : base()
	{
	}
	
	public TimeSubtask(string obj, string scene) : base(obj, scene)
	{
	}
}
