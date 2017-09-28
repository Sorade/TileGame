using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
	
	Vector2 pos;
	TileCategory category;
	TileCategory tempCategory;
	TileCategory targetCategory;
	public bool isLocked = false;
	bool isEnchanted;
	public GameObject skin;
	CallbackBoxCollider bc;
	TypeCounter typeCounter = ScriptableObject.CreateInstance("TypeCounter") as TypeCounter;
	CategoryCounter catCounter = ScriptableObject.CreateInstance("CategoryCounter") as CategoryCounter ;
	List<Vector2> neighborPositions =  new List<Vector2>(); // maybe needs to be a list
	public Gem gem = null;
    private float gemGenerationProba = 0.25f;

    List<int> rowsIndexToAdd;
	List<int> colsIndexToAdd;

	public void Initialise(Vector2 currentPos, int row, int col){
		pos = currentPos;
		//a few iterations of the resolve method should smooth the map up
		TileCategory newCat = TileManager.instance.categories[(int) Random.Range(0, (int) TileManager.instance.categories.Length)];
		category = newCat;

		TileCategory newTarget = TileManager.instance.categories[(int) Random.Range(0, (int) TileManager.instance.categories.Length)];
		targetCategory = newTarget;

		while (category.name == targetCategory.name) {
			newTarget = TileManager.instance.categories[(int) Random.Range(0, (int) TileManager.instance.categories.Length)];
			targetCategory = newTarget;
		}
        GameObject.Destroy(skin);
        SetSkin(row, col);
	}

	void SetSkin(int row, int col){
		//GameObject.Destroy (skin);	
		skin = TileManager.instance.skins [category.skinID];
		GameObject newSkin = GameObject.Instantiate (skin, new Vector3((float) row, 0f, (float) col), Quaternion.identity);
		//newSkin.AddComponent<CallbackBoxCollider> ();
		bc = newSkin.GetComponent<CallbackBoxCollider> ();
		bc.parentTile = this;
		skin = newSkin;
	}

	public void AddNeighbors(int W, int H){
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
        //the the true terrain has not been found generate a new tile
		if (!isLocked) {
			GenerateNewCategory ();
		}
        //generates a gem with random probability
        if (Random.Range(0f,1f) <= gemGenerationProba)
        {
            gem = GemController.instance.GenerateGem(tempCategory.type, pos);
        }
		
		Enchanter.instance.RemoveSpell (pos);
		isEnchanted = false;
	}

	void GenerateNewCategory(){
		List<string> elementList =  new List<string>();
		foreach (var item in typeCounter.elements.Keys) {
			elementList.Add (item);
		}

		List<TileCategory> potentialCategories = new List<TileCategory> ();

		typeCounter.elements [category.type] += category.level;

		foreach (var currentElement in elementList) {
			//counts available pool
			foreach( var n in neighborPositions){ //iterates through the neighbors positions
				int W = (int) n.x;
				int H = (int) n.y;

				if (MyGameManager.instance.map.tiles[W,H].category.type == currentElement)
				{
					int j = MyGameManager.instance.map.tiles [W, H].category.level - 0;
					typeCounter.elements[currentElement] = j > 0 ? typeCounter.elements[currentElement] + j : typeCounter.elements[currentElement]; //if > to 0
				}
			}

			foreach (var potentialCat in TileManager.instance.categories) {
				//Can be made how many times?
				if (potentialCat.type == currentElement){
					catCounter.categories[potentialCat.name]  += (typeCounter.elements[currentElement] / (float) potentialCat.level);
					//Applying modifiers
                    //Will avoid the targetcategory unless the player has enchanted the tile
					if (catCounter.categories[potentialCat.name] > 0)
                    {
                        ApplyModifiers(potentialCat);
                        potentialCategories.Add(potentialCat);
					}
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
            if (tempCategory == targetCategory && !isEnchanted)
            {
                tempCategory = category;
            }
		} else {
			tempCategory = category;
			//int randomIndex = Random.Range (0, TileManager.instance.categories.Length);
			//tempCategory = TileManager.instance.categories [randomIndex];
		}
		//Resetting Counters
		catCounter.Reset();
		typeCounter.Reset();
	}

	void ApplyModifiers(TileCategory cat){
		if (cat.level == 1) { 
			catCounter.categories [cat.name] /= 2f;
		} else {
			catCounter.categories [cat.name] = (float) (int) catCounter.categories [cat.name];
		}			
	}

    //Assigns the newly generated category to the tile and generates the appropriate skin
	public void ApplyRefresh(){
		category = tempCategory;
        GameObject.Destroy(skin);
        SetSkin ((int) pos.x, (int) pos.y);
		if (category.name == targetCategory.name && !isLocked) {
			isLocked = true;
			GameObject.Instantiate (TileManager.instance.revealedSkin, new Vector3 (pos.x, 0.5f, pos.y), Quaternion.identity);
		}
	}

	public TileCategory GetCategory (TileCategory[] categories, float totalWeight){
		// totalWeight is the sum of all Categorys’ weight
		float randomNumber = Random.Range(0f, totalWeight);
		TileCategory selectedCategory  = null;
		foreach (TileCategory  cat in categories){
			if (pos == new Vector2(2f,2f)) {
			}
			if (randomNumber <= catCounter.categories[cat.name]){
				selectedCategory = cat ;
				break;
			}
			randomNumber = randomNumber - catCounter.categories[cat.name];
		}
		return selectedCategory ;
	}

	public void OnMouseOver (){
		if (Input.GetMouseButtonDown (0) && gem != null) {
			GemController.instance.CollectGem (ref gem);
            //Debug.Log("Collected Gem at " + pos.x + "x" + pos.y);
        }
		else if (Input.GetMouseButtonDown (0) && !isEnchanted) {
			Enchant ();
			//Debug.Log ("Pressed Tile at " + pos.x + "x" + pos.y);
		}
	}

	void Enchant(){
		isEnchanted = true;
		Enchanter.instance.EnchantTile (pos, typeCounter);
	}
}
