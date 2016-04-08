using UnityEngine;
using System.Collections;

public class MenuTower : MonoBehaviour {

	public GameObject turret;

	public GameObject[] targets;

	private int currentTarget;

	private float nextTargetTime;


	private Quaternion targetRotation;
	public float rotationSpeed = 0.5f;
	private float adjRotSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//Change target every nextTargetTime
		if (Time.time > nextTargetTime) {
			currentTarget = Random.Range (0, targets.Length);
			nextTargetTime = Time.time + Random.Range (2.5f, 4.5f);
		}

		//Lerp rotate towards target
		targetRotation = Quaternion.LookRotation (targets[currentTarget].transform.position - turret.transform.position);
		adjRotSpeed = Mathf.Min (rotationSpeed * Time.deltaTime, 1);
		turret.transform.rotation = Quaternion.Lerp (turret.transform.rotation, targetRotation, adjRotSpeed);
	}
}
