using UnityEngine;
using System;
using System.Collections;

public class TimeManager : Singleton<TimeManager> {

	Light sun;

	Material dawnDuskSkybox;
	Material daySkybox;
	Material nightSkybox;

	const int dayStart = 8;
	const int nightStart = 18;

	const float second = 1;
	const float minute = 60 * second;
	const float hour = 60 * minute;
	const float day = 24 * hour; // 86400

	public float timeOfDayInSeconds; // Current time of day in seconds
	public float timeOfDayInMinutes
	{
		get { return timeOfDayInSeconds / minuteInGameTime; }
	}
	public float timeOfDayInHours
	{
		get { return timeOfDayInSeconds / hourInGameTime; }
	}

	int currentMinute;
	int currentHour;
	
	public delegate void NextMinuteEvent();
	public event NextMinuteEvent NextMinute;

	public delegate void NextHourEvent();
	public event NextHourEvent NextHour;

	const float dayLengthInMinutes = 20f; // Length of Day
	const float dayLengthInSeconds = dayLengthInMinutes * minute;
	const float dayLengthInHours = dayLengthInSeconds / hour;
	const float dayLengthInDays = dayLengthInSeconds / day;
	
	const float secondInGameTime = dayLengthInSeconds / day;
	const float minuteInGameTime = secondInGameTime * 60;
	const float hourInGameTime = minuteInGameTime * 60;
	const float dayInGameTime = hourInGameTime * 24;
	const float degreeRotation = 360 / dayLengthInSeconds;

	public float Noon
	{
		get { return dayInGameTime / 2; }
	}

	public DayOfTheWeek dayOfTheWeek = DayOfTheWeek.Sunday;
	public enum DayOfTheWeek
	{
		Sunday,
		Monday,
		Tuesday,
		Wednesday,
		Thursday,
		Friday,
		Saturday
	}

	void Start ()
	{
		currentMinute = CurrentMinute();
		currentHour = CurrentHour();

		FindTheSun();
		LoadSkyboxes();
	}

	void OnLevelWasLoaded(int level)
	{
		FindTheSun();
	}

	void FindTheSun()
	{
		GameObject sunGameObject = GameObject.FindGameObjectWithTag("Sun");
		
		if (sunGameObject)
		{
			sun = sunGameObject.GetComponent<Light>();
			sun.transform.localEulerAngles = new Vector3(degreeRotation * timeOfDayInSeconds - 90f, sun.transform.localEulerAngles.y, sun.transform.localEulerAngles.z);
		}
	}

	void LoadSkyboxes()
	{
		dawnDuskSkybox = Resources.Load ("Skyboxes/DawnDusk Skybox") as Material;
		daySkybox = Resources.Load ("Skyboxes/Sunny3 Skybox") as Material;
		nightSkybox = Resources.Load ("Skyboxes/StarryNight Skybox") as Material;

		ChangeSkybox();
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(1, 0, 80, 22), GameTime());
		GUI.Box(new Rect(82, 0, 80, 22), dayOfTheWeek.ToString());
	}

	void Update()
	{
		if (sun)
		{
			sun.transform.Rotate(new Vector3(-degreeRotation, 0, 0) * Time.deltaTime);
		}
		
		UpdateTimeOfDay();
		
		FireTimeBasedEvents();

		ChangeSkybox();
	}

	void UpdateTimeOfDay()
	{
		timeOfDayInSeconds += Time.deltaTime;
		
		float percentOfDayComplete = timeOfDayInSeconds / dayLengthInSeconds * 100;
		
		if (percentOfDayComplete > 100)
		{
			dayOfTheWeek++;
			if (dayOfTheWeek > DayOfTheWeek.Saturday)
			{
				dayOfTheWeek = DayOfTheWeek.Sunday;
			}
			
			timeOfDayInSeconds = 0;
			percentOfDayComplete = 0;
			dayOfTheWeek++;
			if (dayOfTheWeek > DayOfTheWeek.Saturday)
			{
				dayOfTheWeek = DayOfTheWeek.Sunday;
			}
		}
	}

	void FireTimeBasedEvents()
	{
		if (currentMinute != CurrentMinute()) 
		{
			currentMinute = CurrentMinute();

			if (NextMinute != null) NextMinute();

			if (currentHour != CurrentHour())
			{
				currentHour = CurrentHour();

				if (NextHour != null) NextHour();
			}
		}
	}

	public float TimePassedSince(float startDay, float startTime)
	{
		float timeToSubtractFrom = timeOfDayInMinutes;

		// Account for the case where the next day rolled around mid task
		if ((DayOfTheWeek)startDay != dayOfTheWeek)
		{
			timeToSubtractFrom = (dayInGameTime - startTime) + timeOfDayInMinutes;
		}

		return timeToSubtractFrom - startTime;
	}

	void ChangeSkybox()
	{
		RenderSettings.skybox = dawnDuskSkybox;
		int hour = CurrentHour();
		if (hour > nightStart || hour < dayStart - 2)
		{
			RenderSettings.skybox = nightSkybox;
		}
		else if (hour > nightStart - 2 || hour < dayStart)
		{
			RenderSettings.skybox = dawnDuskSkybox;
		}
		else
		{
			RenderSettings.skybox = daySkybox; 
		}
	}

	public string GameTime()
	{
		DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CurrentHour(), CurrentMinute(), 0);
		return dt.ToShortTimeString();
	}

	public int CurrentHour()
	{
		return (int)Mathf.Floor(timeOfDayInSeconds / hourInGameTime);
	}
	
	public int CurrentMinute()
	{
		return (int)Mathf.Floor((timeOfDayInSeconds / minuteInGameTime) % 60);
	}
}
