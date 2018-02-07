// RandomPointOnNavMesh.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
public class ZombieFarmChase : MonoBehaviour {
	[Tooltip("Zombie wandering range")]
	public float wanderRange = 10.0f;
	[Tooltip("Zombie searching range")]
	public float searchRange = 10.0f;
	[Tooltip("Angle of view the zombie notices")]
	public float angleOfView = 90;
	[Tooltip("Distance between the bait and zombie to notice")]
	public float distanceToNotice = 10;
	[Tooltip("Zombie wandering speed, running is doubled")]
	public float wanderSpeed = 1;
	[Tooltip("How far do zombies hear")]
	public float maximumNoiseDistance = 25;
	[Tooltip("After what time do zombies stop looking around the area")]
	public int searchTime = 10;
	public enum States{Wander, Noise, Search, Follow};
	public States zombieState;
	private NavMeshAgent zombieGPS;
	private GameObject zombieTarget;
	private List<GameObject> existingBait;
	private float searchCountdown;
	private Vector3 lastSeenPosition;
	private bool zombieCanWander = true;

	void Start() {
		zombieGPS = this.GetComponent<NavMeshAgent> ();
		zombieGPS.speed = wanderSpeed;
		zombieState = States.Wander;
		RandomiseWander (this.transform.position);
		existingBait = new List<GameObject> ();
		zombieGPS.stoppingDistance = 0.75f;
	}

	void Update() {
		// In order of importance: follow, search, noise, wander
		if (zombieState == States.Follow) {
			if (!zombieTarget) {
				if (LineOfSight (zombieTarget)) {
					zombieGPS.destination = zombieTarget.transform.position;
				} else {
					zombieState = States.Search;
					lastSeenPosition = zombieGPS.destination;
					RandomiseWander (lastSeenPosition);
					searchCountdown = searchTime;
				}
			}
		} else if (zombieState == States.Noise && zombieGPS.remainingDistance <= 1) {
			lastSeenPosition = zombieGPS.destination;
			zombieGPS.speed = wanderSpeed;
			zombieState = States.Search;
			searchCountdown = searchTime;
			RandomiseWander (lastSeenPosition);
		} else if (zombieState == States.Search) {
			if (zombieGPS.remainingDistance <= 1) {
				RandomiseWander (lastSeenPosition);
				if (searchCountdown <= 0) {
					zombieState = States.Wander;
				}
			}
			searchCountdown -= Time.deltaTime;
		} else if (zombieCanWander && zombieState == States.Wander && zombieGPS.remainingDistance <= 1) {
			RandomiseWander (this.transform.position);
			Debug.Log ("wowowowow");
		}

		// Check if not already following a bait
		if (!(zombieState == States.Follow)) {
			if (existingBait.Count > 0) {
				foreach (GameObject currentBait in existingBait) {
					if (Vector3.Distance (this.transform.position, currentBait.transform.position) > distanceToNotice) {
						if (LineOfSight (currentBait)) {
							zombieTarget = currentBait;
							zombieGPS.destination = currentBait.transform.position;
							zombieState = States.Follow;
						}
					}
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Y))
			zombieCanWander = !zombieCanWander;
		Debug.Log (zombieState.ToString() + zombieCanWander.ToString());
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

	void RandomiseWander(Vector3 aroundPosition){
		Vector3 point;
		float theRange = 5;
		if (zombieState == States.Search) {
			theRange = searchRange;
		} else if (zombieState == States.Wander) {
			theRange = wanderRange;
		}
		if (RandomPoint (aroundPosition, theRange, out point)) {
			zombieGPS.destination = point;
		}
	}

	public void baitedZombie(Vector3 baitLocation){
		// If not following a chicken or something
		if (!(zombieState == States.Follow)) {
			if (Vector3.Distance (this.transform.position, baitLocation) < maximumNoiseDistance) {
				zombieState = States.Noise;
				// Set the agent's new destination to the player's position
				zombieGPS.destination = baitLocation;
				zombieGPS.speed = wanderSpeed * 2;
			}
		}
	}

	bool LineOfSight (GameObject target) {
		RaycastHit hit;
		if (Vector3.Angle(target.transform.position - transform.position, transform.forward) <= angleOfView &&
			Physics.Raycast(transform.position, target.transform.position, out hit) &&
			(hit.collider.transform == target || hit.collider.transform.parent == target)) {
			Debug.DrawRay (transform.position, target.transform.position);
			return true;
		}
		return false;
	}
}
