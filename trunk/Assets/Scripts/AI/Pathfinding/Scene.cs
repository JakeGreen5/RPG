using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scene
{
	public string name;
	List<Scene> adjacentScenes = new List<Scene>();
	
	class ScenePathNode
	{			
		public Scene scene;
		public ScenePathNode parent;
		
		public ScenePathNode(Scene scene, ScenePathNode parent)
		{
			this.scene = scene;
			this.parent = parent;
		}
	}

	public Scene()
	{
	}
	
	public Scene(string name)
	{
		this.name = name;
	}
	
	// Returns a path to the scene we are looking for
	public List<Scene> GetScenePath(string targetScene)
	{
		Queue<ScenePathNode> q = new Queue<ScenePathNode>();
		
		ScenePathNode startNode = new ScenePathNode(this, null);
		q.Enqueue(startNode);
		
		// Loop through the queue, breadth first search, finding the target scene
		while (q.Count > 0)
		{
			ScenePathNode currentNode = q.Dequeue();
			//Debug.Log ("Node: " + currentNode.scene.name);
			
			// Did we find the current scene?
			if (currentNode.scene.name == targetScene)
			{
				q.Enqueue(currentNode);
				
				// Get the scene path using the parent of each node starting at the current node
				List<Scene> scenePath = new List<Scene>();
				int counter = 0;
				while (currentNode.parent != null)
				{
					counter++;
					if (counter > 50) 
					{
						Debug.LogError ("This while loop overflowed the stack");
						break;
					}
					
					scenePath.Add (currentNode.scene);
					currentNode = currentNode.parent;
				}
				
				scenePath.Reverse();
				
				return scenePath;
			}
			
			// We didn't find the current scene, so keep looking
			foreach (Scene adjacentScene in currentNode.scene.adjacentScenes)
			{
				ScenePathNode adjacentNode = new ScenePathNode(adjacentScene, currentNode);
				q.Enqueue(adjacentNode);
			}
		}
		
		return null;
	}

	public void AddAdjacentScene(Scene scene)
	{
		adjacentScenes.Add (scene);
	}
}