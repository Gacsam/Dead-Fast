using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour {
	public enum Weapon{Gun, Grenade, Firework, Melee, Trap};
	public Weapon thisWeapon = Weapon.Gun;
	public int ammoCount;
	private GameObject heldWeapon;
	public GameObject projectilePrefab;
	private GameObject spawnPrefab;

	// Use this for initialization
	void Start () {
		ammoCount = 1;
		heldWeapon = transform.Find ("held" + thisWeapon.ToString ()).gameObject;
		spawnPrefab = transform.Find ("spawn" + thisWeapon.ToString ()).gameObject;
	}

	public Weapon GetWeaponType(){
		return thisWeapon;
	}

	public void SetWeaponType(Weapon newWeapon){
		thisWeapon = newWeapon;
	}

	public void ShowHideWeapon(){
		heldWeapon.SetActive (!heldWeapon.activeSelf);
	}

	public int GetAmmo(){
		return ammoCount;
	}

	public void SetAmmo(int newAmmo){
		ammoCount = newAmmo;
	}

	public void ModAmmo(int moreAmmo){
		ammoCount += moreAmmo;
	}
}
