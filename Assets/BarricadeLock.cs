using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeLock : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.GetComponent<Rigidbody> ().velocity = Vector3.down;
	}
	
	// Update is called once per frame
	void OnCollisionEnter (Collision col) {
		if(col.gameObject.tag == "Terrain"){
			this.GetComponent<Rigidbody> ().isKinematic = true;
			Destroy (this.GetComponent<BarricadeLock> ());
		}
	}
}