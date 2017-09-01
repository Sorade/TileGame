using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryCounter: ScriptableObject {

	string[] categoriesToAdd = {"Field", "Forest", "Dirt", "Lava", "Pond", "Lake"};
	public Dictionary<string,float> categories = new Dictionary<string,float>();

	void OnEnable (){
		foreach (var cat in categoriesToAdd){
			categories.Add(cat,0);
		}
	}

	public void Reset(){
		foreach (var cat in categoriesToAdd){
			categories[cat] = 0;
		}
	}
}
