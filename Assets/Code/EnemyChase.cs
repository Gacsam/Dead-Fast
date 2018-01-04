using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour {

	public Transform player;
	static Animator anim;

	string state = "patrol";
	public GameObject[] pathways;
	int currentPW = 0;
	public float rotSpeed = 0.2f;
	public float speed = 1.5f;
	float accuracyPW = 5.0f;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		Vector3 direction = player.position - this.transform.position;
		direction.y = 0;
		float angle = Vector3.Angle (direction, this.transform.forward);
			
		if (state == "patrol" && pathways.Length > 0)
		{
					anim.SetBool ("isIdle", false);
				    anim.SetBool ("isWalking", true);
				    if (Vector3.Distance (pathways [currentPW].transform.position, transform.position) < accuracyPW)
			{
				currentPW = Random.Range (0, pathways.Length);

					//currentPW++;
					//if (currentPW >= pathways.Length)
			//	{
			//			currentPW = 0;

					} 


					//rotate towards pathpoints
					direction = pathways [currentPW].transform.position - transform.position;
					this.transform.rotation = Quaternion.Slerp (transform.rotation,
					                     Quaternion.LookRotation (direction), rotSpeed * Time.deltaTime);
					this.transform.Translate (0, 0, Time.deltaTime * speed);
				}
			if (Vector3.Distance (player.position, this.transform.position) < 10 && (angle < 30 || state == "pursuing")) {
				state = "pursuing";
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, 
					Quaternion.LookRotation (direction), rotSpeed * Time.deltaTime);
				
				if (direction.magnitude > 5) {
					this.transform.Translate (0, 0, Time.deltaTime * speed);
					anim.SetBool ("isWalking", true);
					anim.SetBool ("isAttacking", false);
				
				} else {
					anim.SetBool ("isAttacking", true);
					anim.SetBool ("isWalking", false);
				}
			}

			else
			{
					anim.SetBool ("isWalking", true);
					anim.SetBool ("isAttacking", false);
					state = "patrol";
				}




			}
		}
	

