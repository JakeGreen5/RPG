using UnityEngine;
using System.Collections;

public class DoorInteract : InteractableObject {

	public override int InteractDistance 
	{ 
		get { return 6; }
	}
	
	public enum Scene
	{
		Village,
		Inn,
		House1,
		House2,
		House3,
		House4,
		House5,
		House6,
		House7,
		House8
	}

	public Scene sceneToLoad;

	public string spawnPoint;

	/// <summary>
	/// Called when a player opens a door.
	/// Switches the current scene of the player to the scene that's on the other side of this door.
	/// </summary>
	public override void Interact()
	{
		DataManager.Instance.playerDataFile.currentScene = sceneToLoad.ToString();

		DataManager.Instance.playerDataFile.savedSpawnPoint = spawnPoint;

		Game.Instance.LoadScene(sceneToLoad.ToString());
	}
}
