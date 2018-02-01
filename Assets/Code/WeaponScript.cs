using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript {
	public enum Weapon{Gun, Grenade, Firework, Melee, Trap, Barricade};

	[SerializeField]
	private Weapon thisWeapon;
	[SerializeField]
	private int ammoCount;

	// Use this for initialization
	void Start () {
		ammoCount = 1;

	}

	public Weapon GetWeaponType(){
		return thisWeapon;
	}

	public void SetWeaponType(Weapon newWeapon){
		thisWeapon = newWeapon;
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
