using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBait : MonoBehaviour {

	public enum BaitType{Firework, Grenade, Pig};
	public BaitType thisBait = BaitType.Grenade;
	public int baitDistance = 150;

	void Start(){
		Rigidbody currentRigidbody = gameObject.GetComponent<Rigidbody> ();
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
		if (NavMesh.SamplePosition(newPosition, out hit, 25, NavMesh.AllAreas)) {
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
}