using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicInventory : MonoBehaviour {

	public List<GameObject> heldWeapons;
	private int heldWeaponIndex = 0;
	private GameObject crosshairUIP1;
	private GameObject crosshairUIP2;

	// Use this for initialization
	void Start () {
		crosshairUIP1 = GameObject.Find ("Crosshair1");
		crosshairUIP2 = GameObject.Find ("Crosshair2");
		foreach (GameObject currentWeapon in heldWeapons) {
			currentWeapon.SetActive (false);
		}
		heldWeapons[0].SetActive (true);
	}

	public void ChangeWeapon(bool leftRight){
		heldWeapons [heldWeaponIndex].SetActive (false);
		if (heldWeapons.Count > 0) {
			if (leftRight) {
				heldWeaponIndex -= 1;
				if (heldWeaponIndex < 0) {
					heldWeaponIndex = heldWeapons.Count - 1;
				}
			} else {
				heldWeaponIndex += 1;
				if (heldWeaponIndex > heldWeapons.Count - 1) {
					heldWeaponIndex = 0;
				}
			}
			heldWeapons [heldWeaponIndex].SetActive (true);
			if (heldWeapons [heldWeaponIndex].name == "Barricade") {
				ShowCrosshair (true);
			} else {
				ShowCrosshair (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeWeapon(int weaponNumber){
		heldWeapons.RemoveAt (weaponNumber);
	}

	public string GetWeapon(){
		return heldWeapons[heldWeaponIndex].name;
	}

	void ShowCrosshair(bool showCrosshair){
		if (showCrosshair) {
			if (this.name == "Player 1")
				crosshairUIP1.SetActive (true);
			else
				crosshairUIP2.SetActive (true);
		} else {

			if (this.name == "Player 1")
				crosshairUIP1.SetActive (false);
			else
				crosshairUIP2.SetActive(false);
		}
	}
}
