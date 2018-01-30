using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreen : MonoBehaviour {

	public int playerAmount = 2;
	public GameObject player;

	// Use this for initialization
	void Start () {
		GameObject currentCharacter = Instantiate (player, new Vector3(0, 1, 0), Quaternion.Euler (0, -90, 0));
		Camera currentCamera = currentCharacter.GetComponentInChildren<Camera> ();
		currentCharacter.name = "Player 1";
		currentCharacter.gameObject.tag = "Character";
		currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
		currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (1);
		// Remember to get rid of split-screen audio-listeners
		if (playerAmount != 1) {
			switch (playerAmount) {
			case 2:
				currentCamera.rect = new Rect (0, .5f, 1, .5f);
				currentCamera.cullingMask =currentCamera.cullingMask ^ (1 << 9);
				currentCharacter.transform.position = new Vector3 (25, 1, 0);
				currentCharacter = Instantiate (player, new Vector3 (-25, 1, 0), Quaternion.Euler (0, 90, 0));
//				currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
				currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (2);
				currentCharacter.name = "Player 2";
				currentCharacter.gameObject.tag = "Character";
				currentCamera = currentCharacter.GetComponentInChildren<Camera> ();
				currentCamera.rect = new Rect (0, 0, 1, .5f);
				currentCamera.cullingMask = currentCamera.cullingMask ^ (1 << 8);
				Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
				break;
			case 4:
				currentCharacter.GetComponent<Camera> ().rect = new Rect (0, .5f, .5f, .5f);
				currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (1);
				currentCharacter = Instantiate (player, new Vector3 (-25, 1, 0), Quaternion.Euler (0, 90, 0));
				currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
				currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (2);
				currentCharacter.name = "Player 2";
				currentCharacter.GetComponent<Camera> ().rect = new Rect (.5f, .5f, .5f, .5f);
				Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
				currentCharacter = Instantiate (player, new Vector3 (0, 1, 25), Quaternion.Euler (0, 180, 0));
				currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
				currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (3);
				currentCharacter.name = "Player 3";
				currentCharacter.GetComponent<Camera> ().rect = new Rect (0, 0, .5f, .5f);
				Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
				currentCharacter = Instantiate (player, new Vector3 (0, 1, -25), Quaternion.Euler (0, 0, 0));
				currentCharacter.GetComponent<Rigidbody> ().freezeRotation = true;
				currentCharacter.GetComponent<PlayerControllerSS> ().SetGamepadIndex (4);
				currentCharacter.name = "Player 4";
				currentCharacter.GetComponent<Camera> ().rect = new Rect (.5f, 0, .5f, .5f);
				Destroy (currentCharacter.GetComponentInChildren<AudioListener> ());
				break;
			default:
				return;
			}
		}
	}
}