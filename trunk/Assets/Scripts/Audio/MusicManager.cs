using UnityEngine;
using System.Collections;

public class MusicManager : Singleton<MusicManager> {

	string LionsPrideInn = "Audio/Lion's Pride Inn";
	string SkyrimTheme = "Audio/Skyrim Theme";
	string GameOfThronesTheme = "Audio/Game of Thrones";
	string GameOfFuckingThronesTheme = "Audio/Game of Fucking Thrones";
	string TheRainsOfCastamere = "Audio/The Rains of Castamere";
	string LastOfTheMohicans = "Audio/Last of the Mohicans";
	string GladiatorNowWeAreFree = "Audio/Gladiator - Now We Are Free";
	string LastMarchOfTheEnts = "Audio/LOTR - The Last March of the Ents";

	public override void Awake ()
	{
		base.Awake ();

		AudioListener.volume = 0.5f;
		gameObject.AddComponent<AudioSource>();
		ChooseMusicForLevel();
	}
	
	void OnLevelWasLoaded(int level)
	{
		ChooseMusicForLevel();
	}

	void ChooseMusicForLevel()
	{
		if (Application.loadedLevelName == "Inn")
		{
			PlaySong(LionsPrideInn);
		}
		else if (Application.loadedLevelName == "House1")
		{
			int rand = Random.Range(0, 5);

			if (rand == 0)
			{
				PlaySong(GameOfFuckingThronesTheme);
			}
			else
			{
				PlaySong(GameOfThronesTheme);
			}
		}
		else if (Application.loadedLevelName == "House2")
		{
			PlaySong(TheRainsOfCastamere);
		}
		else if (Application.loadedLevelName == "House3")
		{
			PlaySong(LastOfTheMohicans);
		}
		else if (Application.loadedLevelName == "House4")
		{
			PlaySong(SkyrimTheme);
		}
		else if (Application.loadedLevelName == "House5")
		{
			PlaySong(LastMarchOfTheEnts);
		}
		else if (Application.loadedLevelName == "House6")
		{
			PlaySong(SkyrimTheme);
		}
		else if (Application.loadedLevelName == "House7")
		{
			PlaySong(GameOfThronesTheme);
		}
		else if (Application.loadedLevelName == "House87")
		{
			PlaySong(LastMarchOfTheEnts);
		}
		else
		{
			PlaySong(GladiatorNowWeAreFree);
		}
	}

	void PlaySong(string song)
	{
		//audio.volume = 0f;
		GetComponent<AudioSource>().clip = (AudioClip)Resources.Load (song);
		GetComponent<AudioSource>().loop = true;
		GetComponent<AudioSource>().Play ();
	}
}
