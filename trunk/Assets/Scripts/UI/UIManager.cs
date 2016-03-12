using UnityEngine;
using System.Collections;

public class UIManager : Singleton<UIManager> {

	[HideInInspector] public static HeadsUpDisplay HUD;

	public override void Awake()
	{
		base.Awake();

		Initialize();
	}

	void OnLevelWasLoaded(int level)
	{
		Initialize();
	}

	void Initialize()
	{
		HUD = Tools.Instantiate("UI/MainScreens/HUD").GetComponent<HeadsUpDisplay>();
	}
}
