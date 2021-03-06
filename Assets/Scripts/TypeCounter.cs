﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeCounter: ScriptableObject {
	
	string[] elementsToAdd = {"Earth", "Fire", "Water"};
	public Dictionary<string,int> elements = new Dictionary<string,int>();

	void OnEnable (){
		foreach (var element in elementsToAdd){
			elements.Add(element,0);
		}
	}

	public void Reset(){
		foreach (var element in elementsToAdd){
			elements[element] = 0;
		}
	}
}

 