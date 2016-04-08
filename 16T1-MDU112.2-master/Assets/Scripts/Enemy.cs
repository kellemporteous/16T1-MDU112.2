using UnityEngine;
using System;
using System.Reflection;

public class Enemy : ReflectionEvaluation {

	GameManager gameManager;

	private NavMeshAgent agent;

	private GameObject endPoint;

	public bool initiation = false;
	
	//Effects
	public GameObject enemyExplosion;

	//Audio
	public GameObject enemyExplosionSound;

	#region Reflection Setup
	// Default object types	
	System.Type enemyLogicType = null;

	public object activeEnemyLogic = null;
	
	//Reflection Interfaces
	Assembly fallbackAssembly;
	//EnemyLogic
	FieldInfo speed;
	FieldInfo health;
	FieldInfo cashReward;
	FieldInfo enemyPenalty;
	MethodInfo takeDamageMethod;

	public bool usingStudentEnemyLogicClass = false;
	
	public static Enemy Instance {get; private set;}
	
	void Awake() {

		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();

		Instance = this;
		
		// Load the fallback core		
		fallbackAssembly = FallbackLoader.LoadFallbackLogic ();
		enemyLogicType = fallbackAssembly.GetType("FallbackEnemyLogic");
		
		// Run the validation code to patch in the appropriate types
		usingStudentEnemyLogicClass = ValidateEnemyLogicClass();

		// Check if we are using the fallback code
		ConstructorInfo enemyLogicConstructor = enemyLogicType.GetConstructor(new []{typeof(int)});
		activeEnemyLogic = enemyLogicConstructor.Invoke(new [] {(object)gameManager.Wave});

		//Retrieve the required methods/fields for...
		//EnemyLogic
		speed			 				= enemyLogicType.GetField("speed");
		health			 				= enemyLogicType.GetField("health");
		cashReward			 			= enemyLogicType.GetField("cashReward");
		enemyPenalty			 		= enemyLogicType.GetField("enemyPenalty");
		takeDamageMethod 				= enemyLogicType.GetMethod("takeDamage");
	}

	
	bool ValidateEnemyLogicClass() {
		bool enemyLogicValid = true;
		
		// Retrieve the type of the student supplied crop
		System.Type potentialEnemyLogicType = typeof(EnemyLogic);
		
		// Test the variables
		enemyLogicValid &= EvaluateVariableStatus(potentialEnemyLogicType, "health", typeof(float)) == InterfaceStatus.Valid;
		enemyLogicValid &= EvaluateVariableStatus(potentialEnemyLogicType, "speed", typeof(float)) == InterfaceStatus.Valid;
		enemyLogicValid &= EvaluateVariableStatus(potentialEnemyLogicType, "cashReward", typeof(int)) == InterfaceStatus.Valid;
		enemyLogicValid &= EvaluateVariableStatus(potentialEnemyLogicType, "enemyPenalty", typeof(int)) == InterfaceStatus.Valid;
		
		// Test the constructors
		enemyLogicValid &= EvaluateConstructorStatus(potentialEnemyLogicType, new[]{typeof(int)}) == InterfaceStatus.Valid;
		
		// Test the functions
		enemyLogicValid &= EvaluateMethodStatus(potentialEnemyLogicType, "takeDamage", typeof(void), 
		                                        new[]{typeof(float)}, new[]{false}) == InterfaceStatus.Valid;
		
		
		// If all tests passed then set the crop type
		if (enemyLogicValid) {
			enemyLogicType = potentialEnemyLogicType;
		}
		
		return enemyLogicValid;
	}
	#endregion

	// Use this for initialization
	void Start () {

		//Update Interface Check
		if (initiation == true){
			if (usingStudentEnemyLogicClass == true)
				gameManager.usingStudentEnemyLogicClass = true;
			Destroy (this.gameObject);
		}

		agent = GetComponent<NavMeshAgent> ();

		endPoint = GameObject.FindGameObjectWithTag ("End");

		agent.speed = Speed;
	}
	
	// Update is called once per frame
	void Update () {

		Move ();

		//Check Death
		if (Health <= 0) {
			Destroy ();
		}
	}

	//Hook into EnemyLogic - necessary for Enemy to take damage
	public void takeDamage(float damage){
		EnemyLogic_takeDamage (damage);
	}


	//Method used to move enemy from start point to end
	private void Move(){
		agent.SetDestination (endPoint.transform.position);
	}
	
	void OnTriggerEnter(Collider otherObject){
		if (otherObject.tag == "End") {
			gameManager.GameLogic_updateLives(EnemyPenalty);
			Destroy (this.gameObject);
		}
	}

	private void Destroy(){

		//Assignment specific call to make gameLogic method work
		gameManager.GameLogic_updateCash (CashReward);

		Instantiate (enemyExplosion, transform.position, transform.rotation);
		Instantiate (enemyExplosionSound, transform.position, transform.rotation);
		Destroy (this.gameObject);
	}

	#region Reflection Interfaces
	//EnemyLogic Wrappers
	public float Speed {
		get {
			return (float)speed.GetValue(activeEnemyLogic);
		}
	}

	public float Health {
		get {
			return (float)health.GetValue(activeEnemyLogic);
		}
	}

	public int CashReward {
		get {
			return (int)cashReward.GetValue(activeEnemyLogic);
		}
	}

	public int EnemyPenalty {
		get {
			return (int)enemyPenalty.GetValue(activeEnemyLogic);
		}
	}
	
	private void EnemyLogic_takeDamage(float damage) {
		takeDamageMethod.Invoke(activeEnemyLogic, new []{(object)damage});
	}
	#endregion
}
