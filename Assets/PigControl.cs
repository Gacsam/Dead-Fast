using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigControl : ZombieBait {

	private x360_Gamepad gamepadController;
	private float pigSpeed = 3;
	[SerializeField]
	static private float timeControlling = 15f;
	[SerializeField]

	private bool isControlled = true;
	private bool didOink = false;

	GameObject controllingPlayer;

	void Start(){
		foreach (GameObject theZombie in GameObject.FindGameObjectsWithTag ("Zombie")) {
		}
	}


	void OnCollisionEnter(Collision col){
		if (col.collider.tag == "Zombie") {
			if (isControlled) {
				controllingPlayer.GetComponentInChildren<PlayerControllerSS>(true).enabled = true;
				controllingPlayer.gameObject.GetComponentInChildren<Projector> ().enabled = true;
			}
			Destroy (this.gameObject);
		}
	}

	public IEnumerator TakeControl(x360_Gamepad theGamepad, GameObject thePlayer){
		this.gamepadController = theGamepad;
		controllingPlayer = thePlayer;

		yield return new WaitForSeconds (timeControlling);
		controllingPlayer.GetComponentInChildren<PlayerControllerSS>(true).enabled = true;
		controllingPlayer.gameObject.GetComponentInChildren<Projector> ().enabled = true;
		Destroy (this.transform.Find("Projector").gameObject);
		isControlled = false;
		StartCoroutine (RandomOink ());
		this.enabled = false;
	}

	IEnumerator RandomOink(){
		yield return new WaitForSeconds (Random.Range(5, 30));
		setBaitLocation (this.transform.position);
	}

	void MovePlayer(){
		Vector3 rbMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (rbMove != Vector3.zero) {
			rbMove = Camera.main.transform.Find("CameraDirection").transform.InverseTransformDirection (rbMove);
			rbMove.y = 0;
			GetComponent<Rigidbody> ().MovePosition (this.transform.position + rbMove.normalized * pigSpeed * Time.deltaTime);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (rbMove), 0.75f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isControlled) {
			MovePlayer ();


			// Oink
			if (Input.GetMouseButtonDown (0) && !didOink) {
				setBaitLocation (this.transform.position);
				didOink = true;
			}

			if (gamepadController.IsConnected) {
				if (gamepadController.GetTriggerTap_R () && !didOink) {
					setBaitLocation (this.transform.position);
					didOink = true;
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