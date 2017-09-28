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

	void Awake () {
		iconSlot = gameObject.GetComponent<Image> ();
	}
	
	void ResetButton () {
		iconSlot.sprite = icons [0];
		currentIconIndex = 0;
	}

    //Calls the enchanting function on the enchanter and assigns the corresponding icon to the button
	public void Charge (string elementType){
        currentIconIndex = Enchanter.instance.Charge (elementType);
        iconSlot.sprite = icons [currentIconIndex];
	}
}
