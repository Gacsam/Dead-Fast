﻿using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour {

	public GameObject zombieTemplate;
	public int zombieLimit = 25;
	public Transform[] spawnPoints;

	// Use this for initialization
	void Start () {
		// Create singleton instance
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectsWithTag ("Zombie").Length < zombieLimit) {
			Vector3 point = spawnPoints[Random.Range(0, spawnPoints.Length-1)].position;
			if (RandomPoint (point, 10, out point)) {
				Instantiate (zombieTemplate, point, Quaternion.Euler (new Vector3 (0, Random.Range (0, 360), 0)));
				return;
			} else
				return;
		} else if (GameObject.FindGameObjectsWithTag ("Zombie").Length > zombieLimit) {
			Destroy (GameObject.FindGameObjectWithTag ("Zombie"));
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			zombieLimit -= 1;
		}else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			zombieLimit += 1;
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
