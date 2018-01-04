// RandomPointOnNavMesh.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class ZombieNavmeshChase : MonoBehaviour {
	[Tooltip("Zombie wandering range")]
	public float wanderRange = 10.0f;
	[Tooltip("1 in X chances of wandering about")]
	public int inHowManyPatrolChance = 100;
	[Tooltip("Angle of view the zombie notices")]
	public float angleOfView = 90;
	[Tooltip("Distance between player and zombie to notice")]
	public float distanceToNotice = 10;
	[Tooltip("Zombie wandering speed, running is doubled")]
	public float wanderSpeed = 1;
	[Tooltip("How many bullets before death")]
	public float maximumNoiseDistance = 25;
	[Tooltip("How far do zombies hear")]

	public float lifeHits = 3;
	string state = "wander";
	private NavMeshAgent zombieGPS;
	private GameObject[] players;

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

	void OnCollisionEnter(Collision col){
		if (col.collider.tag == "Bullet") {
			lifeHits -= 1;
			wanderSpeed -= 0.25f;
			if (lifeHits <= 0) {
				Destroy (this.gameObject);
			}
		}
	}

	void Start() {
		zombieGPS = this.GetComponent<NavMeshAgent> ();
		zombieGPS.speed = wanderSpeed;
	}

	void Update() {
		if(players == null)
			players = GameObject.FindGameObjectsWithTag("Character");
			foreach (GameObject player in players) {
				float characterDistance = Vector3.Distance (player.transform.position, this.transform.position);
				if (characterDistance <= distanceToNotice) {
					if (LineOfSight (player.transform)) {
						zombieGPS.destination = player.transform.position;
						zombieGPS.stoppingDistance = 0.1f;
						zombieGPS.speed = wanderSpeed * 3;
						state = "chase";
						return;
					} else if (zombieGPS.remainingDistance <= 3) {
						zombieGPS.stoppingDistance = 3;
						state = "wander";
					}
				}
			}
		if (state == "noise" && zombieGPS.remainingDistance <= 3 && Random.Range (0, inHowManyPatrolChance) == 0) {
			zombieGPS.speed = wanderSpeed;
			state = "wander";
			return;
		}
		else if (state == "wander" && zombieGPS.remainingDistance <= 3) {
			if(Random.Range (0, inHowManyPatrolChance) == 0){
			Vector3 point;
				if (RandomPoint (transform.position, wanderRange, out point)) {
					zombieGPS.destination = point;
					return;
				}
			}
		}
	}

	public void baitedZombie(Vector3 baitLocation){
		if (state != "chase") {
			if (Vector3.Distance (this.transform.position, baitLocation) < maximumNoiseDistance) {
				// Set the agent's new destination to the player's position
				zombieGPS.destination = baitLocation;
				zombieGPS.speed = wanderSpeed * 2;
				zombieGPS.stoppingDistance = 1;
				state = "noise";
			}
		}
	}

	bool LineOfSight (Transform target) {
		RaycastHit hit;
		if (Vector3.Angle(target.position - transform.position, transform.forward) <= angleOfView &&
			Physics.Linecast(transform.position, target.position, out hit) &&
			(hit.collider.transform == target || hit.collider.transform.parent == target)) {
			return true;
		}
		return false;
	}
}