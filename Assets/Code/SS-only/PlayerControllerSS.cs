using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerControllerSS : MonoBehaviour
{
	public int gamepadIndex;
	private x360_Gamepad gamepad;

	public int throwForce = 25;
	private advancedInventory playerInventory;
	public int rotationSpeed = 50;
	public int moveSpeed = 3;
	public int jumpHeight = 3;

	[Tooltip("Are you using keyboard/mouse?")]
	public bool mouseKeyboard = false;

	[Tooltip("Are you using a gamepad?")]
	public bool gamepadPlugged = true;
	

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
						if (theWeapon == WeaponScript.Weapon.Grenade) {
							StartCoroutine (GrenadeThrow ());
						} else if (theWeapon == WeaponScript.Weapon.Barricade) {
							DropBarricade ();
						} else if (theWeapon == WeaponScript.Weapon.Pig) {
							PlayPig ();
						} else
							Debug.Log ("No Weapon");
					}
				}
				if (playerInventory.GetWeaponAmmo () == 0 && playerInventory.dynamicInventory) {
					playerInventory.NextWeapon ();
				}

				Vector3 rbMove = new Vector3 (this.gamepad.GetStick_L ().X, 0, this.gamepad.GetStick_L ().Y);
				if (rbMove != Vector3.zero) {
					rbMove = Camera.main.transform.TransformDirection (rbMove);
					rbMove.y = 0;
					GetComponent<Rigidbody> ().MovePosition (this.transform.position + rbMove * moveSpeed * Time.deltaTime);
					transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (rbMove), 0.075f);
				}

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
			MovePlayer ();
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		} else if (Input.GetKeyDown (KeyCode.K)) {
			mouseKeyboard = !mouseKeyboard;
		}

	}

	void TestRumble()
	{
		//                timer            power         fade
		gamepad.AddRumble(1.0f, new Vector2(0.9f, 0.9f), 0.5f);
		gamepad.AddRumble(2.5f, new Vector2(0.5f, 0.5f), 0.2f);
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


	void DropBarricade(){
		// Create the Barricade from the Barricade Prefab
		Instantiate(
			playerInventory.GetProjectilePrefab("Barricade"),
			playerInventory.GetProjectileSpawn("Barricade").position,
			playerInventory.GetProjectileSpawn("Barricade").rotation);
	}

	void PlayPig(){
		GameObject pig = Instantiate (
			                 playerInventory.GetProjectilePrefab ("Pig"),
			                 playerInventory.GetProjectileSpawn ("Pig").position,
			                 playerInventory.GetProjectileSpawn ("Pig").rotation);
		pig.GetComponentInChildren<PigControl> ().StartCoroutine(pig.GetComponentInChildren<PigControl> ().TakeControl (gamepad, this.gameObject));
		Projector theProjector = this.gameObject.GetComponentInChildren<Projector> ();
		Projector newProjector = pig.transform.Find ("Projector").gameObject.AddComponent<Projector> ();
		newProjector.material = theProjector.material;
		newProjector.orthographic = theProjector.orthographic;
		newProjector.orthographicSize = theProjector.orthographicSize;
		newProjector.ignoreLayers = theProjector.ignoreLayers;
		theProjector.enabled = false;
		this.enabled = false;
	}

	void MovePlayer(){
		Vector3 rbMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (rbMove != Vector3.zero) {
			rbMove = Camera.main.transform.TransformDirection (rbMove);
			rbMove.y = 0;
			GetComponent<Rigidbody> ().MovePosition (this.transform.position + rbMove * moveSpeed * Time.deltaTime);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (rbMove), 0.075f);
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			playerInventory.PrevWeapon ();
		} else if (Input.GetKeyDown (KeyCode.E)) {
			playerInventory.NextWeapon ();
		} else if (Input.GetMouseButtonDown (0)) {
			WeaponScript.Weapon theWeapon = playerInventory.getWeapon ();
			if (playerInventory.GetWeaponAmmo () > 0) {
				if (theWeapon == WeaponScript.Weapon.Grenade) {
					StartCoroutine (GrenadeThrow ());
				} else if (theWeapon == WeaponScript.Weapon.Barricade) {
					DropBarricade ();
				} else if (theWeapon == WeaponScript.Weapon.Pig) {
					PlayPig ();
				} else
					Debug.Log ("No Weapon");
			}
			if (playerInventory.GetWeaponAmmo () == 0 && playerInventory.dynamicInventory) {
				playerInventory.NextWeapon ();
			}
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			Vector3 newVelocity = GetComponent<Rigidbody> ().velocity;
			newVelocity.y = jumpHeight;
			GetComponent<Rigidbody> ().velocity = newVelocity;
		}
	}
}