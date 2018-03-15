using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour {

	/*
	 * To be used as a component of the spawn point, as it's using its transform
	 */

	[SerializeField]
	private GameObject theAnimal;
	[SerializeField]
	private float timeBetweenSpawns = 5;
	[SerializeField]
	private float maxSpawnDelay = 10;
	[SerializeField]
	private bool randomSpawnDelay = false;

	// Use this for initialization
	void Start () {
		StartCoroutine (SpawnAnimal ());
	}

	IEnumerator SpawnAnimal(){
		float theTime = timeBetweenSpawns;
		if (randomSpawnDelay) {
			theTime = Random.Range ((int)timeBetweenSpawns, (int)maxSpawnDelay);
		}
		yield return new WaitForSeconds (theTime);
		Instantiate (theAnimal, transform.position, transform.rotation);
	}
}
