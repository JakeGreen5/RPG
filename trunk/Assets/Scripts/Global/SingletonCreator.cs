using UnityEngine;
using System.Collections;

public class SingletonCreator : MonoBehaviour {

	void Awake()
	{
		AwakeSingletons();
	}
	
	// Creates singletons if they dont already exist
	void AwakeSingletons()
	{
		DataManager.Instance.DummyMethod();
		Pathfinding.Instance.DummyMethod();
		SceneNavigation.Instance.DummyMethod();
		Game.Instance.DummyMethod();
		UIManager.Instance.DummyMethod();
		MusicManager.Instance.DummyMethod();
		TimeManager.Instance.DummyMethod();
		AISimulation.Instance.DummyMethod();
	}
}
