using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigControl : ZombieBait {

	private x360_Gamepad gamepadController;
	[SerializeField]
	private float pigSpeed = 3;
	[SerializeField]
	static private float timeControlling = 15f;

	private bool isControlled = true;
	private bool didOink = false;
	private bool keyboardMouse = false;

	private PlayerControllerSS controllingPlayer;

	[SerializeField]
	private AudioSource pigSound;
	[SerializeField]
	private Material firstMaterial;

	void Start(){
		baitDistance = 5;
		pigSound = GetComponent<AudioSource> ();
	}


	void OnCollisionEnter(Collision col){
		if(col.collider.tag == "Zombie")
			RemoveControl ();
	}

	public IEnumerator TakeControl(x360_Gamepad theGamepad, GameObject thePlayer){
		this.gamepadController = theGamepad;
		controllingPlayer = thePlayer.GetComponentInChildren<PlayerControllerSS>(true);
		Debug.Log (controllingPlayer.name);
		if (controllingPlayer.name == "Player 1") {
			this.GetComponentInChildren<Projector> ().material = firstMaterial;
		}
		yield return new WaitForSeconds (timeControlling);
	}

	void RemoveControl(){
		if(this.transform.Find("Projector"))
			Destroy (this.transform.Find("Projector").gameObject);
		
		if (isControlled)
			controllingPlayer.enabled = true;
		
		isControlled = false;
		this.enabled = false;
	}

	void MovePlayer(){
		if (Input.GetMouseButtonDown (0) && !didOink) {
			Oink ();
		}

		if (Input.GetMouseButtonDown (1)) {
			RemoveControl ();
		}

		Vector3 rbMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (rbMove != Vector3.zero) {
			rbMove = Camera.main.transform.Find("CameraDirection").transform.InverseTransformDirection (rbMove);
			rbMove.y = 0;
			GetComponent<Rigidbody> ().MovePosition (this.transform.position + rbMove.normalized * pigSpeed * Time.deltaTime);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (rbMove), 0.75f);
		}
	}

	void Oink(){
		Debug.Log ("oink");
		pigSound.Play ();
		setBaitLocation (this.transform.position);
		didOink = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (isControlled) {
			if(keyboardMouse)
				MovePlayer ();
			// Oink
			if (gamepadController.IsConnected) {
				if (gamepadController.GetTriggerTap_R () && !didOink) {
					Oink ();
				}
				if(gamepadController.GetTriggerTap_L()){
					RemoveControl();
				}

				Vector3 rbMove = new Vector3 (this.gamepadController.GetStick_L ().X, 0, this.gamepadController.GetStick_L ().Y);
				if (rbMove != Vector3.zero) {
					rbMove = Camera.main.transform.Find("CameraDirection").transform.TransformDirection (rbMove);
					rbMove.y = 0;
					GetComponent<Rigidbody> ().MovePosition (this.transform.position + rbMove * pigSpeed * Time.deltaTime);
					transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (rbMove), 0.075f);
				}

				gamepadController.Refresh ();
			}

		}
	}
}