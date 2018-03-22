using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerControllerSS : MonoBehaviour
{
	public int gamepadIndex;
	private Animator myAnimator;
	private x360_Gamepad gamepad;
	[SerializeField]
	private int throwForce = 25;
	private advancedInventory playerInventory;
	[SerializeField]
	private int moveSpeed = 5;
	[SerializeField]
	private float weaponDelay = 0.25f;
	private bool notUsingWeapon = true;

	[Tooltip("Are you using keyboard/mouse?")]
	public bool mouseKeyboard = false;

	[Tooltip("Are you using a gamepad?")]
	public bool gamepadPlugged = true;

	private Rigidbody theRigidbody;

	public void SetGamepadIndex(int newIndex){
		gamepadIndex = newIndex;
	}


	void Start(){
		playerInventory = this.GetComponent<advancedInventory> ();
		myAnimator = GetComponentInChildren<Animator> ();
		theRigidbody = GetComponent<Rigidbody> ();
	}

	void Update ()
	{
		if (gamepadPlugged) {
			gamepad = GamepadManager.Instance.GetGamepad (gamepadIndex);
			if (gamepad.IsConnected) {
				if (gamepad.GetTriggerTap_R ()) {
					WeaponScript.Weapon theWeapon = playerInventory.getWeapon ();
					if (playerInventory.GetWeaponAmmo () > 0) {
						StartCoroutine (UseWeapon (theWeapon));
					}
					if (playerInventory.GetWeaponAmmo () == 0 && playerInventory.dynamicInventory) {
						playerInventory.NextWeapon ();
					}
				}

				Vector3 rbMove = new Vector3 (this.gamepad.GetStick_L ().X, 0, this.gamepad.GetStick_L ().Y);
				myAnimator.SetFloat ("speed", rbMove.magnitude);
				if (rbMove != Vector3.zero) {
					rbMove = Camera.main.transform.Find("CameraDirection").transform.TransformDirection (rbMove);
					rbMove.y = 0;
					theRigidbody.MovePosition (this.transform.position + rbMove * moveSpeed * Time.deltaTime);
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

	IEnumerator UseWeapon(WeaponScript.Weapon theWeapon){
		if (notUsingWeapon) {
			playerInventory.HideWeapon();
			notUsingWeapon = false;
			myAnimator.SetBool ("fire", true);
			myAnimator.SetFloat ("speed", 0);
			if (theWeapon == WeaponScript.Weapon.Grenade) {
				GrenadeThrow ();
			} else if (theWeapon == WeaponScript.Weapon.Barricade) {
				DropBarricade ();
			} else if (theWeapon == WeaponScript.Weapon.Pig) {
				PlayPig ();
			} else
				Debug.Log ("No Weapon");
			playerInventory.ModAmmo (-1);
			if (playerInventory.GetWeaponAmmo () == 0) {
				playerInventory.HideWeapon ();
				if (playerInventory.dynamicInventory) {
					playerInventory.NextWeapon ();
				}
				yield return null;
			} else {
				yield return new WaitForSeconds (weaponDelay);
				playerInventory.ShowWeapon ();
			}
			notUsingWeapon = true;
		}
	}

	void GrenadeThrow()
	{
		// Initiate and throw
		GameObject grenade = Instantiate(
			playerInventory.GetProjectilePrefab("Grenade"),
			playerInventory.GetProjectileSpawn("Grenade").position,
			playerInventory.GetProjectileSpawn("Grenade").rotation);
		grenade.GetComponent<Rigidbody> ().velocity = grenade.transform.forward * throwForce;
		Vector3 randomTorque = new Vector3 (Random.Range (0, 15), Random.Range (0, 15), Random.Range (0, 15));
		grenade.GetComponent<Rigidbody> ().AddTorque(randomTorque * throwForce);
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
		this.enabled = false;
	}

	void MovePlayer(){
		Vector3 rbMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		myAnimator.SetFloat ("speed", rbMove.magnitude);
		if (rbMove != Vector3.zero) {
			rbMove = Camera.main.transform.Find("CameraDirection").transform.TransformDirection (rbMove);
			rbMove.y = 0;
			theRigidbody.MovePosition (this.transform.position + rbMove * moveSpeed * Time.deltaTime);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (rbMove), moveSpeed * 0.02f);
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			playerInventory.PrevWeapon ();
		} else if (Input.GetKeyDown (KeyCode.E)) {
			playerInventory.NextWeapon ();
		} else if (Input.GetMouseButtonDown (0)) {
			WeaponScript.Weapon theWeapon = playerInventory.getWeapon ();
			if (playerInventory.GetWeaponAmmo () > 0) {
					StartCoroutine (UseWeapon (theWeapon));
			}
		}
	}
}