using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigControl : ZombieBait {

	static CameraTopFollow cameraScript;
	private x360_Gamepad gamepadController;
	[SerializeField]
	static private float timeControlling = 15f;
	[SerializeField]
	private float rotationSpeed = 5;

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
		rbMove = transform.TransformDirection (rbMove);
		GetComponent<Rigidbody> ().MovePosition(this.transform.position + rbMove * 3 * Time.deltaTime);
		if (rbMove != Vector3.zero) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (rbMove.normalized), 0.1f);
		}

		// Oink
		if (Input.GetMouseButtonDown (0) && !didOink) {
			setBaitLocation (this.transform.position);
			didOink = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isControlled) {
			MovePlayer ();

			if (gamepadController.IsConnected) {
				if (gamepadController.GetTriggerTap_R () && !didOink) {
					setBaitLocation (this.transform.position);
					didOink = true;
				}
				float rotateY = this.gamepadController.GetStick_R ().X * Time.deltaTime * 150.0f;
				float moveX = this.gamepadController.GetStick_L ().Y * Time.deltaTime * 3.0f;
				float moveY = this.gamepadController.GetStick_L ().X * Time.deltaTime * 3.0f;

				// Rotate towards for gamepad, needs testing
				//				Vector3 rbMove = new Vector3 (moveX, 0, moveY);
				//				if(rbMove != Vector3.zero) 
				//					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rbMove.normalized), 0.2f);

				transform.RotateAround (transform.position, Vector3.right, rotateY * rotationSpeed);
				transform.Translate (moveY, 0, moveX);
				gamepadController.Refresh ();
			}

		}
	}
}