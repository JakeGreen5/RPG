using UnityEngine;
using System.Collections;

public class AIProperty {

	public string houseScene;
	
	public string bedName;
	public string diningChairName;

	public AIProperty()
	{
	}

	public AIProperty (int houseNumber, int bedNumber)
	{
		houseScene = "House" + houseNumber;
		bedName = "Bed" + bedNumber;
		diningChairName = "DiningChair" + bedNumber;
	}
}
