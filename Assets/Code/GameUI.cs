using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

	[SerializeField]
	private float maxTime = 60;
	private float timeLeft;
	[SerializeField]
	private Image theTimer;
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
		timeLeft = maxTime;
		panelSize = this.transform.Find ("CountPanel").GetComponent<RectTransform> ().sizeDelta.x;
		StartCoroutine (UpdateZombieCount ());
		Button[] allButtons = GetComponentsInChildren<Button> ();
		foreach (Button theButton in allButtons) {
			theButton.onClick.AddListener (() => ButtonClick (theButton.name));
		}
		this.transform.Find ("End").GetComponent<Image> ().gameObject.SetActive (false);
	}

	IEnumerator UpdateZombieCount(){
		yield return new WaitForSeconds(1);
		GameObject[] zombies = GameObject.FindGameObjectsWithTag ("Zombie");
		zombieCount = zombies.Length;
	}

	void ButtonClick(string theButton){
		Debug.Log ("PRESSED");
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	// Update is called once per frame
	void Update () {
		theTimer.fillAmount = 1 - (timeLeft / maxTime);
		if (Input.GetKeyDown (KeyCode.Plus) || Input.GetKeyDown (KeyCode.Equals)) {
			timeLeft += 5;
		} else if (Input.GetKeyDown (KeyCode.Minus)) {
			timeLeft -= 5;
		}
		if (timeLeft > 0) {
			timeLeft -= Time.deltaTime;
			theTimer.GetComponentInChildren<Text>().text = Mathf.RoundToInt (timeLeft).ToString();
		} else {
			string winner = FindObjectOfType<ZombieCounter> ().PlayerWinner();
			Destroy(theTimer.GetComponentInChildren<Text> ());
			Destroy (theTimer.transform.Find ("Circle").gameObject);
			Image endMenu = this.transform.Find ("End").GetComponent<Image> ();
			endMenu.gameObject.SetActive (true);
			Text endText = endMenu.GetComponentInChildren<Text> ();
			if (winner == "Draw") {
				endMenu.color = Color.black;
				endText.text = "Both houses were overrun by zombies";
			} else if (winner == "Red") {
				endMenu.color = Color.red;
				endText.text = "Blue's house was overrun by zombies";
			} else {
				endMenu.color = Color.blue;
				endText.text = "Red's house was overrun by zombies";
			}
			this.enabled = false;
		}
	}
}