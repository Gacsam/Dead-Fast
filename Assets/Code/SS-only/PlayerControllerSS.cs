using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerControllerSS : MonoBehaviour
{
	private Camera cam;
	public int gamepadIndex;
	private x360_Gamepad gamepad;

	public Transform projectileSpawn;
	public Transform barricadeSpawn;
	public GameObject bulletPrefab;
	public GameObject grenadePrefab;
	public GameObject barricadePrefab;

	private GameObject heldGrenade;
	private GameObject heldGun;
	private GameObject heldBarricade;
	public int playerHealth = 5;
	private bool invincibilityFrame;
	public int throwForce = 25;

	public void SetGamepadIndex(int newIndex){
		gamepadIndex = newIndex;
	}

	void Start(){
		Component[] thePlayerComponents = this.GetComponentsInChildren<MeshRenderer> ();
		bool gunPicked = false;
		bool grenadePicked = false;
		bool barricadePicked = false;
		foreach (MeshRenderer playerComponents in thePlayerComponents) {
			if (playerComponents.name == "HeldGun" && !gunPicked) {
				heldGun = playerComponents.gameObject;
				gunPicked = true;
			}
			if(playerComponents.name == "HeldGrenade" && !grenadePicked){
				heldGrenade = playerComponents.gameObject;
				grenadePicked = true;
			}
			if(playerComponents.name == "HeldBarricade" && !barricadePicked){
				heldBarricade = playerComponents.gameObject;
				barricadePicked = true;
			}
		}

		// Set player visibility layers
		if (this.name == "Player 1") {
			heldGun.layer = 8;
			heldGrenade.layer = 8;
			heldBarricade.layer = 8;
		}else{
			heldGrenade.layer = 9;
			heldGun.layer = 9;
			heldBarricade.layer = 9;
		}
	}

	void Update()
	{
		gamepad = GamepadManager.Instance.GetGamepad (gamepadIndex);

		if (gamepad.IsConnected) {
			if (gamepad.GetTriggerTap_R ()) {
				string heldWeapon = this.GetComponent<BasicInventory> ().GetWeapon();
				if (heldWeapon == "Grenade") {
					StartCoroutine (GrenadeThrow ());
				} else if (heldWeapon == "Gun") {
					Fire ();
				} else if (heldWeapon == "Barricade") {
					DropBarricade ();
				}
			}
			float rotateY = gamepad.GetStick_R ().X * Time.deltaTime * 150.0f;
			float rotateZ = gamepad.GetStick_R ().Y * Time.deltaTime * 150.0f;
			float moveX = gamepad.GetStick_L ().Y * Time.deltaTime * 3.0f;
			float moveY = gamepad.GetStick_L ().X * Time.deltaTime * 3.0f;
			Mathf.Clamp (rotateZ + transform.rotation.eulerAngles.x, -89, 89);

			transform.eulerAngles += new Vector3 (-rotateZ, rotateY, 0);
			transform.Translate (moveY, 0, moveX);

			if (gamepad.GetButtonDown ("LB")) {
//				this.GetComponent<BasicInventory> ().ChangeWeapon (true);
			}
			if (gamepad.GetButtonDown ("RB")) {
//				this.GetComponent<BasicInventory> ().ChangeWeapon (false);
			}
			gamepad.Refresh ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			string heldWeapon = this.GetComponent<BasicInventory> ().GetWeapon ();
			if (heldWeapon == "Grenade") {
				StartCoroutine (GrenadeThrow ());
			} else if (heldWeapon == "Gun") {
				Fire ();
			} else if (heldWeapon == "Barricade") {
				DropBarricade ();
			}
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			this.GetComponent<BasicInventory> ().ChangeWeapon (true);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			this.GetComponent<BasicInventory> ().ChangeWeapon (false);
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
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
			projectileSpawn.position,
			projectileSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 100;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}

	IEnumerator GrenadeThrow()
	{
		// Hide player-held grenade
		heldGrenade.SetActive (false);
		// Initiate and throw
		GameObject grenade = Instantiate (grenadePrefab, projectileSpawn.position, projectileSpawn.rotation);
		grenade.GetComponent<Rigidbody> ().velocity = grenade.transform.forward * throwForce;
		yield return new WaitForSeconds(0.05f);
		// Show player-held grenade again
		heldGrenade.SetActive (true);
	}

	void DropBarricade(){
		// Create the Barricade from the Barricade Prefab
		Instantiate(
			barricadePrefab, barricadeSpawn.position, barricadeSpawn.rotation);
		this.GetComponent<BasicInventory> ().ChangeWeapon (false);
	}
}