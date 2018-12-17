using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main_menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void Update () {
		if (Input.GetKeyDown ("escape")) {
			Application.Quit();
		}

	}

	public void PressStart(){			
		SceneManager.LoadScene ("Main", LoadSceneMode.Single);
	}

	public void Credits (){			
		SceneManager.LoadScene ("Credits", LoadSceneMode.Single);
	}
	public void Quit (){			
		Application.Quit();
	}

	public void GoBack (){			
		SceneManager.LoadScene ("Menu", LoadSceneMode.Single);
	}
}
