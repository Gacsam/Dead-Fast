using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCounter : MonoBehaviour {

	private static int playerOneZombies, playerTwoZombies;
	[SerializeField]
	private static GameUI theUI;

	// Use this for initialization
	void Start () {
		playerOneZombies = 0;
		playerTwoZombies = 0;
		theUI = FindObjectOfType<GameUI> ();
	}

	public string PlayerWinner(){
		if (playerOneZombies < playerTwoZombies)
			return "Red";
		else if (playerOneZombies > playerTwoZombies)
			return "Blue";
		else
			return "Draw";
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Zombie") {
			if (this.name == "playerOneArea") {
				playerOneZombies += 1;
			} else {
				playerTwoZombies += 1;
			}
		}
		theUI.UpdateCount (playerOneZombies, playerTwoZombies);
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Zombie") {
			if (this.name == "playerOneArea") {
				playerOneZombies -= 1;
			} else {
				playerTwoZombies -= 1;
			}
		}
		theUI.UpdateCount (playerOneZombies, playerTwoZombies);
	}
}
