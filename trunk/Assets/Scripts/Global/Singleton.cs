using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance 
	{
        get 
		{
			// Create a new instance of this singleton if one doesn't exist
            if (_instance == null) 
			{
				//Debug.Log ("Creating a new " + typeof(T).ToString() + " singleton");
				GameObject obj = new GameObject(typeof(T).ToString());
				T t = obj.AddComponent<T>();
				
				DontDestroyOnLoad (obj);

				_instance = t;
            }
			
            return _instance;
        }
    }
 
    public virtual void Awake ()
    {
        if (_instance != null && _instance != this)
		{
			// Destroy this instance if one already exists
			Destroy (gameObject);
		}
    }

	// Keep this here but put nothing in it
	public void DummyMethod()
	{
	}
}