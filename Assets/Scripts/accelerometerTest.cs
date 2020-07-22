using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script was a test of tilt functionality on android, for possible use in minigames
// we didnt implement the tilt/motion controls in any games in the end.
public class accelerometerTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // this will rotate a game object the script is attatched to as the phone tilts
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, Input.acceleration.x * 180);
    }
}
