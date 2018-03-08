using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBait : MonoBehaviour {

	public enum BaitType{Firework, Grenade, Pig};
	public BaitType thisBait = BaitType.Grenade;
	public float baitLife = 1;
	public int baitDistance = 15;

	void Start(){
		Rigidbody currentRigidbody = gameObject.GetComponent<Rigidbody> ();
		switch (thisBait.ToString()) {
		case "Firework":
			baitLife = 3;
			currentRigidbody.drag = 0.1f;
			break;
		case "Grenade":
			baitLife = 5;
			currentRigidbody.drag = 0.25f;
			break;
		default:
			baitLife = 1;
			break;
		}


	}

	// On walking into a trap
	void OnCollisionEnter(Collision collidedObject){
		if (collidedObject.gameObject.tag == "Zombie") {
			Destroy (this.gameObject);
		}
		if (thisBait == BaitType.Grenade) {
			setBaitLocation (transform.position);
		}
	}

	protected void setBaitLocation(Vector3 newPosition){
		// Get all the existing zombies
		GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
		// Find closest
		NavMeshHit hit;
		if (NavMesh.SamplePosition(newPosition, out hit, 1.0f, NavMesh.AllAreas)) {
			newPosition = hit.position;
		}
		// Loop through every zombie
		foreach (GameObject zombie in zombies) {
			// Check if zombie is close enough
			if (Vector3.Distance (this.transform.position, zombie.transform.position) < baitDistance) {
				zombie.GetComponent<ZombieFarmChase> ().baitedZombie (newPosition);
			}
		}
	}

	// Check for bullet shoot
	void Update(){
		// Explode firework
		if (baitLife > 0) {
			baitLife -= Time.deltaTime;
		} else {
			Destroy (this.gameObject);
			// PLAY ANIMATION AND SOUND
		}

		// Upon shooting a gun
		bool holdingaGun = false;
		if(holdingaGun && Input.GetMouseButtonDown(0)){
			// Get all the existing zombies
			GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
			// Loop through every zombie
			foreach(GameObject zombie in zombies){
				// Get the individual zombie agent
				NavMeshAgent theAgent = zombie.GetComponent<NavMeshAgent> ();
				// Set the agent's new destination to the shot's position
				theAgent.destination = transform.position;
			}
		}
	}
}