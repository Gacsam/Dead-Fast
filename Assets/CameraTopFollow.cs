using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTopFollow : MonoBehaviour {
	[SerializeField]
	private GameObject playerPrefab;
	[SerializeField]
	private float isometricScale = 1;
	[SerializeField]
	private float topDownScale= 2;
	private GameObject[] thePlayers;
	private Camera theCamera;
	private Transform lookAt;
	private bool isTopDown;

	// Use this for initialization
	void Start () {
		theCamera = this.GetComponent<Camera> ();
		lookAt = GameObject.Find ("CameraFollow").transform;

		GameObject currentCharacter = Instantiate (playerPrefab, new Vector3(20, 1, 0), Quaternion.Euler (0, -90, 0));
		currentCharacter.name = "Player 1";
		currentCharacter.gameObject.tag = "Player";
		currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
		currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (1);
		Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
		currentCharacter.GetComponentInChildren<Projector> ().material.color = Color.red;
		currentCharacter = Instantiate (playerPrefab, new Vector3 (-20, 1, 0), Quaternion.Euler (0, 90, 0));
		currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
		currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (2);
		currentCharacter.name = "Player 2";
		currentCharacter.gameObject.tag = "Player";
		thePlayers = GameObject.FindGameObjectsWithTag ("Player");
		Material playerTwoMaterial = new Material(currentCharacter.GetComponentInChildren<Projector> ().material);
		playerTwoMaterial.color = Color.blue;
		currentCharacter.GetComponentInChildren<Projector> ().material = playerTwoMaterial;
		Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
	}
	
	// Update is called once per frame
	void Update () {
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
			float swap = playerDistance.x;
			playerDistance.x = playerDistance.z;
			playerDistance.z = -swap;
			this.transform.position = playerDistance + halfWay.normalized * distance * isometricScale;
			this.transform.LookAt (lookAt);
		}

		if (Input.GetKeyDown (KeyCode.T))
			isTopDown = !isTopDown;
	}
}
