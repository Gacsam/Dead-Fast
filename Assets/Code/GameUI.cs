using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	[SerializeField]
	private float timeLeft = 60;
	[SerializeField]
	private Text theTimer;
	[SerializeField]
	private RectTransform playerOne, playerTwo, middle;
	private int zombieCount = 20;
	private float panelSize = 500;

	public void UpdateCount(int pOne, int pTwo){
		float displaySize = ((float)pOne / (float)zombieCount) * panelSize;
		playerOne.sizeDelta = new Vector2 (displaySize, 25);
		displaySize = ((float)pTwo / (float)zombieCount) * panelSize;
		playerTwo.sizeDelta = new Vector2 (displaySize, 25);
		displaySize = ((float)zombieCount - ((float)pOne + (float)pTwo)) / (float)zombieCount * panelSize;
		middle.sizeDelta = new Vector2 (displaySize, 25);
	}

	public void Start(){
		panelSize = this.transform.Find ("CountPanel").GetComponent<RectTransform> ().sizeDelta.x;
		StartCoroutine (UpdateZombieCount ());
	}

	IEnumerator UpdateZombieCount(){
		yield return new WaitForSeconds(1);
		GameObject[] zombies = GameObject.FindGameObjectsWithTag ("Zombie");
		zombieCount = zombies.Length;
	}

	// Update is called once per frame
	void Update () {
		Debug.Log (zombieCount);
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
