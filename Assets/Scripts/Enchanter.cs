using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchanter : MonoBehaviour {

	public static Enchanter instance = null;
	public GameObject[] spellComponentPrefabs;
	Dictionary<string, GameObject> spellComponents = new Dictionary<string, GameObject> ();
	TypeCounter tc;
	GameObject[,] spellEffects;

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

		tc = ScriptableObject.CreateInstance("TypeCounter") as TypeCounter;
		GenerateSpellPrefabDictionary ();
	}

	void GenerateSpellPrefabDictionary(){
		//safety check
		if (spellComponentPrefabs.Length != TypeCounter.elementsToAdd.Length) {
			Debug.LogWarning ("TypeCounter elements and spell components not the same size.");
			return;
		}

		for (int i = 0; i < TypeCounter.elementsToAdd.Length; i++) {
			string elementName = TypeCounter.elementsToAdd [i];
			GameObject componentPrefab = spellComponentPrefabs [i];
			spellComponents.Add (elementName, componentPrefab);
		}
	}

	public void SetUp(int W, int H){
		spellEffects = new GameObject[H + 1,W + 1];
	}

	public void PlaceSpell(Vector2 pos){
		if (tc.Sum() == 0) {
			Debug.Log ("Not Enough Gems Used");
			return;
		}
		int w = (int)pos.x;
		int h = (int)pos.y;
		//place VFX at pos
		RemoveSpell(pos);
		GameObject[] newComponents = MakeSpell ();
		spellEffects[w,h] = new GameObject("Spell");
		spellEffects[w,h].transform.position = new Vector3 (pos.x, 0f, pos.y);
		foreach (var component in newComponents) {
			GameObject newComponent = GameObject.Instantiate(component, new Vector3(pos.x, 0f, pos.y), Quaternion.identity);//might need to set to 0
			newComponent.transform.SetParent (spellEffects[w,h].transform);
		}
	}

	GameObject[] MakeSpell(){
		GameObject[] newComponents = new GameObject[(int)tc.Sum ()];
		int currentIndex = 0;
		foreach (KeyValuePair<string, int> element in tc.elements) {
			for (int i = 0; i < element.Value; i++) {
				newComponents[currentIndex] = spellComponents [element.Key];
				currentIndex += 1;
			}
		}
		return newComponents;
	}

	public void RemoveSpell(Vector2 pos){
		//remove VFX from pos If it has one
		int w = (int) pos.x;
		int h = (int)pos.y;
		if (spellEffects[w,h] != null) {
			Destroy (spellEffects [w, h]);
			Debug.Log ("Removed Spell");
		}
	}

	public void ResetCounter(){
		tc.Reset ();
	}

	/*Add a Charge(string elementType) method to the Enchanter. 
	 * It needs to typeCounter[elementType] += 1; 
	 * up to a maximum of 3, and loop back to 0 (not omitting to remove 3 from the counter).
	 */
	public void Charge(string elementType){
		if (tc.elements [elementType] <= 3) {
			tc.elements [elementType] += 1;
		} else {
			tc.elements [elementType] = 0;
		}
	}

	/*add GetEnchantement(TypeCounter tc) method
	 * which adds the Enchanter typeCounter to that of the tile to be enchanted. 
	 * And Calls the  ResetButtons() method on the EnchanterUI via a FindWithTag. */
	public void GetEnchantement(TypeCounter tileCounter){
		EventManager.TriggerEvent (AllEvents.enchantmentOccured);
		foreach (var elementType in tc.elements.Keys) {
			tileCounter.elements[elementType] += tc.elements [elementType];//
		}
	}

	public void EnchantTile(Vector2 targetPosition, TypeCounter targetCounter){
		GetEnchantement (targetCounter);
		PlaceSpell (targetPosition);
		ResetCounter ();
	}
}
