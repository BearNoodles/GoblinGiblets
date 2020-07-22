using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToBegin : MonoBehaviour {

    public GameObject blur; // UI blur effect
    public GameObject minigame; // reference to the catapult minigame object
    private CatapultScript minigameScript; // script for the catapult

	// Use this for initialization
	void Start () {
        // display all the tuturial parts
        blur.SetActive(true);        
        minigameScript = minigame.GetComponent<CatapultScript>();
        minigameScript.tutorialActive = true;
	}
	
	// Update is called once per frame
	void Update () {
        // Wait for user input to start the game
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {            
            // hide the tutorial ui objects
            blur.SetActive(false);
            gameObject.SetActive(false);
            minigameScript.tutorialActive = false;
        }
    }
}
