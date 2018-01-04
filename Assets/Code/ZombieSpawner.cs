using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour {

	public GameObject zombieTemplate;
	public int zombieLimit = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectsWithTag ("Zombie").Length < zombieLimit) {
			Vector3 point;
			if (RandomPoint (Vector3.up, 10, out point)) {
				Instantiate (zombieTemplate, point, Quaternion.Euler (new Vector3 (0, Random.Range (0, 360), 0)));
				return;
			} else
				return;
		}
	}

	bool RandomPoint(Vector3 center, float range, out Vector3 result) {
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
}
