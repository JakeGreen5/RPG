using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DataManager : Singleton<DataManager> {
	
	public GameDataFile gameDataFile;
	string GameDataFilePath = Path.Combine(Application.persistentDataPath, "GameData.xml");

	public AIDataFile AIDataContainer;
	string AIDataFilePath = Path.Combine(Application.persistentDataPath, "AIData.xml");
	Type[] extraAIDataTypes = {typeof(Food), typeof(Rest), 
		typeof(Drink), typeof(Eat), typeof(Sleep), typeof(Talk), typeof(Walk), 
		typeof(Converse), typeof(DrinkAtTavern), typeof(EatAtHome), typeof(SleepAtHome)};
	
	public PlayerDataFile playerDataFile;
	string PlayerDataFilePath = Path.Combine(Application.persistentDataPath, "PlayerData.xml");

	public static bool ApplicationHasQuit;

	public override void Awake()
	{
		base.Awake();
		
		LoadData();
	}

	/// <summary>
	/// When the application quits, we save the data
	/// </summary>
	void OnApplicationQuit()
	{
		DataManager.ApplicationHasQuit = true;
		
		SaveData();
	}

	/// <summary>
	/// Deletes any existing data and creates it fresh
	/// </summary>
	void CreateData()
	{		
		// Delete old game
		DeleteData();

		CreateGameData();
		CreateAIData();
		CreatePlayerData();
	}

	/// <summary>
	/// Saves the data.
	/// </summary>
	void SaveData()
	{
		Debug.Log ("Saving data to " + Application.persistentDataPath);

		SaveGameData();
		SaveAIData();
		SavePlayerData();
	}

	/// <summary>
	/// Loads the data.
	/// </summary>
	void LoadData()
	{
		if (Constants.FORCE_NEW_GAME)
		{
			CreateData();
		}
		else
		{
			LoadGameData();
			LoadAIData();
			LoadPlayerData();
		}
	}

	/// <summary>
	/// Deletes the data.
	/// </summary>
	void DeleteData()
	{
		DeleteGameData();
		DeleteAIData();
		DeletePlayerData();
	}

	#region GameData
	/// <summary>
	/// Creates the game data.
	/// </summary>
	void CreateGameData()
	{
		Debug.Log ("Creating new game");
		
		// New game
		gameDataFile = new GameDataFile();
		
		TimeManager.Instance.timeOfDayInSeconds = TimeManager.Instance.Noon; // start the day at noon
	}

	/// <summary>
	/// Saves the game data.
	/// </summary>
	void SaveGameData()
	{
		Debug.Log ("Saving game");

		gameDataFile.timeOfDayInSeconds = TimeManager.Instance.timeOfDayInSeconds;
		
		FileManager.Save(gameDataFile, GameDataFilePath, null);
	}

	/// <summary>
	/// Loads the game data.
	/// </summary>
	void LoadGameData()
	{
		Debug.Log ("Loading saved game");
		
		if (System.IO.File.Exists(GameDataFilePath))
		{
			gameDataFile = FileManager.Load(typeof(GameDataFile), GameDataFilePath, null) as GameDataFile;
			
			TimeManager.Instance.timeOfDayInSeconds = gameDataFile.timeOfDayInSeconds;
		}
		else
		{
			CreateGameData();
		}
	}

	/// <summary>
	/// Deletes the game data.
	/// </summary>
	void DeleteGameData()
	{
		if (System.IO.File.Exists(GameDataFilePath))
		{
			Debug.Log("Deleting game at " + GameDataFilePath);
			FileManager.Delete(GameDataFilePath);
		}
	}
	#endregion

	#region AIData
	/// <summary>
	/// Creates the AI data.
	/// </summary>
	void CreateAIData()
	{
		Debug.Log ("Creating AI data");

		AIDataContainer = new AIDataFile();
		
		AIData data = new AIData("House1", "Arthur", new AIProperty(1, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House1", "Robb Stark", new AIProperty(1, 2));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House2", "Christopher", new AIProperty(2, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House3", "George", new AIProperty(3, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House3", "Lancelot", new AIProperty(3, 2));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House4", "Frederick", new AIProperty(4, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House4", "Michael", new AIProperty(4, 2));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House4", "Eric", new AIProperty(4, 3));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House5", "Hadrian", new AIProperty(5, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House5", "Henry", new AIProperty(5, 2));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House5", "Thomas", new AIProperty(5, 3));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House6", "Roger", new AIProperty(6, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House6", "Richard", new AIProperty(6, 2));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House6", "Daniel", new AIProperty(6, 3));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House7", "Jon Snow", new AIProperty(7, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House7", "Ned Stark", new AIProperty(7, 2));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House8", "Aragon", new AIProperty(8, 1));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House8", "Faramir", new AIProperty(8, 2));
		AIDataContainer.AIDataList.Add (data);
		
		data = new AIData("House8", "Boromir", new AIProperty(8, 3));
		AIDataContainer.AIDataList.Add (data);
	}

	/// <summary>
	/// Saves the AI data.
	/// </summary>
	public void SaveAIData()
	{
		Debug.Log ("Saving AI data");
		FileManager.Save(AIDataContainer, AIDataFilePath, extraAIDataTypes);
	}

	/// <summary>
	/// Loads the AI data.
	/// </summary>
	public void LoadAIData()
	{
		if (System.IO.File.Exists(AIDataFilePath))
		{
			Debug.Log ("Loading AI data");

			// To avoid memory leaks, remove the event handlers on all AIData scripts since we're about to replace them.
			foreach (AIData data in AIDataContainer.AIDataList)
			{
				data.RemoveEventHandlers();
			}

			AIDataContainer = FileManager.Load(typeof(AIDataFile), AIDataFilePath, extraAIDataTypes) as AIDataFile;
		}
		else
		{
			CreateAIData();
		}
	}

	/// <summary>
	/// Deletes the AI data.
	/// </summary>
	void DeleteAIData()
	{
		Debug.Log ("Deleting AI data");
		if (System.IO.File.Exists(AIDataFilePath))
		{
			FileManager.Delete(AIDataFilePath);
		}
	}
	#endregion

	#region PlayerData
	/// <summary>
	/// Creates the player data.
	/// </summary>
	void CreatePlayerData()
	{
		playerDataFile = new PlayerDataFile();

		playerDataFile.currentScene = "Village";
	}

	/// <summary>
	/// Saves the player data.
	/// </summary>
	public void SavePlayerData()
	{
		playerDataFile.savedGame = true;
		playerDataFile.position = Game.Instance.player.transform.localPosition;
		playerDataFile.rotation = Game.Instance.player.transform.localRotation;

		FileManager.Save(playerDataFile, PlayerDataFilePath, null);
	}

	/// <summary>
	/// Loads the player data.
	/// </summary>
	public void LoadPlayerData()
	{
		if (System.IO.File.Exists(PlayerDataFilePath))
		{
			playerDataFile = FileManager.Load(typeof(PlayerDataFile), PlayerDataFilePath, null) as PlayerDataFile;
		}
		else
		{
			CreatePlayerData();
		}
	}

	/// <summary>
	/// Deletes the player data.
	/// </summary>
	void DeletePlayerData()
	{
		if (System.IO.File.Exists(PlayerDataFilePath))
		{
			FileManager.Delete(PlayerDataFilePath);
		}
	}
	#endregion
}
