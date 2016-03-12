using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("PlayerDataCollection")]
public class PlayerDataFile
{	
	public string currentScene;

	// Used when walking through doors
	public string savedSpawnPoint;

	public bool savedGame;
	public Vector3 position;	
	public Quaternion rotation;
}