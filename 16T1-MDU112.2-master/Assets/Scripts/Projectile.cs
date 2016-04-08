using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float damage;
	public float projectileSpeed;

	public float lifeTime;

	public GameObject lockedEnemy;

	//Effects
	public GameObject trail;
	public GameObject hitEffect;
	
	//Audio
	public GameObject sound;
	public GameObject hitSound;

	// Use this for initialization
	void Start () {
	
		//Set object kill time
		Destroy (this.gameObject, lifeTime);
	}
}
