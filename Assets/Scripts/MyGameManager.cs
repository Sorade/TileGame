using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour {

	public static MyGameManager instance = null;
	public Map map = new Map ();
	public int mapWidth;
	public int mapHeight;

	#region
	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			//if not, set instance to this
			instance = this;
		//If instance already exists and it's not this:
		else if (instance != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);
	}
	#endregion //Singleton setup

	void Start(){
		StartNewGame ();
	}

	public void StartNewGame(){
		AddMap ();
		Enchanter.instance.SetUp (mapWidth, mapHeight);
		foreach (var item in GameObject.FindGameObjectsWithTag("LevelResettable")) {
			Destroy(item);
		}
	}

    public void AddMap()
    {
		map.GenerateMap(mapWidth, mapHeight);
    }

    public void RefreshMap()
    {
        
		map.RefreshMap ();
        EventManager.TriggerEvent(AllEvents.turnEnded);
    }
}
