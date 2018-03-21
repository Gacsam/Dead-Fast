using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class PlayerControllerLAN : NetworkBehaviour
{
	public int gamepadIndex;
	private x360_Gamepad gamepad;
	private int throwForce = 25;
	[SerializeField]
	private advancedInventory playerInventory;
	[SerializeField]
	private int moveSpeed = 5;
	[SerializeField]
	private float weaponDelay = 0.25f;
	private bool notUsingWeapon = true;

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
		if (!isLocalPlayer)
			return;
		
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
				if (rbMove != Vector3.zero) {
					rbMove = Camera.main.transform.Find("CameraDirection").transform.TransformDirection (rbMove);
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

	IEnumerator UseWeapon(WeaponScript.Weapon theWeapon){
		if (notUsingWeapon) {
			playerInventory.HideWeapon();
			notUsingWeapon = false;
			if (theWeapon == WeaponScript.Weapon.Grenade) {
				CmdGrenadeThrow ();
			} else if (theWeapon == WeaponScript.Weapon.Barricade) {
				CmdDropBarricade ();
			} else if (theWeapon == WeaponScript.Weapon.Pig) {
				CmdPlayPig ();
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

	[Command]
	void CmdGrenadeThrow()
	{
		// Initiate and throw
		GameObject grenade = Instantiate(
			playerInventory.GetProjectilePrefab("Grenade"),
			playerInventory.GetProjectileSpawn("Grenade").position,
			playerInventory.GetProjectileSpawn("Grenade").rotation);
		grenade.GetComponent<Rigidbody> ().velocity = grenade.transform.forward * throwForce;
		Vector3 randomTorque = new Vector3 (Random.Range (0, 15), Random.Range (0, 15), Random.Range (0, 15));
		grenade.GetComponent<Rigidbody> ().AddTorque(randomTorque * throwForce);
		NetworkServer.Spawn (grenade);
	}

	[Command]
	void CmdDropBarricade(){
		// Create the Barricade from the Barricade Prefab
		GameObject barricade = Instantiate(
			playerInventory.GetProjectilePrefab("Barricade"),
			playerInventory.GetProjectileSpawn("Barricade").position,
			playerInventory.GetProjectileSpawn("Barricade").rotation);
		NetworkServer.Spawn (barricade);
	}

	[Command]
	void CmdPlayPig(){
		GameObject pig = Instantiate (
			playerInventory.GetProjectilePrefab ("Pig"),
			playerInventory.GetProjectileSpawn ("Pig").position,
			playerInventory.GetProjectileSpawn ("Pig").rotation);
		pig.GetComponentInChildren<PigControl> ().StartCoroutine(pig.GetComponentInChildren<PigControl> ().TakeControl (gamepad, this.gameObject));
		NetworkServer.Spawn (pig);
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
			rbMove = Camera.main.transform.Find("CameraDirection").transform.TransformDirection (rbMove);
			rbMove.y = 0;
			GetComponent<Rigidbody> ().MovePosition (this.transform.position + rbMove * moveSpeed * Time.deltaTime);
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