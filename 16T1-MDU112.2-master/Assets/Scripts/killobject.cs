using UnityEngine;
using System.Collections;

public class killobject : MonoBehaviour {
	
	private float lifeTime;
	public float lifeTimeDuration = 1.0f;

	// Use this for initialization
	void Start () {
		lifeTime = Time.time + lifeTimeDuration;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > lifeTime){
			Destroy(this.gameObject);
		}
	}
	
}
