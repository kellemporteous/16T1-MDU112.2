using UnityEngine;
using System.Collections;

public class Missile : Projectile {
	
	private Quaternion targetRotation;
	
	public float rotationSpeed = 1.0f;
	
	// Use this for initialization
	void Start () {
		
		//Launch Audio Parenting
		GameObject launchAudio = Instantiate (sound, transform.position, transform.rotation) as GameObject;
		launchAudio.transform.parent = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Projectile Movement
		transform.position += Time.deltaTime * projectileSpeed * transform.forward;
		
		//Missile Behaviour
		if (lockedEnemy != null) {
			
			//Smooth Lock
			//Determine the target rotation. This is the rotation if the transform looks at the target point
			targetRotation = Quaternion.LookRotation (lockedEnemy.transform.position - transform.position);
			
			//Smoothly rotate towards the target point.
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}
	
	
	void OnTriggerEnter(Collider otherObject){
		if (otherObject.tag == "Enemy" || otherObject.tag == "Environment") {
			otherObject.SendMessage ("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
			trail.transform.parent = null;
			Instantiate(hitEffect, transform.position, transform.rotation);
			Instantiate (hitSound, transform.position, transform.rotation);
			Destroy (this.gameObject);
		} 
	}
}
