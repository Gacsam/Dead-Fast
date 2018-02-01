using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerControllerSS : MonoBehaviour
{
	private Camera cam;
	public int gamepadIndex;
	private x360_Gamepad gamepad;

	public int playerHealth = 5;
	private bool invincibilityFrame;
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
		int playerLayer = 9;
		switch (this.name) {
		case "Player 2":
			playerLayer = 8;
			break;
		default:
			break;
		}
		playerInventory.SetLayers (playerLayer);
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
				float rotateZ = gamepad.GetStick_R ().Y * Time.deltaTime * 150.0f;
				float moveX = gamepad.GetStick_L ().Y * Time.deltaTime * 3.0f;
				float moveY = gamepad.GetStick_L ().X * Time.deltaTime * 3.0f;
				Mathf.Clamp (rotateZ + transform.rotation.eulerAngles.x, -89, 89);

				transform.eulerAngles += new Vector3 (-rotateZ, rotateY, 0);
				transform.Translate (moveY, 0, moveX);

				if (gamepad.GetButtonDown ("LB")) {
					playerInventory.PrevWeapon ();
				} else if (gamepad.GetButtonDown ("RB")) {
					playerInventory.NextWeapon ();
				}
				if (gamepad.GetButtonDown ("Guide")) {
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


	void OnCollisionEnter(Collision col){
		if (col.collider.tag == "Zombie") {
			// Invincibility frame based on https://answers.unity.com/questions/680668/making-a-player-invulnerable-for-a-few-seconds-get.html
			if (!invincibilityFrame) {
				// Calculate Angle Between the collision point and the player
				Vector3 dir = col.contacts [0].point - transform.position;
				// We then get the opposite (-Vector3) and normalize it
				dir = -dir.normalized;
				// And finally we add force in the direction of dir and multiply it by force. 
				// This will push back the player
				GetComponent<Rigidbody> ().AddForce (dir * 125);
				playerHealth -= 1;
				playerHit ();
				if (playerHealth <= 0) {
					Destroy (this.gameObject);
				}
			}
		}
	}

	IEnumerator playerHit(){
		invincibilityFrame = true;
		yield return new WaitForSeconds(1);
		invincibilityFrame = false;
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
		if (Input.GetAxis ("Mouse Y") != 0) {
			Vector3 newRotation = transform.localEulerAngles;
			newRotation.x = transform.localEulerAngles.x - (Input.GetAxis ("Mouse Y") * rotationSpeed * Time.deltaTime);
			newRotation.x = Mathf.Clamp (((newRotation.x + 540) % 360) - 180, -60, 60);
			transform.localEulerAngles = newRotation;
		}
		if(Input.GetAxis("Mouse X") != 0){
			transform.eulerAngles += Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
		}
		GetComponent<Rigidbody> ().MovePosition (transform.position + transform.forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical"));
	}
}