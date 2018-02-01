// RandomPointOnNavMesh.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class ZombieNavmeshChase : MonoBehaviour {
	[Tooltip("Zombie wandering range")]
	public float wanderRange = 10.0f;
	[Tooltip("1 in X chances of wandering about")]
	public int inHowManyPatrolChance = 1;
	[Tooltip("Angle of view the zombie notices")]
	public float angleOfView = 90;
	[Tooltip("Distance between player and zombie to notice")]
	public float distanceToNotice = 10;
	[Tooltip("Zombie wandering speed, running is doubled")]
	public float wanderSpeed = 1;
	[Tooltip("How far do zombies hear")]
	public float maximumNoiseDistance = 25;
	[Tooltip("How many bullets to kill")]
	public int lifeHits = 3;
	[Tooltip("Maximum time zombies don't do anything")]
	public int maximumIdle = 3;
	[Tooltip("After what time do zombies stop looking around the area")]
	public int searchTime = 10;
	public enum States{Idle, Wander, Search, Noise, Chase};
	public States zombieState;
	private NavMeshAgent zombieGPS;
	private GameObject[] players;
	private GameObject zombieTarget;
	private bool wanderingSearching = false;
	private Coroutine runningCoroutine;

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
			Destroy(col.gameObject);
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
		zombieState = States.Wander;
		runningCoroutine = StartCoroutine(RandomiseWander (this.transform.position));
		StopCoroutine (runningCoroutine);
		if(players == null)
			players = GameObject.FindGameObjectsWithTag("Character");
	}

	void Update() {
		// In order of importance, chase, search, noise, wander, idle
		if (zombieState == States.Chase) {
			float characterDistance = Vector3.Distance (zombieTarget.transform.position, this.transform.position);
			if (characterDistance <= distanceToNotice) {
				if (LineOfSight (zombieTarget.transform)) {
					zombieGPS.destination = zombieTarget.transform.position;
					zombieGPS.stoppingDistance = 0;
//					transform.LookAt(zombieTarget.transform);
				} else if (zombieGPS.remainingDistance <= 3) {
					zombieGPS.stoppingDistance = 1;
					zombieState = States.Search;
				}
			}
		} else if (zombieState == States.Search && zombieGPS.remainingDistance <= 1) {
			if (!wanderingSearching) {
				wanderingSearching = true;
				runningCoroutine = StartCoroutine (RandomiseWander (zombieGPS.destination));
				StartCoroutine (DisableWander ());
			}
		} else if (zombieState == States.Noise && zombieGPS.remainingDistance <= 1) {
			zombieGPS.speed = wanderSpeed;
			zombieState = States.Search;
		} else if (zombieState == States.Wander && zombieGPS.remainingDistance <= 1) {
				StartCoroutine (RandomiseWander (this.transform.position));
				StartCoroutine (DisableWander ());
				wanderingSearching = true;
		}

		if (zombieState != States.Chase) {
			foreach (GameObject player in players) {
				float characterDistance = Vector3.Distance (player.transform.position, this.transform.position);
				if (characterDistance <= distanceToNotice) {
					if (LineOfSight (player.transform)) {
						zombieGPS.destination = player.transform.position;
						zombieTarget = player;
						zombieGPS.stoppingDistance = 0.1f;
						zombieGPS.speed = wanderSpeed * 2;
						zombieState = States.Chase;
						wanderingSearching = false;
					}
				}
			}
		}
	}

	IEnumerator RandomiseWander(Vector3 aroundPosition){
		while (true) {
			Vector3 point;
			if (RandomPoint (aroundPosition, wanderRange, out point)) {
				zombieGPS.destination = point;
				yield return new WaitForSeconds (1 + Random.Range (0f, 3));
			}
		}
	}

	IEnumerator DisableWander(){
		yield return new WaitForSeconds (15);
		StopCoroutine (runningCoroutine);
		zombieState = States.Wander;
		wanderingSearching = false;
	}

	public void baitedZombie(Vector3 baitLocation){
		wanderingSearching = false;
		if (zombieState != States.Chase) {
			if (Vector3.Distance (this.transform.position, baitLocation) < maximumNoiseDistance) {
				zombieState = States.Noise;
				// Set the agent's new destination to the player's position
				zombieGPS.destination = baitLocation;
				zombieGPS.speed = wanderSpeed * 2;
				zombieGPS.stoppingDistance = 1;
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
