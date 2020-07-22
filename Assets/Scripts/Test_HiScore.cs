using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this script was only used to display the high score for the original testing of the system
public class Test_HiScore : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // update text 
        GetComponent<Text>().text = "" + SaveLoad.getServingHiScore();
	}
}
