using UnityEngine;
using System;
using System.Reflection;

public class GameManager : ReflectionEvaluation {

	public GameObject[] enemyList;
	
	public GameObject[] towers;
	public int[] towersCost;
	public int towerSelection = 0;
	
	//Enemy Spawning Variables
	public GameObject enemy;
	public GameObject spawner;
	
	public bool gameOver = false;

	//Mouse Controls
	public Vector3 mousePosition;

	#region Reflection Setup
	// Default object types	
	public System.Type gameLogicType = null;

	public object activeGameLogic = null;
	
	//Reflection Interfaces
	Assembly fallbackAssembly;
	//GameLogic
	FieldInfo wave;
	FieldInfo maxWave;
	FieldInfo cash;
	FieldInfo lives;
	MethodInfo updateCashMethod;
	MethodInfo updateLivesMethod;
	MethodInfo haveCashForTowerMethod;
	MethodInfo spawnAmountMethod;
	MethodInfo spawnIntervalMethod;
	MethodInfo spawnWaveEnemyMethod;
	
	public bool usingStudentGameLogicClass = false;
	public bool usingStudentEnemyLogicClass = false;
	public bool usingStudentTowerLogicClass = false;
	
	public static GameManager Instance {get; private set;}

	void Awake() {
		Instance = this;
		
		// Load the fallback core		
		fallbackAssembly = FallbackLoader.LoadFallbackLogic ();
		gameLogicType = fallbackAssembly.GetType("FallbackGameLogic");
		
		// Run the validation code to patch in the appropriate types
		usingStudentGameLogicClass = ValidateGameLogicClass();

		// Check if we are using the fallback code
		ConstructorInfo gameLogicConstructor = gameLogicType.GetConstructor(Type.EmptyTypes);
		activeGameLogic = gameLogicConstructor.Invoke(Type.EmptyTypes);

		//Retrieve the required methods/fields for...
		//GameLogic
		wave			 				= gameLogicType.GetField("wave");
		maxWave			 				= gameLogicType.GetField("maxWave");
		cash			 				= gameLogicType.GetField("cash");
		lives			 				= gameLogicType.GetField("lives");
		updateCashMethod				= gameLogicType.GetMethod("updateCash");
		updateLivesMethod				= gameLogicType.GetMethod("updateLives");
		haveCashForTowerMethod			= gameLogicType.GetMethod("haveCashForTower");
		spawnAmountMethod				= gameLogicType.GetMethod("spawnAmount");
		spawnIntervalMethod				= gameLogicType.GetMethod("spawnInterval");
		spawnWaveEnemyMethod			= gameLogicType.GetMethod("spawnWaveEnemy");
	}

	bool ValidateGameLogicClass() {
		bool gameLogicValid = true;
		
		// Retrieve the type of the student supplied gamestate
		System.Type potentialGameLogicType = typeof(GameLogic);
		
		// Test the variables
		gameLogicValid &= EvaluateVariableStatus(potentialGameLogicType, "wave", typeof(int)) == InterfaceStatus.Valid;
		gameLogicValid &= EvaluateVariableStatus(potentialGameLogicType, "maxWave", typeof(int)) == InterfaceStatus.Valid;
		gameLogicValid &= EvaluateVariableStatus(potentialGameLogicType, "cash", typeof(int)) == InterfaceStatus.Valid;
		gameLogicValid &= EvaluateVariableStatus(potentialGameLogicType, "lives", typeof(int)) == InterfaceStatus.Valid;
		gameLogicValid &= EvaluateVariableStatus(potentialGameLogicType, "waveCounter", typeof(int)) == InterfaceStatus.Valid;
		gameLogicValid &= EvaluateVariableStatus(potentialGameLogicType, "spawnTimer", typeof(float)) == InterfaceStatus.Valid;
		
		// Test the functions
		gameLogicValid &= EvaluateMethodStatus(potentialGameLogicType, "updateCash", typeof(void), 
		                                       new[]{typeof(int)}, new[]{false}) == InterfaceStatus.Valid;

		gameLogicValid &= EvaluateMethodStatus(potentialGameLogicType, "updateLives", typeof(void), 
		                                       new[]{typeof(int)}, new[]{false}) == InterfaceStatus.Valid;

		gameLogicValid &= EvaluateMethodStatus(potentialGameLogicType, "haveCashForTower", typeof(bool), 
		                                       new[]{typeof(int)}, new[]{false}) == InterfaceStatus.Valid;

		gameLogicValid &= EvaluateMethodStatus(potentialGameLogicType, "spawnAmount", typeof(int), 
		                                       new[]{typeof(int)}, new[]{false}) == InterfaceStatus.Valid;

		gameLogicValid &= EvaluateMethodStatus(potentialGameLogicType, "spawnInterval", typeof(float), 
		                                       new[]{typeof(int)}, new[]{false}) == InterfaceStatus.Valid;

		gameLogicValid &= EvaluateMethodStatus(potentialGameLogicType, "spawnWaveEnemy", typeof(bool), 
		                                       new[]{typeof(int), typeof(float)}, new[]{false,false}) == InterfaceStatus.Valid;

		// If all tests passed then set the game state type
		if (gameLogicValid) {
			gameLogicType = potentialGameLogicType;
		}
		
		return gameLogicValid;
	}
	#endregion


	//Initialization
	void Start(){
		//Instantiate an Enemy and Tower prefab for interface checking (happy face)
		GameObject initiatedEnemy = Instantiate (enemy, transform.position, transform.rotation) as GameObject;
		initiatedEnemy.GetComponent <Enemy> ().initiation = true;
		GameObject initiatedTower = Instantiate (towers[0], transform.position, transform.rotation) as GameObject;
		initiatedTower.GetComponent <Tower> ().initiation = true;
	}

	// Update is called once per frame
	void Update () {
		
		//Game Over Logic - player loses all lives
		if (Lives <= 0 || Wave >= MaxWave && enemyList.Length == 0) {
			gameOver = true;
		}

		//Wave Spawning
		//If game isnt over, keep spawing enemies
		if (gameOver == false && Wave <= MaxWave) {
			//GameLogic check to see if it is time to spawn an enemy in the current wave
			if(GameLogic_spawnWaveEnemy(GameLogic_spawnAmount (Wave), GameLogic_spawnInterval(Wave))){
				Instantiate (enemy, spawner.transform.position, spawner.transform.rotation);
			}
		}
		
		//Update enemy list every game loop
		enemyList = GameObject.FindGameObjectsWithTag ("Enemy");
		
		//Controls - Keyboard
		//Tower selection
		if (Input.GetKeyDown ("1")) {
			towerSelection = 0;
		} else if (Input.GetKeyDown ("2")) {
			towerSelection = 1;
		}
		
		//Controls - Mouse
		//Raycast tower platform selection
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit)) {
			mousePosition = hit.point;
			Debug.DrawLine (Camera.main.transform.position, mousePosition, Color.green);
			
			//Left click - Place Tower
			if (Input.GetMouseButtonDown (0) && hit.transform.tag == "TowerPlatform" && GameLogic_haveCashForTower(towersCost[towerSelection]) == true) {
				hit.transform.GetComponent<TowerPlatform>().placeTower (towers[towerSelection], towersCost[towerSelection]);
			}
		}
	}
	
	#region Reflection Interfaces

	//GameLogic
	public int Wave {
		get {
			return (int)wave.GetValue(activeGameLogic);
		}
	}
	
	public int MaxWave {
		get {
			return (int)maxWave.GetValue(activeGameLogic);
		}
	}
	
	public int Cash {
		get {
			return (int)cash.GetValue(activeGameLogic);
		}
	}
	
	public int Lives {
		get {
			return (int)lives.GetValue(activeGameLogic);
		}
	}

	public void GameLogic_updateCash(int adjustedCash) {
		updateCashMethod.Invoke(activeGameLogic, new []{(object)adjustedCash});
	}

	public void GameLogic_updateLives(int adjustedLives) {
		updateLivesMethod.Invoke(activeGameLogic, new []{(object)adjustedLives});
	}
	
	private bool GameLogic_haveCashForTower(int towerCost) {
		return (bool) haveCashForTowerMethod.Invoke(activeGameLogic, new []{(object)towerCost});
	}

	private int GameLogic_spawnAmount(int currentWave) {
		return (int) spawnAmountMethod.Invoke(activeGameLogic, new []{(object)currentWave});
	}

	private float GameLogic_spawnInterval(int currentWave) {
		return (float) spawnIntervalMethod.Invoke(activeGameLogic, new []{(object)currentWave});
	}

	private bool GameLogic_spawnWaveEnemy(int spawnAmount, float spawnInterval) {
		return (bool) spawnWaveEnemyMethod.Invoke(activeGameLogic, new []{(object)spawnAmount, (object)spawnInterval});
	}
	#endregion
}
