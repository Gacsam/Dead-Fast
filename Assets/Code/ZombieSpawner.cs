using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieSpawner : MonoBehaviour {

	public GameObject zombieTemplate;
	[SerializeField]
	private int startZombies = 10;
	[SerializeField]
	private int zombieLimit = 25;
	public Transform[] spawnPoints;

	// Use this for initialization
	void Start () {
		// Create singleton instance
		StartCoroutine (AddZombie ());
		FindObjectOfType<GameUI> ().maxZombies = zombieLimit;
		this.startZombies = FindObjectOfType<UIManager> ().zombieCount;
		this.zombieLimit =FindObjectOfType<UIManager> ().maxZombies;
	}
	
	// Update is called once per frame
	void Update () {
		if (spawnPoints.Length != 0) {
			if (GameObject.FindGameObjectsWithTag ("Zombie").Length < startZombies) {
				Vector3 point = spawnPoints [Random.Range (0, spawnPoints.Length)].position;
				if (RandomPoint (point, 10, out point)) {
					Instantiate (zombieTemplate, point, Quaternion.Euler (new Vector3 (0, Random.Range (0, 360), 0)));
					return;
				} else
					return;
			} else if (GameObject.FindGameObjectsWithTag ("Zombie").Length > startZombies) {
				Destroy (GameObject.FindGameObjectWithTag ("Zombie"));
			}

			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				zombieLimit -= 1;
			} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
				zombieLimit += 1;
			}
		}
	}

	private bool RandomPoint(Vector3 center, float range, out Vector3 result) {
		for (int i = 0; i < 30; i++) {
			Vector3 randomPoint = center + Random.insideUnitSphere * range;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas)) {
				result = hit.position;
				return true;
			}
		}
		result = Vector3.zero;
		return false;
	}

	IEnumerator AddZombie(){
		while (true) {
			yield return new WaitForSeconds (4); // GameObject.FindObjectOfType<GameUI> ().timeLeft / (zombieLimit - startZombies)
			if (startZombies < zombieLimit) {
				startZombies++;
			}
		}
	}
}
