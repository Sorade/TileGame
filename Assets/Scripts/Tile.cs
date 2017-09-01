using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour {
	
	Vector2 pos;
	TileCategory category;
	TileCategory tempCategory;
	TileCategory targetCategory;
	public bool isLocked = false;
	public GameObject skin;
	TypeCounter typeCounter;
	CategoryCounter catCounter;
	List<Vector2> neighborPositions =  new List<Vector2>(); // maybe needs to be a list

	List<int> rowsIndexToAdd;
	List<int> colsIndexToAdd;

	void Awake(){
		typeCounter = ScriptableObject.CreateInstance("TypeCounter") as TypeCounter;
		catCounter = ScriptableObject.CreateInstance("CategoryCounter") as CategoryCounter ;
	}

	public void Initialise(Vector2 currentPos, int row, int col){
		pos = currentPos;
		//a few iterations of the resolve method should smooth the map up
		TileCategory newCat = TileManager.instance.categories[(int) Random.Range(0f, TileManager.instance.categories.Length)];
		category = newCat;

		TileCategory newTarget = TileManager.instance.categories[(int) Random.Range(0f, TileManager.instance.categories.Length)];
		targetCategory = newTarget;

		while (category.name == targetCategory.name) {
			newTarget = TileManager.instance.categories[(int) Random.Range(0f, TileManager.instance.categories.Length)];
			targetCategory = newTarget;
		}

		SetSkin(row, col);

	}

	void SetSkin(int row, int col){
		GameObject.Destroy (skin);	
		skin = TileManager.instance.skins [category.skinID];
		GameObject newSkin = GameObject.Instantiate (skin, new Vector3((float) row, 0f, (float) col), Quaternion.identity);
		skin = newSkin;
	}

	public void AddNeighbors(int W, int H){
		int posRow = (int) pos.x;
		int posCol = (int) pos.y;

		for (int r = -1; r < 2; r++) {
			for (int c = -1; c < 2; c++) {
				bool isTile = (pos == new Vector2 (r + pos.x, c + pos.y));
				bool inXRange = (r + pos.x >= 0 && r + pos.x <= H);
				bool inYRange = (c + pos.y >= 0 && c + pos.y <= W);
				if (!isTile && inXRange && inYRange) {
					neighborPositions.Add (new Vector2 ((int) pos.x + r, (int) pos.y + c));
				}
			}
		}
	}

	public void RefreshTile(){		
		List<string> elementList =  new List<string>();
		foreach (var item in typeCounter.elements.Keys) {
			elementList.Add (item);
		}

		List<TileCategory> potentialCategories = new List<TileCategory> ();

		foreach (var currentElement in elementList) {			
			//counts available pool
			foreach( var n in neighborPositions){ //iterates through the neighbors positions
				int W = (int) n.x;
				int H = (int) n.y;

				if (MyGameManager.instance.map.tiles[W,H].category.type == currentElement)
				{
					int j = MyGameManager.instance.map.tiles [W,H].category.level - 1;
					typeCounter.elements[currentElement] = j > 0 ? typeCounter.elements[currentElement] + j : typeCounter.elements[currentElement]; //if > to 0
				}
			}
				
			foreach (var potentialCat in TileManager.instance.categories) {
				//Can be made how many times?
				if (potentialCat.type == currentElement){
					catCounter.categories[potentialCat.name]  += typeCounter.elements[currentElement] / potentialCat.level;
				}

				//Applying modifiers
				if (catCounter.categories[potentialCat.name] > 0) {
					potentialCategories.Add (potentialCat);
					if (potentialCat.type == category.type){ 
						if (potentialCat.name == category.name){ 
							catCounter.categories[potentialCat.name] *= 2.5f;
						} else {catCounter.categories[potentialCat.name] *= 1.5f;}
					} else {catCounter.categories[potentialCat.name] *= 0.5f;}
				}
			}
		}

		//Select the new Cat
		float sum = 0f;
		foreach (var item in catCounter.categories.Values) {
			sum += item;
		}
		float totalWeight = sum;
		if (totalWeight > 0) {
			tempCategory = GetCategory (potentialCategories.ToArray(), totalWeight);
		} else {
			tempCategory = category;
			//int randomIndex = Random.Range (0, TileManager.instance.categories.Length);
			//tempCategory = TileManager.instance.categories [randomIndex];
		}
		//Resetting Counters
		catCounter.Reset();
		typeCounter.Reset();
	}
		
	public void ApplyRefresh(){
		category = tempCategory;
		SetSkin ((int) pos.x, (int) pos.y);
		if (category.name == targetCategory.name) {
			isLocked = true;
		}
	}

	public TileCategory GetCategory (TileCategory[] categories, float totalWeight){
		// totalWeight is the sum of all Categorys’ weight
		float randomNumber = Random.Range(0, totalWeight);
		TileCategory selectedCategory  = null;
		foreach (TileCategory  category in categories){
			if (randomNumber <= catCounter.categories[category.name]){
				selectedCategory = category ;
				break;
			}
			randomNumber = randomNumber - catCounter.categories[category.name];
		}
		return selectedCategory ;
	}
}
