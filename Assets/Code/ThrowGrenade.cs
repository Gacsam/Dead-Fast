using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour {

	public GameObject grenadePrefab;
	public Transform grenadeTransform;
	private GameObject heldGrenade;
	public int throwForce = 15;

	void Start(){
		heldGrenade = GameObject.Find ("HeldGrenade");

	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			StartCoroutine(GrenadeThrow());
		}
	}

	IEnumerator GrenadeThrow()
	{
		heldGrenade.SetActive (false);
		GameObject grenade = Instantiate (grenadePrefab, grenadeTransform.position, grenadeTransform.rotation);
		grenade.GetComponent<Rigidbody> ().velocity = grenade.transform.forward * throwForce;
		yield return new WaitForSeconds(1);
		heldGrenade.SetActive (true);
	}
}