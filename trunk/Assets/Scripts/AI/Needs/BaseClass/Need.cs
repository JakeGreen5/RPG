using UnityEngine;
using System.Collections;

public abstract class Need {

	public float rateOfDecay;

	private float score = 100;
	public float Score
	{
		get
		{
			return score;
		}
		set
		{
			if (value > 101f) value = 100f;
			else if (value < 0f) value = 0f;
			
			score = value;
		}
	}

	public Need()
	{
	}

	public Need(float rateOfDecay)
	{
		this.rateOfDecay = rateOfDecay;
	}
}
