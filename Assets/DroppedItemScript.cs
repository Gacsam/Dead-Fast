﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemScript : MonoBehaviour {

	private WeaponScript thisWeapon;
	[SerializeField]
	private WeaponScript.Weapon theWeapon =  WeaponScript.Weapon.Grenade;
	[SerializeField]
	private int ammo = 1;


	void Start(){
		thisWeapon = new WeaponScript ();
		thisWeapon.SetWeaponType (theWeapon);
		thisWeapon.SetAmmo (ammo);
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Character") {
			col.gameObject.GetComponentInParent<advancedInventory> ().WeaponPickup (thisWeapon);
			Destroy (this.gameObject);
		}
	}
}
