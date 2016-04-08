using UnityEngine;
using System.Collections;

public class TowerPlatform : MonoBehaviour {

	GameManager gameManager;

	private bool towerPlaced = false;

	public GameObject spawnLocation;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Method used to place towers on Tower Platforms
	public void placeTower(GameObject tower, int towerCost){

		if (towerPlaced == false) {
			GameObject thisTower = Instantiate (tower, spawnLocation.transform.position, spawnLocation.transform.rotation) as GameObject;

			gameManager.GameLogic_updateCash (-towerCost);

			thisTower.transform.parent = this.transform;

			towerPlaced = true;
		}
	}
}
