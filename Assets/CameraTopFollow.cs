using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTopFollow : MonoBehaviour {
	[SerializeField]
	private GameObject playerPrefab;
	[SerializeField]
	private Transform[] playerSpawns;
	private float isometricScale = 1;
	[SerializeField]
	private float topDownScale= 2;
	private GameObject[] thePlayers;
	private Transform lookAt;
	private bool isTopDown;
	[SerializeField]
	private GameObject[] playerPanels;

	private Quaternion defaultRotation;
	private Vector3 defaultPosition;

	public void ChangePlayerFocus(GameObject thePlayer, int playerIndex){
		thePlayers [playerIndex] = thePlayer;
	}

	// Use this for initialization
	void Start () {
		lookAt = GameObject.Find ("CameraFollow").transform;
		CreateTwoPlayers ();
		defaultPosition = this.transform.position;
		defaultRotation = this.transform.rotation;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.M))
			MoveCamera ();
		else {
			Debug.Log ("no m");
			this.transform.position = defaultPosition;
			this.transform.rotation = defaultRotation;
		}
	}

	void CreateTwoPlayers(){
		GameObject currentCharacter = Instantiate (playerPrefab, playerSpawns[0].position, playerSpawns[0].rotation);
		currentCharacter.name = "Player 1";
		currentCharacter.gameObject.tag = "Player";
		currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
		currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (1);
		currentCharacter.GetComponent<advancedInventory> ().playerPanel = playerPanels [0];
		Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
		currentCharacter.GetComponentInChildren<Projector> ().material.color = Color.red;
		currentCharacter = Instantiate (playerPrefab, playerSpawns[1].position, playerSpawns[1].rotation);
		currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
		currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (2);
		currentCharacter.GetComponent<advancedInventory> ().playerPanel = playerPanels [1];
		currentCharacter.name = "Player 2";
		currentCharacter.gameObject.tag = "Player";
		thePlayers = GameObject.FindGameObjectsWithTag ("Player");
		Material playerTwoMaterial = new Material(currentCharacter.GetComponentInChildren<Projector> ().material);
		playerTwoMaterial.color = Color.yellow;
		currentCharacter.GetComponentInChildren<Projector> ().material = playerTwoMaterial;
		Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
	}

	void MoveCamera(){
		Vector3 playerDistance = (thePlayers [0].transform.position - thePlayers [1].transform.position) / 2;
		lookAt.position = thePlayers [0].transform.position - playerDistance;
		float distance = Vector3.Distance (thePlayers [0].transform.position, thePlayers [1].transform.position) / 2;
		if (distance < 3)
			distance = 3;
		if (isTopDown) {
			Vector3 newPosition = new Vector3 (lookAt.position.x, distance * topDownScale , lookAt.position.z);
			this.transform.position = newPosition;
			this.transform.LookAt (lookAt);
		} else {
			Vector3 halfWay = (thePlayers [1].transform.position - thePlayers [0].transform.position);
			if (halfWay.x < 0) {
				halfWay = Vector3.Cross (halfWay, Vector3.forward);
			} else {
				halfWay = Vector3.Cross (halfWay, Vector3.back);
			}
			float swapCameraAngle = playerDistance.x;
			playerDistance.x = playerDistance.z;
			playerDistance.z = -swapCameraAngle;
			this.transform.position = playerDistance + halfWay.normalized * distance * isometricScale;
			this.transform.LookAt (lookAt);
		}

		if (Input.GetKeyDown (KeyCode.T))
			isTopDown = !isTopDown;
	}
}
