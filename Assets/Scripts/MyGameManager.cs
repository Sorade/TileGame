using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyGameManager : MonoBehaviour {

	public static MyGameManager instance = null;
	public Map map = new Map ();
	public int mapWidth;
	public int mapHeight;
    int score;
    public TextMeshProUGUI textScore;

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
        ResetScore();
        AddMap ();
		Enchanter.instance.SetUp (mapWidth, mapHeight);
        GemController.instance.SetUp();
        //used to remove the revelead effect
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
        UpdateScore();
        map.RefreshMap ();
    }

    public void UpdateScore()
    {
        score += 1;
        textScore.SetText("{0}", score);
    }

    private void ResetScore()
    {
        score = 0;
        textScore.SetText("{0}", score);
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    }
}
