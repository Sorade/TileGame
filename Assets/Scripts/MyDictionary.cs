using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDictionary<T> : Dictionary<T,float> {

	public float SumValues () {
		float sum = 0f;
		foreach (var item in Values) {
			sum += item;
		}
		return sum;
	}
}
