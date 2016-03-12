using UnityEngine;
using System.Collections;

public static class Tools {

	public static GameObject Instantiate(string resourcePath)
	{
		return (GameObject)GameObject.Instantiate(Resources.Load(resourcePath));
	}
}
