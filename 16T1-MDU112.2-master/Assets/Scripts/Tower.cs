using UnityEngine;
using System;
using System.Reflection;

public class Tower : ReflectionEvaluation{

	GameManager gameManager;

	public bool initiation = false;

	public GameObject turret;
	public GameObject[] turretMuzzles;
	public GameObject projectile;
	private Quaternion targetRotation;
	public float rotationSpeed = 5.0f;
	private float adjRotSpeed;
	private float range = 1000.0f;
	private GameObject enemyTarget = null;

	//Effects
	public GameObject towerSpawnEffect;

	//Audio
	public GameObject towerSpawnSound;

	#region Reflection Setup
	// Default object types	
	public System.Type towerLogicType = null;
	
	public object activeTowerLogic = null;

	//Reflection Interfaces
	public Assembly fallbackAssembly;
	//TowerLogic
	FieldInfo fireRate;
	FieldInfo damage;
	FieldInfo fireTime;
	MethodInfo closestEnemyMethod;
	MethodInfo fireProjectileMethod;

	public bool usingStudentTowerLogicClass = false;
	
	public static Tower Instance {get; private set;}

	void Awake() {
		Instance = this;

		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
		
		// Load the fallback core		
		fallbackAssembly = FallbackLoader.LoadFallbackLogic ();
		towerLogicType = fallbackAssembly.GetType("FallbackTowerLogic");
		
		// Run the validation code to patch in the appropriate types
		usingStudentTowerLogicClass = ValidateTowerLogicClass();

		// Check if we are using the fallback code
		ConstructorInfo towerLogicConstructor = towerLogicType.GetConstructor(new []{typeof(int)});
		activeTowerLogic = towerLogicConstructor.Invoke(new [] {(object)gameManager.towerSelection});

		//Retrieve the required methods/fields for...
		//TowerLogic
		fireRate		 				= towerLogicType.GetField("fireRate");
		damage		 					= towerLogicType.GetField("damage");
		fireTime		 				= towerLogicType.GetField("fireTime");
		closestEnemyMethod 				= towerLogicType.GetMethod("closestEnemy");
		fireProjectileMethod			= towerLogicType.GetMethod("fireProjectile");
	}

	bool ValidateTowerLogicClass() {
		bool towerLogicValid = true;
		
		// Retrieve the type of the student supplied crop
		System.Type potentialTowerLogicType = typeof(TowerLogic);
		
		// Test the variables
		towerLogicValid &= EvaluateVariableStatus(potentialTowerLogicType, "fireRate", typeof(float)) == InterfaceStatus.Valid;
		towerLogicValid &= EvaluateVariableStatus(potentialTowerLogicType, "damage", typeof(float)) == InterfaceStatus.Valid;
		towerLogicValid &= EvaluateVariableStatus(potentialTowerLogicType, "fireTime", typeof(float)) == InterfaceStatus.Valid;
		towerLogicValid &= EvaluateVariableStatus(potentialTowerLogicType, "muzzleIndex", typeof(int)) == InterfaceStatus.Valid;
		
		// Test the constructors
		towerLogicValid &= EvaluateConstructorStatus(potentialTowerLogicType, new[]{typeof(int)}) == InterfaceStatus.Valid;
		
		// Test the functions
		towerLogicValid &= EvaluateMethodStatus(potentialTowerLogicType, "closestEnemy", typeof(GameObject), 
		                                        new[]{typeof(Vector3),typeof(GameObject[])}, new[]{false,false}) == InterfaceStatus.Valid;
		towerLogicValid &= EvaluateMethodStatus(potentialTowerLogicType, "fireProjectile", typeof(int), 
		                                        new[]{typeof(GameObject[]),typeof(float)}, new[]{false,false}) == InterfaceStatus.Valid;
		
		// If all tests passed then set the crop type
		if (towerLogicValid) {
			towerLogicType = potentialTowerLogicType;
		}
		
		return towerLogicValid;
	}
	#endregion
	
	// Use this for initialization
	void Start () {

		//Update Interface Initiation Check
		if (initiation == true) {
			if (usingStudentTowerLogicClass == true)
				gameManager.usingStudentTowerLogicClass = true;
			Destroy (this.gameObject);
		} else {
			//Tower Spawn Effects
			Instantiate (towerSpawnEffect, transform.position, transform.rotation);
			Instantiate (towerSpawnSound, transform.position, transform.rotation);
		}
	}
	
	// Update is called once per frame
	void Update () {

		//Obtain a potential enemyTarget from enemyList
		enemyTarget = TowerLogic_closestEnemy (transform.position, gameManager.enemyList);

		//Rotate turret to look at closest enemy
		if (enemyTarget != null) 
			TrackAndFire();
	}

	private void TrackAndFire(){
		//Lerp rotate turrettowards target
		targetRotation = Quaternion.LookRotation (enemyTarget.transform.position - turret.transform.position);
		adjRotSpeed = Mathf.Min (rotationSpeed * Time.deltaTime, 1);
		turret.transform.rotation = Quaternion.Lerp (turret.transform.rotation, targetRotation, adjRotSpeed);
		
		//if enemyTarget is within line of sight, look at it and shoot at it
		RaycastHit hit;
		if (Physics.Raycast (turretMuzzles[0].transform.position, -(turretMuzzles[0].transform.position - enemyTarget.transform.position).normalized, out hit, range)){

			//Check if hit is an enemy and attack it
			if (hit.transform.tag == "Enemy"){
				Debug.DrawLine (turretMuzzles[0].transform.position, enemyTarget.transform.position, Color.green);
				//Check to find if it is time to fire AND which muzzle to fire from
				if(Time.time > FireTime){
					int muzzle = TowerLogic_fireProjectile (turretMuzzles, FireRate);
					GameObject thisProjectile = Instantiate (projectile, turretMuzzles[muzzle].transform.position, turretMuzzles[muzzle].transform.rotation) as GameObject;

					thisProjectile.GetComponent<Projectile>().damage = Damage;

					thisProjectile.GetComponent<Projectile>().lockedEnemy = enemyTarget;
				}
			} else {
				Debug.DrawLine (turretMuzzles[0].transform.position, enemyTarget.transform.position, Color.red);
			}
		}
	}

	#region Reflection Interfaces
	//TowerLogic Wrappers
	public float FireRate {
		get {
			return (float)fireRate.GetValue(activeTowerLogic);
		}
	}

	public float FireTime {
		get {
			return (float)fireTime.GetValue(activeTowerLogic);
		}
	}
	
	public float Damage {
		get {
			return (float)damage.GetValue(activeTowerLogic);
		}
	}
	
	private GameObject TowerLogic_closestEnemy(Vector3 towerPosition, GameObject[] enemyList) {
		return (GameObject) closestEnemyMethod.Invoke(activeTowerLogic, new []{(object)towerPosition, (object)enemyList});
	}
	
	private int TowerLogic_fireProjectile(GameObject[] enemyList, float nextFireTime) {
		return (int) fireProjectileMethod.Invoke(activeTowerLogic, new []{(object)enemyList, (object)nextFireTime});
	}
	#endregion
}
