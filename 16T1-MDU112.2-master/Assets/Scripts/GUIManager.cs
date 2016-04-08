using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	GameManager gameManager;

	public Text cashText;
	public Text waveText;
	public Text livesText;
	public Text towerText;

	public GameObject pauseMenu;
	private bool paused = false;
	public GameObject gameOverTextObject;
	public Text gameOverText;

	public Sprite sadFace;
	public Sprite happyFace;
	public Image face;


	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();

		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {

		//Check for student interfaces - Happy Face
		if (gameManager.usingStudentGameLogicClass == true 
		    && gameManager.usingStudentEnemyLogicClass == true 
		    && gameManager.usingStudentTowerLogicClass == true)
			face.sprite = happyFace;

		//Pause Menu Input Checking, Time Stop & Display
		if (paused == false && Input.GetKeyDown ("escape")) {
			paused = true;
		} else if (paused == true) {
			pauseMenu.SetActive (true);
			Time.timeScale = 0;
			if(Input.GetKeyDown ("escape")){
				paused = false;
				Time.timeScale = 1;
				pauseMenu.SetActive (false);
			}
		}

		//Game Over Conditional Checking
		if (gameManager.gameOver == false) {
			//Update constant GUI elements
			towerText.text = gameManager.towers[gameManager.towerSelection].name.ToString () + " (" + gameManager.towersCost[gameManager.towerSelection].ToString() + ")";
			cashText.text = "Cash: $" + gameManager.Cash.ToString ();
			livesText.text = "Lives: " + gameManager.Lives.ToString ();
			//Update wave number only if it does not exceed maxwave
			if(gameManager.Wave <= gameManager.MaxWave)
				waveText.text = "Wave: " + gameManager.Wave.ToString ();
		} else if (gameManager.gameOver == true) {
			//Logic controlling display of win/lose screen
			gameOverTextObject.SetActive(true);
			paused = true;

			if(gameManager.Lives > 0)
				gameOverText.text = "You Win!";
			else {
				livesText.text = "Lives: " + gameManager.Lives.ToString ();
				gameOverText.text = "You Lose!";
			}
		}
	}


	public void restartGame(){
		Application.LoadLevel (Application.loadedLevel);
	}

	public void returnToMainMenu(){
		Application.LoadLevel ("Main Menu");
	}
}
