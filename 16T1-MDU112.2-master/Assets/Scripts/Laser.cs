using UnityEngine;
using System.Collections;

public class Laser : Projectile {

	GameObject laserSound;

	// Use this for initialization
	void Start () {
		//Launch Audio Parenting
		laserSound = Instantiate (sound, transform.position, transform.rotation) as GameObject;

		//Launch Effect
		Instantiate(hitEffect, transform.position, transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		//Projectile Movement
		transform.position += Time.deltaTime * projectileSpeed * transform.forward;
	}


	void OnTriggerEnter(Collider otherObject){
		if (otherObject.tag == "Enemy" || otherObject.tag == "Environment") {
			otherObject.SendMessage ("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
			trail.transform.parent = null;
			laserSound.transform.parent = null;
			Instantiate(hitEffect, transform.position, transform.rotation);
			Destroy (this.gameObject);
		} 
	}
}
