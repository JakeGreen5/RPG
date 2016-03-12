using UnityEngine;
using System.Collections;

public class PlayerSpawning : MonoBehaviour {

	string playerPath = "Player/PlayerPaladin";
	string playerCameraPath = "Player/PlayerCamera";

	void Awake () 
	{
		GameObject player = Tools.Instantiate(playerPath);
		GameObject playerCamera = Tools.Instantiate(playerCameraPath);

		PlayerDataFile playerDataFile = DataManager.Instance.playerDataFile;
		
		// Did we go through a door?
		if (playerDataFile.savedSpawnPoint != null)
		{
			GameObject spawnPoint = GameObject.Find ("SpawnPoint_" + playerDataFile.savedSpawnPoint);
			
			player.transform.localPosition = spawnPoint.transform.position;
			player.transform.localRotation = spawnPoint.transform.rotation;

			// Clear the saved spawn point
			playerDataFile.savedSpawnPoint = null;
		}
		else
		{
			// The player either loaded or created a new game at this point
			if (playerDataFile.currentScene != Application.loadedLevelName)
			{
				Game.Instance.comingFromStartupSceneRedirect = true;
				Application.LoadLevel(playerDataFile.currentScene);

				return;
			}

			if (playerDataFile.savedGame && playerDataFile.position.y > -20)
			{
				// Saved game
				player.transform.localPosition = playerDataFile.position;
				player.transform.localRotation = playerDataFile.rotation;
			}
			else
			{
				// New game
				GameObject spawnPoint = GameObject.Find ("PlayerSpawnPoint");

				if (spawnPoint == null)
				{
					Debug.Log ("Something went wrong with spawning. Spawning at a random spawn point");

					// Contingency plan -- Used as a last resort
					spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
				}

				player.transform.localPosition = spawnPoint.transform.position;
				player.transform.localRotation = spawnPoint.transform.rotation;
			}
		}
		
		MouseOrbit mouseOrbit = playerCamera.GetComponent<MouseOrbit>();
		mouseOrbit.target = player.transform;
		playerCamera.transform.rotation = player.transform.rotation;
	}
}
