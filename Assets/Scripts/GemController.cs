using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemController : MonoBehaviour {
	public TypeCounter gemCounter;
	public static GemController instance = null;
	public GameObject[] gemObjects;
	float gemHeight = 0.3f;
	Dictionary<string, GameObject> gems = new Dictionary<string, GameObject> ();
    GameObject gemCounterUI;
    string UItext;

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

    void Start () {
		GenerateGemPrefabDictionary ();
		gemCounter = ScriptableObject.CreateInstance ("TypeCounter") as TypeCounter;
        gemCounterUI = GameObject.FindGameObjectWithTag("GemCounter");
    }
	
	void GenerateGemPrefabDictionary(){
		//safety check
		if (gemObjects.Length != TypeCounter.elementsToAdd.Length) {
			Debug.LogWarning ("TypeCounter elements and gem prefab skins not the same size.");
			return;
		}

		for (int i = 0; i < TypeCounter.elementsToAdd.Length; i++) {
			string elementName = TypeCounter.elementsToAdd [i];
			GameObject gemPrefab = gemObjects [i];
			gems.Add (elementName, gemPrefab);
		}
	}

	public Gem GenerateGem(string elementOfTile, Vector2 pos){
		Gem newGem = new Gem();
		newGem.elementType = elementOfTile;
		newGem.skin = PlaceGem (pos, gems [elementOfTile]);
		return newGem;
	}

	GameObject PlaceGem(Vector2 pos, GameObject skin){
		return GameObject.Instantiate (skin, new Vector3 (pos.x, gemHeight, pos.y), Quaternion.identity);
	}

	public void CollectGem(ref Gem gem){
		gemCounter.elements [gem.elementType] += 1;
		Destroy(gem.skin);
		gem = null;
        UpdateGemCounterUI();
    }

    void UpdateGemCounterUI()
    {
        UItext = "Fire: " + gemCounter.elements["Fire"] + "  Water: " + gemCounter.elements["Water"] + "  Earth: " + gemCounter.elements["Earth"];
        gemCounterUI.GetComponent<Text>().text = UItext;
    }

    public void AddToCounter(string elementName, int value)
    {
        gemCounter.elements[elementName] += value;
        UpdateGemCounterUI();
    }

    public void SetUp()
    {
        gemCounter = ScriptableObject.CreateInstance("TypeCounter") as TypeCounter;
        if (gemCounterUI != null)
        {
            UpdateGemCounterUI();
        }

        foreach (var item in GameObject.FindGameObjectsWithTag("Gem"))
        {
            Destroy(item);
        }
    }
}
