using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

	public float maxTime = 60;
	public float timeLeft;
	[SerializeField]
	private Image theTimer;
	[SerializeField]
	private Image zombieCounter;
	[SerializeField]
	private RectTransform playerOne, playerTwo, middle;
	public int zombieCount = 10;
	private float panelSize = 500;
	public int maxZombies = 25;

	public void UpdateCount(int pOne, int pTwo){
		float displaySize = ((float)pOne / (float)zombieCount) * panelSize;
		playerOne.sizeDelta = new Vector2 (displaySize, 25);
		displaySize = ((float)pTwo / (float)zombieCount) * panelSize;
		playerTwo.sizeDelta = new Vector2 (displaySize, 25);
		displaySize = ((float)zombieCount - ((float)pOne + (float)pTwo)) / (float)zombieCount * panelSize;
		middle.sizeDelta = new Vector2 (displaySize, 25); 

		if (pOne > pTwo) {
			playerOne.transform.Find ("Border").gameObject.SetActive (true);
			playerTwo.transform.Find ("Border").gameObject.SetActive (false);
			playerOne.transform.Find ("Border").GetComponent<RectTransform> ().sizeDelta = new Vector2 (playerOne.sizeDelta.x + 10, 35);
		}
		else if (pTwo > pOne) {
			playerOne.transform.Find ("Border").gameObject.SetActive (false);
			playerTwo.transform.Find ("Border").gameObject.SetActive (true);
			playerTwo.transform.Find ("Border").GetComponent<RectTransform> ().sizeDelta = new Vector2 (playerTwo.sizeDelta.x + 10, 35);
		} else {
			playerOne.transform.Find ("Border").gameObject.SetActive (false);
			playerTwo.transform.Find ("Border").gameObject.SetActive (false);
		}
	}

	public void Start(){
		this.maxTime = FindObjectOfType<UIManager> ().maxTime;
		this.zombieCount = FindObjectOfType<UIManager> ().zombieCount;
		this.maxZombies = FindObjectOfType<UIManager> ().maxZombies;
		zombieCounter.transform.Find ("Text").GetComponent<Text> ().text = zombieCount.ToString();
		zombieCounter.transform.Find ("Count").GetComponent<Image> ().fillAmount = 1 - (float)this.zombieCount / (float)this.maxZombies;
		timeLeft = maxTime;
		StartCoroutine (UpdateZombieCount ());
		this.transform.Find ("End").GetComponent<Image> ().gameObject.SetActive (false);
		panelSize = this.transform.Find ("GameUI/CountPanel").GetComponent<RectTransform> ().sizeDelta.x;
		UpdateCount (0, 0);
	}

	IEnumerator UpdateZombieCount(){
		while (true) {
			GameObject[] zombies = GameObject.FindGameObjectsWithTag ("Zombie");
			zombieCount = zombies.Length;
			zombieCounter.transform.Find ("Count").GetComponent<Image> ().fillAmount = 1 - (float)zombieCount / (float)maxZombies;
			zombieCounter.transform.Find ("Text").GetComponent<Text> ().text = zombieCount.ToString();
			yield return new WaitForSeconds (1);
		}
	}

	public void RestartButton(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	// Update is called once per frame
	void Update () {
		theTimer.transform.Find ("Count").GetComponent<Image> ().fillAmount = 1 - (timeLeft / maxTime);
		if (Input.GetKeyDown (KeyCode.Plus) || Input.GetKeyDown (KeyCode.Equals)) {
			timeLeft += 5;
		} else if (Input.GetKeyDown (KeyCode.Minus)) {
			timeLeft -= 5;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene (0);
		}
		if (timeLeft > 0) {
			timeLeft -= Time.deltaTime;
			theTimer.GetComponentInChildren<Text> ().text = Mathf.RoundToInt (timeLeft).ToString ();
		} else {
			string winner = FindObjectOfType<ZombieCounter> ().PlayerWinner();
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
			GameObject[] zombies = GameObject.FindGameObjectsWithTag ("Zombie");
			foreach (GameObject zombie in zombies)
				Destroy (zombie.gameObject);
			FindObjectOfType<ZombieSpawner> ().enabled = false;
			this.enabled = false;
		}
	}
}