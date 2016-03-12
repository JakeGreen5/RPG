using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : Singleton<Game> {

	public List<InanimateObject> objectList = new List<InanimateObject>();
	
	public GameObject player;
	public Player playerScript;

	public GameObject[] villagers;

	public List<GameObject> allPeople = new List<GameObject>();

	public bool comingFromStartupSceneRedirect;

	public override void Awake()
	{
		base.Awake();
	}

	void Start()
	{
		Setup();
	}
	
	/// <summary>
	/// Loads a scene, saving the AIData first.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	public void LoadScene(string sceneName)
	{		
		DataManager.Instance.SaveAIData();
		
		Application.LoadLevel(sceneName);
	}

	/// <summary>
	/// When a level is loaded, we load the AIData for the level so we can setup the correct people in our scene.
	/// </summary>
	/// <param name="level">Level.</param>
	void OnLevelWasLoaded(int level)
	{
		// Load the AIData only if we're not coming from an initial scene redirect in case we launched the game in another scene.
		// This will prevent the AIData from being created twice (once in DataManager.Awake and once here).
		if (comingFromStartupSceneRedirect)
		{
			comingFromStartupSceneRedirect = false;
		}
		else
		{
			DataManager.Instance.LoadAIData();
		}

		Setup();
	}

	/// <summary>
	/// Initializer for anything that needs to be taken care of.
	/// </summary>
	void Setup()
	{
		Pathfinding.Instance.SetupMap();

		allPeople.Clear();

		AISimulation.Instance.ClearList();

		AddObjects();
		AddPlayer();
		AddVillagers();
	}

	/// <summary>
	/// Finds the objects in the scene, adds a component, and adds them to an array.
	/// </summary>
	private void AddObjects()
	{
		objectList.Clear();

		GameObject[] objectArray = GameObject.FindGameObjectsWithTag("Object");
		for (int i = 0; i < objectArray.Length; i++)
		{
			InanimateObject inanimateObject = objectArray[i].AddComponent<InanimateObject>();
			objectList.Add(inanimateObject);
		}
	}

	/// <summary>
	/// Adds the player.
	/// </summary>
	private void AddPlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerScript = player.GetComponent<Player>();
		allPeople.Add(player);
	}

	/// <summary>
	/// Adds the villagers to either the scene or the AISimulation script by checking the saved AIData.
	/// </summary>
	private void AddVillagers()
	{
		foreach (AIData data in DataManager.Instance.AIDataContainer.AIDataList)
		{
			if (data.currentScene == Application.loadedLevelName)
			{
				AddVillager(data);
			}
			else
			{
				AISimulation.Instance.AddVillager(data);
			}
		}
	}

	/// <summary>
	/// Adds a villager to the scene the player is currently in.
	/// </summary>
	/// <param name="data">Data.</param>
	/// <param name="spawnPointName">Spawn point name.</param>
	public void AddVillager(AIData data, string spawnPointName = "")
	{
		AddTasks(data);

		GameObject villager = Instantiate(Resources.Load("Characters/Villager")) as GameObject;

		villager.GetComponent<AIManager>().data = data;

		if (string.IsNullOrEmpty(spawnPointName))
		{
			spawnPointName = "AISpawnPoint" + allPeople.Count;
		}
		Transform spawnPoint = GameObject.Find(spawnPointName).transform;

		villager.transform.position = spawnPoint.position;
		villager.transform.eulerAngles = spawnPoint.eulerAngles;

		villager.name = "Villager - " + data.name;

		allPeople.Add (villager);
	}
	
	/// <summary>
	/// This method takes the task list and recreates it via the AddTask method.
	/// Since the data was loaded directly into the list of tasks, AddTask wasn't called.
	/// This caused problems since the OnTaskFinished event wasn't being set properly upon loading.
	/// </summary>
	/// <param name="data">Data.</param>
	private void AddTasks(AIData data)
	{
		List<Task> tempTaskList = new List<Task>();
		foreach (Task task in data.tasks)
		{
			tempTaskList.Add(task);
		}

		data.tasks.Clear();

		foreach (Task task in tempTaskList)
		{
			data.AddTask(task);
		}
	}

	/// <summary>
	/// Removes a villager from the scene the player is currently in.
	/// Adds the villager to the simulation array in AISimulation.
	/// </summary>
	/// <param name="villager">Villager.</param>
	public void RemoveVillager(GameObject villager)
	{
		allPeople.Remove (villager);
		
		DataManager.Instance.SaveAIData();
		
		AISimulation.Instance.AddVillager(villager.GetComponent<AIManager>().data);

		Destroy (villager);
	}
}
