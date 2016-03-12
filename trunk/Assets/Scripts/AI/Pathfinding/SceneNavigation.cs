using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneNavigation : Singleton<SceneNavigation> {

	List<Scene> scenes = new List<Scene>();

	public override void Awake ()
	{
		base.Awake();

		Scene Village = AddScene("Village");
		Scene Inn = AddScene("Inn");
		Scene House1 = AddScene("House1");
		Scene House2 = AddScene("House2");
		Scene House3 = AddScene("House3");
		Scene House4 = AddScene("House4");
		Scene House5 = AddScene("House5");
		Scene House6 = AddScene("House6");
		Scene House7 = AddScene("House7");
		Scene House8 = AddScene("House8");

		AddConnection(Village, Inn);
		AddConnection(Village, House1);
		AddConnection(Village, House2);
		AddConnection(Village, House3);
		AddConnection(Village, House4);
		AddConnection(Village, House5);
		AddConnection(Village, House6);
		AddConnection(Village, House7);
		AddConnection(Village, House8);
	}

	Scene AddScene(string sceneName)
	{
		Scene scene = new Scene(sceneName);
		
		scenes.Add(scene);

		return scene;
	}

	void AddConnection(Scene sceneOne, Scene sceneTwo)
	{
		sceneOne.AddAdjacentScene(sceneTwo);
		sceneTwo.AddAdjacentScene(sceneOne);
	}

	public List<Scene> GetScenePathToScene(string currentScene, string targetScene)
	{
		Scene scene = GetSceneByName(currentScene);

		List<Scene> scenePath = scene.GetScenePath(targetScene);
		//Debug.Log ("Scene Path: " + scenePath.Count);

		return scenePath;
	}
	
	public GameObject GetDoorToAdjacentScene(Scene adjacentScene)
	{
		DoorInteract[] doorsInScene = FindObjectsOfType(typeof(DoorInteract)) as DoorInteract[];

		foreach (DoorInteract door in doorsInScene)
		{
			if (door.sceneToLoad.ToString() == adjacentScene.name)
			{
				return door.gameObject;
			}
		}

		return null;
	}

	Scene GetSceneByName(string sceneName)
	{
		foreach (Scene scene in scenes)
		{
			if (scene.name == sceneName)
				return scene;
		}

		return null;
	}
}
