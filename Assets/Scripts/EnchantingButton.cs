using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnchantingButton : MonoBehaviour {
	public Sprite[] icons;
	int currentIconIndex;
	Image iconSlot;

	void OnEnable(){
		EventManager.StartListening (AllEvents.enchantmentOccured, ResetButton);
	}

	void OnDisable(){
		EventManager.StopListening (AllEvents.enchantmentOccured, ResetButton);
	}

	// Use this for initialization
	void Awake () {
		iconSlot = gameObject.GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void ResetButton () {
		iconSlot.sprite = icons [0];
		currentIconIndex = 0;
	}

	public void Charge (string elementType){
		if (currentIconIndex < 3) {
			currentIconIndex += 1;
			Enchanter.instance.Charge (elementType);
		} else {
			currentIconIndex = 0;
		}
		iconSlot.sprite = icons [currentIconIndex];
	}
}
