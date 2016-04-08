using UnityEngine;
using System.Collections;

public class MenuGUIManager : MonoBehaviour {

	public GameObject mainScreen;

	public GameObject instructionsScreen;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGame(){
		Application.LoadLevel ("Tower Defense");
	}

	public void showInstructions(){
		mainScreen.SetActive (false);
		instructionsScreen.SetActive (true);
	}

	public void backToMain(){
		mainScreen.SetActive (true);
		instructionsScreen.SetActive (false);
	}

	public void quitGame(){
		Application.Quit ();
	}
}
