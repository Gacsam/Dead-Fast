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
	private bool dynamicInventory = true;
	private GameObject crosshairUI;
	private int thisLayer;

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
		if (theInventory [newWeaponIndex].GetWeaponType () != WeaponScript.Weapon.Melee) {
			projectileSpawn.Add (newHeldWeapon.transform.Find ("spawn" + theInventory [newWeaponIndex].GetWeaponType ()).transform);
		} else {
			projectileSpawn.Add (new GameObject().transform);
		}
	}

	// Use this for initialization
	public void Init () {
		// Create a new empty weapon
		WeaponScript newGrenade = new WeaponScript();
		WeaponScript newGun = new WeaponScript();
		WeaponScript newBarrier = new WeaponScript();
		// Set weapon to 5 grenades
		newGrenade.SetWeaponType(WeaponScript.Weapon.Grenade);
		newGrenade.SetAmmo (5);
		WeaponPickup (newGrenade);
		ShowHideWeapon ();
		// Set weapon to gun with 3 ammo
		newGun.SetWeaponType(WeaponScript.Weapon.Gun);
		newGun.SetAmmo (3);
		WeaponPickup (newGun);
		// Set weapon to 1 barricade
		newBarrier.SetWeaponType(WeaponScript.Weapon.Barricade);
		newBarrier.SetAmmo (1);
		WeaponPickup (newBarrier);
		inventoryIndex = 0;
	}

	public int GetWeaponAmmo(){
		return theInventory [inventoryIndex].GetAmmo ();
	}

	public void Start(){
		Init ();
		crosshairUI = this.GetComponentInChildren<Image> (true).gameObject;
	}

	public WeaponScript.Weapon getWeapon(){
		return theInventory [inventoryIndex].GetWeaponType();
	}

	public void ShowHideWeapon(){
		heldWeapons[inventoryIndex].SetActive (!heldWeapons[inventoryIndex].activeSelf);
		if (crosshairUI != null) {
			if (theInventory [inventoryIndex].GetWeaponType () == WeaponScript.Weapon.Barricade) {
				crosshairUI.SetActive (false);
			} else {
				crosshairUI.SetActive (true);
			}
		}
	}

	public int GetWeaponIndex(){
		return inventoryIndex;
	}

	public void NextWeapon(){
		ShowHideWeapon ();
		if (dynamicInventory){
			do {
				inventoryIndex += 1;
				if (inventoryIndex > theInventory.Count - 1) {
					inventoryIndex = 0;
					ShowHideWeapon ();
					return;
				}
			} while(theInventory [inventoryIndex].GetAmmo () == 0);
		}else inventoryIndex += 1;

		if (inventoryIndex > theInventory.Count - 1) {
			inventoryIndex = 0;
		}

		ShowHideWeapon ();
	}

	public void PrevWeapon(){
		ShowHideWeapon ();
		if (inventoryIndex == 0) {
			inventoryIndex = theInventory.Count;
		}
		if (dynamicInventory){
			do {
				inventoryIndex -= 1;
			} while(theInventory [inventoryIndex].GetAmmo () == 0);
		}else inventoryIndex -= 1;
		ShowHideWeapon ();
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
	}

	public bool isDynamicInventory(){
		return dynamicInventory;
	}


	public bool isMelee(){
		if (theInventory [inventoryIndex].GetWeaponType() == WeaponScript.Weapon.Melee)
			return true;
		return false;
	}
}