using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	[SerializeField]
	private float timeLeft = 60;
	[SerializeField]
	private Text theTimer, playerOne, playerTwo;

	public void UpdateCount(int pOne, int pTwo){
		playerOne.text = "Zombies in player one's area = " + pOne;
		playerTwo.text = "Zombies in player two's area = " + pTwo;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Plus) || Input.GetKeyDown (KeyCode.Equals)) {
			timeLeft += 5;
		} else if (Input.GetKeyDown (KeyCode.Minus)) {
			timeLeft -= 5;
		}
		if (timeLeft > 0) {
			timeLeft -= Time.deltaTime;
			theTimer.text = "Time left = " + Mathf.RoundToInt (timeLeft) + " seconds";
		} else {
			string winner = FindObjectOfType<ZombieCounter> ().PlayerWinner();
			theTimer.text = "Time end, winner = " + winner;
			this.enabled = false;
		}
	}
}
