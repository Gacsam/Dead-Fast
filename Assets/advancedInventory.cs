using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class advancedInventory : MonoBehaviour {

	private int inventoryIndex = 0;
	private List<GameObject> heldWeapons;
	private List<Transform> projectileSpawn;
	[SerializeField]
	private List<GameObject> projectilePrefabs;
	[SerializeField]
	public bool dynamicInventory = true;
	private int thisLayer;
	public GameObject playerPanel;
	private Image[] iconsUI;

	// Inventory system
	private List<WeaponScript> theInventory;
	public void WeaponPickup(WeaponScript pickedWeapon)
	{
		if (pickedWeapon.GetAmmo() == 0)
			return;
		
		if (theInventory.Count > 1) {
			// Go through all the weapons in inventory
			foreach (WeaponScript currentWeapon in theInventory) {
				// Check if weapon already exists in inventory
				if (currentWeapon.GetWeaponType () == pickedWeapon.GetWeaponType ()) {
					// If it exists, take the ammo and close the loop
					currentWeapon.ModAmmo (pickedWeapon.GetAmmo ());
					if (theInventory [inventoryIndex].GetAmmo () == 0)
						NextWeapon ();
					UpdateUI ();
					return;
				}
			}
		}
		// If it doesn't exist, add the weapon to inventory
		theInventory.Add (pickedWeapon);
		GetNewObjectData ();
	}

	void Awake(){
		theInventory = new List<WeaponScript>();
		heldWeapons = new List<GameObject>();
		projectileSpawn = new List<Transform>();
	}

	// Do not touch this, called by WeaponPickup, gets the weapon held by the player and its spawn position
	void GetNewObjectData () {
		int newWeaponIndex = theInventory.Count - 1;
		GameObject newHeldWeapon = this.transform.Find ("held" + theInventory [newWeaponIndex].GetWeaponType ()).gameObject;
		heldWeapons.Add(newHeldWeapon);
		newHeldWeapon.layer = thisLayer;
		newHeldWeapon.SetActive (false);

		projectileSpawn.Add (newHeldWeapon.transform.Find ("spawn" + theInventory [newWeaponIndex].GetWeaponType ()).transform);
	}

	// Use this for initialization
	public void Init () {
		// Create a new empty weapon
		WeaponScript newGrenade = new WeaponScript();
		WeaponScript newBarrier = new WeaponScript();
		WeaponScript newPig = new WeaponScript();
		// Set weapon to 5 grenades
		newGrenade.SetWeaponType(WeaponScript.Weapon.Grenade);
		newGrenade.SetAmmo (1);
		WeaponPickup (newGrenade);
		ShowWeapon ();
		// Set weapon to 1 pigs
		newPig.SetWeaponType(WeaponScript.Weapon.Pig);
		newPig.SetAmmo (1);
		WeaponPickup (newPig);
		// Set weapon to 1 barricade
		newBarrier.SetWeaponType(WeaponScript.Weapon.Barricade);
		newBarrier.SetAmmo (1);
		WeaponPickup (newBarrier);
	}

	public int GetWeaponAmmo(){
		return theInventory [inventoryIndex].GetAmmo ();
	}

	public void Start(){
		Init ();
		iconsUI = playerPanel.GetComponentsInChildren<Image> ();
		UpdateUI ();
	}

	public WeaponScript.Weapon getWeapon(){
		return theInventory [inventoryIndex].GetWeaponType();
	}

	public void HideWeapon(){
		heldWeapons[inventoryIndex].SetActive (false);
	}

	public void ShowWeapon(){
		heldWeapons[inventoryIndex].SetActive (true);
	}

	public int GetWeaponIndex(){
		return inventoryIndex;
	}

	public void NextWeapon(){
		HideWeapon ();
		if (dynamicInventory) {
			int startIndex = inventoryIndex;
			do {
				inventoryIndex += 1;
				if(inventoryIndex >theInventory.Count - 1){
					inventoryIndex = 0;
					HideWeapon ();
				}
				if (inventoryIndex == startIndex) {
					return;
				}
			} while(theInventory [inventoryIndex].GetAmmo () == 0);
		} else {
			inventoryIndex += 1;
			if (inventoryIndex > theInventory.Count - 1) {
				inventoryIndex = 0;
			}
		}
		ShowWeapon ();
		UpdateUI ();
	}

	public void PrevWeapon(){
		HideWeapon ();
		if (dynamicInventory) {
			int startIndex = inventoryIndex;
			do {
				inventoryIndex -= 1;
				if(inventoryIndex < 0){
					inventoryIndex = theInventory.Count - 1;
				}
				if (inventoryIndex == startIndex){
					return;
				}
			} while(theInventory [inventoryIndex].GetAmmo () == 0);
		} else {
			inventoryIndex -= 1;
			if (inventoryIndex == 0)
				inventoryIndex = theInventory.Count;
		}
		ShowWeapon ();
		UpdateUI ();
	}

	public void SetLayers(int theLayer){
		// Set player visibility layers
		thisLayer = theLayer;
	}

	public Transform GetProjectileSpawn(string spawnName){
		foreach (Transform theSpawn in projectileSpawn) {
			if (theSpawn.name == "spawn" + spawnName) {
				return theSpawn;
			}
		}
		return null;
	}

	public GameObject GetProjectilePrefab(string prefabName){
		foreach (GameObject theProjectile in projectilePrefabs) {
			if (theProjectile.name == prefabName) {
				return theProjectile;
			}
		}
		return null;
	}

	public void ModAmmo(int changeAmmo){
		theInventory [inventoryIndex].ModAmmo (changeAmmo);
		if (theInventory [inventoryIndex].GetAmmo () == 0 && dynamicInventory)
			NextWeapon ();
		UpdateUI ();
	}

	private void UpdateUI(){
		for(int i = 0; i < theInventory.Count; i++){
			WeaponScript weapon = theInventory [i];
			if (weapon.GetAmmo () == 0) {
				iconsUI [i].color = Color.black;
			}else if (weapon == theInventory [inventoryIndex]) {
				iconsUI [i].color = Color.white;
			} else {
				iconsUI [i].color = Color.grey;
			}
		}
	}
}