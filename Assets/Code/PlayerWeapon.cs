using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : WeaponScript {
	private GameObject heldWeapon;
	public GameObject projectilePrefab;
	private GameObject spawnPrefab;

	// Use this for initialization
	void Start () {
		ammoCount = 1;
		heldWeapon = transform.Find ("held" + thisWeapon.ToString ()).gameObject;
		spawnPrefab = transform.Find ("spawn" + thisWeapon.ToString ()).gameObject;
	}

	public void ShowHideWeapon(){
		heldWeapon.SetActive (!heldWeapon.activeSelf);
	}
}
