using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerControllerSS : MonoBehaviour
{
	private Camera cam;
	public int gamepadIndex;
	private x360_Gamepad gamepad;

	public int throwForce = 25;
	private advancedInventory playerInventory;
	public int rotationSpeed = 50;
	public int moveSpeed = 3;
	public int jumpHeight = 3;

	[Tooltip("Are you using keyboard/mouse?")]
	public bool mouseKeyboard = true;

	[Tooltip("Are you using a gamepad?")]
	public bool gamepadPlugged = false;
	

	public void SetGamepadIndex(int newIndex){
		gamepadIndex = newIndex;
	}


	void Start(){
		playerInventory = this.GetComponent<advancedInventory> ();
	}

	void Update ()
	{
		if (gamepadPlugged) {
			gamepad = GamepadManager.Instance.GetGamepad (gamepadIndex);
			if (gamepad.IsConnected) {
				if (gamepad.GetTriggerTap_R ()) {
					WeaponScript.Weapon theWeapon = playerInventory.getWeapon ();
					if (playerInventory.GetWeaponAmmo () > 0) {
						if (theWeapon == WeaponScript.Weapon.Melee) {
							MeleeAttack ();
						} else if (theWeapon == WeaponScript.Weapon.Grenade) {
							StartCoroutine (GrenadeThrow ());
						} else if (theWeapon == WeaponScript.Weapon.Gun) {
							Fire ();
						} else if (theWeapon == WeaponScript.Weapon.Barricade) {
							DropBarricade ();
						} else
							Debug.Log ("No Weapon");
					}
				}
				if (playerInventory.GetWeaponAmmo () == 0 && playerInventory.isDynamicInventory ()) {
					playerInventory.NextWeapon ();
				
					if (!playerInventory.isMelee ())
						playerInventory.ModAmmo (-1);
				}
				float rotateY = gamepad.GetStick_R ().X * Time.deltaTime * 150.0f;
				float moveX = gamepad.GetStick_L ().Y * Time.deltaTime * 3.0f;
				float moveY = gamepad.GetStick_L ().X * Time.deltaTime * 3.0f;
				Vector3 rbMove = new Vector3 (moveX, 0, moveY);
//				GetComponent<Rigidbody> ().MovePosition(transform.position + rbMove * moveSpeed * Time.deltaTime);
				transform.LookAt(transform.position + rbMove);
				transform.eulerAngles += new Vector3 (0, rotateY, 0);
				transform.Translate (moveY, 0, moveX);

				if (gamepad.GetButtonDown ("LB")) {
					playerInventory.PrevWeapon ();
				} else if (gamepad.GetButtonDown ("RB")) {
					playerInventory.NextWeapon ();
				}
				if (gamepad.GetButtonDown ("Start")) {
					SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
				}
				gamepad.Refresh ();
			}
		}

		if (mouseKeyboard) {
			if (Input.GetMouseButtonDown (0)) {
				WeaponScript.Weapon theWeapon = playerInventory.getWeapon ();
				if (playerInventory.GetWeaponAmmo () > 0) {
					if (theWeapon == WeaponScript.Weapon.Melee) {
						StartCoroutine(MeleeAttack ());
					} else if (theWeapon == WeaponScript.Weapon.Grenade) {
						StartCoroutine (GrenadeThrow ());
					} else if (theWeapon == WeaponScript.Weapon.Gun) {
						Fire ();
					} else if (theWeapon == WeaponScript.Weapon.Barricade) {
						DropBarricade ();
					} else
						Debug.Log ("No Weapon");
				}
				if (playerInventory.GetWeaponAmmo () == 0 && playerInventory.isDynamicInventory ()) {
					playerInventory.NextWeapon ();
				}
					if (!playerInventory.isMelee ())
						playerInventory.ModAmmo (-1);
				}
			if (Input.GetKeyDown (KeyCode.Q)) {
				playerInventory.PrevWeapon ();
			} else if (Input.GetKeyDown (KeyCode.E)) {
				playerInventory.NextWeapon ();
			} else if (Input.GetKeyDown (KeyCode.Space)) {
				Vector3 newVelocity = GetComponent<Rigidbody> ().velocity;
				newVelocity.y = jumpHeight;
				GetComponent<Rigidbody> ().velocity = newVelocity;
			} else if (Input.GetKeyDown (KeyCode.R)) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			}
			MovePlayer ();
			}
	}

	void TestRumble()
	{
		//                timer            power         fade
		gamepad.AddRumble(1.0f, new Vector2(0.9f, 0.9f), 0.5f);
		gamepad.AddRumble(2.5f, new Vector2(0.5f, 0.5f), 0.2f);
	}


	void Fire()
	{
		// Create the Bullet from the Bullet Prefab
		GameObject bullet = Instantiate(
			playerInventory.GetProjectilePrefab("Bullet"),
			playerInventory.GetProjectileSpawn("Gun").position,
			playerInventory.GetProjectileSpawn("Gun").rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 100;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}

	IEnumerator GrenadeThrow()
	{
		// Hide player-held grenade
		playerInventory.ShowHideWeapon();
		// Initiate and throw
		GameObject grenade = Instantiate(
			playerInventory.GetProjectilePrefab("Grenade"),
			playerInventory.GetProjectileSpawn("Grenade").position,
			playerInventory.GetProjectileSpawn("Grenade").rotation);
		grenade.GetComponent<Rigidbody> ().velocity = grenade.transform.forward * throwForce;
		Vector3 randomTorque = new Vector3 (Random.Range (0, 15), Random.Range (0, 15), Random.Range (0, 15));
		grenade.GetComponent<Rigidbody> ().AddTorque(randomTorque * throwForce);
		grenade.transform.localScale = this.transform.localScale * 10;
		yield return new WaitForSeconds(0.05f);
		// Show player-held grenade again
		playerInventory.ShowHideWeapon();
	}

	IEnumerator MeleeAttack(){
		Animator anim = transform.Find ("heldMelee").GetComponentInChildren<Animator> (true);
		anim.SetBool ("isAttacking", true);
		yield return new WaitForSeconds(0.5f);
		anim.SetBool ("isAttacking", false);
	}

	void DropBarricade(){
		// Create the Barricade from the Barricade Prefab
		Instantiate(
			playerInventory.GetProjectilePrefab("Barricade"),
			playerInventory.GetProjectileSpawn("Barricade").position,
			playerInventory.GetProjectileSpawn("Barricade").rotation);
	}

	void MovePlayer(){
		Vector3 rbMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		rbMove = transform.TransformDirection (rbMove);
		GetComponent<Rigidbody> ().MovePosition(transform.position + rbMove * moveSpeed * Time.deltaTime);
	}
}